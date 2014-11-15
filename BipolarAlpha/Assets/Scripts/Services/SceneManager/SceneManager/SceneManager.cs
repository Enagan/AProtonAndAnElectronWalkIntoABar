// Written by: Pedro Engana

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The Scene Manager class is responsible for dynamically creating the game world, as the player traverses it
/// By using it's assigned RoomFactory, the scene manager is capable of instancing new rooms, without a loading screen, giving the illusion of an open world.
/// </summary>
public class SceneManager : MonoBehaviour , IPlayerRoomChangeListner, IObjectRoomChangeListner
{
  private RoomDefinition _activeRoom;
  private List<RoomDefinition> _currentlyLoadedRooms;
  private Dictionary<string, RoomDefinition> _allRooms;

  // Editable Property, defines the level of depth the manager goes through when instancing rooms
  [SerializeField]
  private int _roomInstancingDepth = 1;

  //Delegate in charge of instancing all objects in a room and correctly placing them according to a specific world state
  private RoomFactory _roomFactory;

  private List<RoomDefinition> _roomsInQueueToBeDeleted;

	private void Start () 
  {
    ServiceLocator.ProvideSceneMananger(this);
    ServiceLocator.GetEventHandlerSystem().RegisterPlayerRoomChangeListner(this);
    ServiceLocator.GetEventHandlerSystem().RegisterObjectRoomChangeListner(this);

    #if UNITY_PRO_LICENSE
      _roomFactory = new RoomFactoryAsync();
    #else
      _roomFactory = new RoomFactory();
    #endif

    _currentlyLoadedRooms = new List<RoomDefinition>();
    _allRooms = new Dictionary<string, RoomDefinition>();
    _roomsInQueueToBeDeleted = new List<RoomDefinition>();

    KeyValuePair<string,List<RoomDefinition>> initState = ServiceLocator.GetSaveSystem().LoadInitialState();
    LoadNewWorldState(initState);
	}

  private void Update()
  {
    DeleteScheduledRooms();
  }

  #region Public - World State Loading and Saving
  /// <summary>
  /// Retrieves a definition of the current world state
  /// </summary>
  public KeyValuePair<string, List<RoomDefinition>> getCurrentWorldStateDefinition()
  {
    List<RoomDefinition> roomsToSave = new List<RoomDefinition>();
    RoomDefinition tempUpdatedRoomDef;
    foreach (RoomDefinition roomDef in _allRooms.Values)
    {
      SMConsole.Log(tag: "[SCENE MANAGER]", log: "Saving Room state for " + roomDef.roomName);
      tempUpdatedRoomDef = _roomFactory.UpdateRoomDefinition(roomDef);

      _allRooms[roomDef.roomName] = tempUpdatedRoomDef != null ? tempUpdatedRoomDef : roomDef;
    }

    return new KeyValuePair<string, List<RoomDefinition>>(_activeRoom.roomName, new List<RoomDefinition>(_allRooms.Values));
  }

  /// <summary>
  /// Loads a new world state. Destroys previously loaded state
  /// </summary>
  public void LoadNewWorldState(KeyValuePair<string, List<RoomDefinition>> initState)
  {
    DeleteAndClearWorldState();

    foreach (RoomDefinition room in initState.Value)
    {
      _allRooms.Add(room.roomName, room);
    }

    try
    {
      setActiveRoom(initState.Key);
    }
    catch(KeyNotFoundException exception)
    {
      SMConsole.Log(tag: "[SCENE MANAGER]", type: SMLogType.ERROR, log: exception.Message);
    }

    ServiceLocator.GetAudioSystem().PlayMusic("kahvi315z1_lackluster-sina");
  }

  /// <summary>
  /// Safely Destroys world state
  /// </summary>
  public void DeleteAndClearWorldState()
  {
    _activeRoom = null;
    SaveAndUninstanceRooms(_currentlyLoadedRooms);
    _allRooms.Clear();
  }
  #endregion

  public int roomInstancingDepth
  {
    get
    {
      return _roomInstancingDepth;
    }
    set
    {
      if (value < 1)
      {
        _roomInstancingDepth = 1;
        SMConsole.Log(tag: "[SCENE MANAGER]", type: SMLogType.ERROR,
          log: "Provided instancing depth" + value + " is not valid (must be greater than 1). roomInstancingDepth defaulted to 1.");
      }
      else
        _roomInstancingDepth = value;
    }
  }

  #region Public - Event Listners
  public void ListenPlayerRoomChange(string newRoomName)
  {
    SMConsole.Log(tag: "[SCENE MANAGER]", log: "[SCENE MANAGER] Player changed room to " + newRoomName + " updating instanced room tree");
    if (newRoomName != _activeRoom.roomName)
    {
      try
      {
        setActiveRoom(newRoomName);
      }
      catch(KeyNotFoundException exception)
      {
        SMConsole.Log(tag: "[SCENE MANAGER]", type: SMLogType.ERROR, log: exception.Message);
      }
    }
  }

  public void ListenObjectRoomChange(string prevRoomName, string newRoomName, GameObject objectChangedRoom)
  {
    SMConsole.Log(tag: "[SCENE MANAGER]", log: "[SCENE MANAGER] Movable object" + objectChangedRoom.name + " room changed from " + prevRoomName + " to " + newRoomName + " updating object definition");
    if (newRoomName != _activeRoom.roomName)
    {
      _roomFactory.ChangeObjectRoom(_allRooms[prevRoomName], _allRooms[newRoomName], objectChangedRoom);
    }
  }
  #endregion

  #region Private - Room Creation and Deletion
  /// <summary>
  /// Defines in which room the player is currently at, and sets it as the origin of the room instancing tree.
  /// From there, all adjacent rooms are instanced, and in case some rooms have fallen outside the instancing depth, they are deleted safely
  /// </summary>
  private void setActiveRoom(string newActiveRoomName)
  {
    SMConsole.Log(tag:"[SCENE MANAGER]", log:"Setting room " + newActiveRoomName + " as active");

    RoomDefinition newActiveRoomDefinition;
    if (!_allRooms.TryGetValue (newActiveRoomName, out newActiveRoomDefinition)) 
    {
      throw new KeyNotFoundException("KeyNotFoundException: room " + newActiveRoomName + " does not exist in loaded world state.");
    }

    List<RoomDefinition> previousLoadedRooms = new List<RoomDefinition>(_currentlyLoadedRooms);
    _currentlyLoadedRooms.Clear();

    SMConsole.Log(tag: "[SCENE MANAGER]", log:"------- Beggining instancing of room tree, root from " + newActiveRoomName + " ---------");

    InstanceRoomsFromRoot(newActiveRoomDefinition);

    SMConsole.Log(tag: "[SCENE MANAGER]", log: "---------- Finished Instancing room tree -----------");

    _activeRoom = newActiveRoomDefinition;

    //Compare newly instanced rooms to the previously loaded rooms, 
    //deletes any that exist in the previous version but not in the new.
    SaveAndUninstanceRooms(previousLoadedRooms, _currentlyLoadedRooms);
  }

  /// <summary>
  /// Safely saves and un-instances all rooms. Doesn't un-instance any room present in shouldIgnore
  /// </summary>
  private void SaveAndUninstanceRooms(List<RoomDefinition> roomsToDelete, List<RoomDefinition> shouldIgnore = null)
  {
    List<RoomDefinition> culledDeletionList = shouldIgnore != null ? roomsToDelete.FindAll(room => !shouldIgnore.Contains(room)) : roomsToDelete;

    foreach(RoomDefinition roomToDelete in culledDeletionList)
    {
      //Run updateRoomDefinition to update the definition according to the current world state of the room'
      RoomDefinition updatedDefinition = _roomFactory.UpdateRoomDefinition(roomToDelete);
      _allRooms[roomToDelete.roomName] = updatedDefinition;

      SMConsole.Log(tag: "[SCENE MANAGER]", log: "[SCENE MANAGER] Deleting room " + roomToDelete.roomName);

      if (!roomToDelete.inConstruction)
        _roomFactory.DestroyRoom(roomToDelete);
      else
        _roomsInQueueToBeDeleted.Add(roomToDelete);
    }
  }

  /// <summary>
  /// Traverses the room tree ordering the instancing of all rooms until it his the depth limit
  /// </summary>
  private void InstanceRoomsFromRoot(RoomDefinition root, int currentDepth = 0, RoomDefinition parent = null)
  {
     SMConsole.Log(tag: "[SCENE MANAGER]", log: "Depth: " + currentDepth + ",   Creating room " + root.roomName + "...");
     _roomsInQueueToBeDeleted.Remove(root);
     _roomFactory.CreateRoom(root, parent);
     _currentlyLoadedRooms.Add(root);
    //If we haven't reached the current depth, create all adjacent rooms to current root
    if (currentDepth < _roomInstancingDepth)
    {
      SMConsole.Log(tag: "[SCENE MANAGER]", log: "Depth: " + currentDepth + ",   This room " + root.roomName + " has  " + root.gateways.Count + " connections.");
      foreach (RoomObjectGatewayDefinition gate in root.gateways)
      {
        if (_allRooms.ContainsKey(gate.connectedToRoom))
          InstanceRoomsFromRoot(_allRooms[gate.connectedToRoom], ++currentDepth, root);
        else
          SMConsole.Log(tag: "[SCENE MANAGER]", log: "Room " + gate.connectedToRoom + " does not exist in loaded world state.");
      }
    }
  }

  private void DeleteScheduledRooms()
  {
    List<RoomDefinition> deletedRooms = new List<RoomDefinition>();
    foreach (RoomDefinition roomDef in _roomsInQueueToBeDeleted)
    {
      if (!roomDef.inConstruction)
      {
        _roomFactory.DestroyRoom(roomDef);
        deletedRooms.Add(roomDef);
      }
    }
    _roomsInQueueToBeDeleted.RemoveAll(room => deletedRooms.Contains(room));
  }
  #endregion
}
