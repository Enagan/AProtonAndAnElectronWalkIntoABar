// Written by: Pedro Engana

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SMSceneManagerSystem
{
  /// <summary>
  /// The Scene Manager class is responsible for dynamically creating the game world, as the player traverses it
  /// By using its assigned RoomFactory, the scene manager is capable of instancing new rooms, without a loading screen, giving the illusion of an open world.
  /// </summary>
  public class SceneManager : MonoBehaviour, IPlayerRoomChangeListner, IObjectRoomChangeListner
  {
    private RoomDefinition _activeRoom;
    private List<RoomDefinition> _currentlyInstancedRooms;
    private Dictionary<string, RoomDefinition> _allLoadedRooms;

    [SerializeField]
    private int _roomInstancingDepthLimit = 1;

    //Delegate in charge of instancing all objects in a room and correctly placing them according to a specific world state
    private RoomFactory _roomFactory;

    private List<RoomDefinition> _roomsInQueueToBeDeleted;

    private void Start()
    {
      ServiceLocator.ProvideSceneMananger(this);
      ServiceLocator.GetEventHandlerSystem().RegisterPlayerRoomChangeListner(this);
      ServiceLocator.GetEventHandlerSystem().RegisterObjectRoomChangeListner(this);

      //#if UNITY_PRO_LICENSE
      //  _roomFactory = new RoomFactoryAsync();
      //#else
      _roomFactory = new RoomFactory();
      //#endif

      _activeRoom = null;
      _allLoadedRooms = new Dictionary<string, RoomDefinition>();

      _currentlyInstancedRooms = new List<RoomDefinition>();
      _roomsInQueueToBeDeleted = new List<RoomDefinition>();

      WorldStateDefinition initState = ServiceLocator.GetSaveSystem().LoadInitialState();
      LoadNewWorldState(initState);

      ServiceLocator.GetAudioSystem().PlayMusic("kahvi315z1_lackluster-sina");
    }

    private void Update()
    {
      DeleteScheduledRooms();
    }

    #region Public - World State Loading and Saving
    /// <summary>
    /// Retrieves a definition of the current world state
    /// </summary>
    public WorldStateDefinition getCurrentWorldStateDefinition()
    {
      if(_activeRoom == null || _allLoadedRooms.Count == 0)
      {
        SMConsole.Log(tag: "[SCENE MANAGER]", type:SMLogType.ERROR, log: "Error: cannot get current world state. No world state is loaded. Returning null");
        return null;
      }
      List<RoomDefinition> roomsToSave = new List<RoomDefinition>();

      RoomDefinition tempUpdatedRoomDef;
      foreach (RoomDefinition roomDef in _allLoadedRooms.Values)
      {
        SMConsole.Log(tag: "[SCENE MANAGER]", log: "Saving Room state for " + roomDef.roomName);

        tempUpdatedRoomDef = _roomFactory.UpdateRoomDefinition(roomDef);
        _allLoadedRooms[roomDef.roomName] = tempUpdatedRoomDef != null ? tempUpdatedRoomDef : roomDef;
      }

      return new WorldStateDefinition(new List<RoomDefinition>(_allLoadedRooms.Values), _activeRoom.roomName);
    }

    /// <summary>
    /// Loads a new world state. Destroys previously loaded state
    /// </summary>
    public void LoadNewWorldState(WorldStateDefinition initState)
    {
      DeleteAndClearWorldState();

      foreach (RoomDefinition room in initState.roomsDefinedInState)
      {
        _allLoadedRooms.Add(room.roomName, room);
      }

      try
      {
        setActiveRoom(initState.startingRoom);
      }
      catch (SMSceneManagerSystemExceptionCantInstanceRoom exception)
      {
        SMConsole.Log(tag: "[SCENE MANAGER]", type: SMLogType.ERROR, log: exception.Message);
        throw exception;
      }
    }

    public void DeleteAndClearWorldState()
    {
      _activeRoom = null;
      SaveAndUninstanceRooms(_currentlyInstancedRooms);
      _allLoadedRooms.Clear();
    }
    

    // Defines the level of depth the manager goes through when instancing rooms
    public int roomInstancingDepthLimit
    {
      get
      {
        return _roomInstancingDepthLimit;
      }
      set
      {
        if (value < 1)
        {
          _roomInstancingDepthLimit = 1;
          SMConsole.Log(tag: "[SCENE MANAGER]", type: SMLogType.ERROR,
                        log: "Provided instancing depth" + value + " is not valid (must be greater than 1). roomInstancingDepth defaulted to 1.");
        }
        else
          _roomInstancingDepthLimit = value;
      }
    }
    #endregion

    #region Public - Event Listners
    public void ListenPlayerRoomChange(string newRoomName)
    {
      if (_activeRoom == null || _allLoadedRooms.Count == 0)
      {
        SMConsole.Log(tag: "[SCENE MANAGER]", type: SMLogType.ERROR, log: "Error: cannot change active room to " + newRoomName + ". No world state is loaded.");
        throw new SMSceneManagerSystemExceptionNoWorldStateLoaded("Error: cannot change active room to " + newRoomName + ". No world state is loaded.");
      }

      SMConsole.Log(tag: "[SCENE MANAGER]", log: "[SCENE MANAGER] Player changed room to " + newRoomName + " updating instanced room graph");
      if (newRoomName != _activeRoom.roomName)
      {
        try
        {
          setActiveRoom(newRoomName);
        }
        catch (SMSceneManagerSystemExceptionCantInstanceRoom exception)
        {
          SMConsole.Log(tag: "[SCENE MANAGER]", type: SMLogType.ERROR, log: exception.Message);
          throw exception;
        }
      }
    }

    public void ListenObjectRoomChange(string prevRoomName, string newRoomName, GameObject objectChangedRoom)
    {
      if (_activeRoom == null || _allLoadedRooms.Count == 0)
      {
        SMConsole.Log(tag: "[SCENE MANAGER]", type: SMLogType.ERROR, log: "Error: cannot change room of object " + objectChangedRoom.name + ". No world state is loaded.");
        throw new SMSceneManagerSystemExceptionNoWorldStateLoaded("Error: cannot change room of object " + objectChangedRoom.name + ". No world state is loaded.");
      }

      SMConsole.Log(tag: "[SCENE MANAGER]", log: "[SCENE MANAGER] Movable object" + objectChangedRoom.name + " room changed from " + prevRoomName + " to " + newRoomName + " updating object definition");
      if (newRoomName != _activeRoom.roomName)
      {
        _roomFactory.ChangeObjectRoom(_allLoadedRooms[prevRoomName], _allLoadedRooms[newRoomName], objectChangedRoom);
      }
    }
    #endregion

    #region Private - Room Creation and Deletion
    /// <summary>
    /// Defines in which room the player is currently at, and sets it as the origin of the room instancing graph.
    /// From there, all adjacent rooms are instanced, and in case some rooms have fallen outside the instancing depth, they are deleted safely
    /// </summary>
    private void setActiveRoom(string newActiveRoomName)
    {
      SMConsole.Log(tag: "[SCENE MANAGER]", log: "Setting room " + newActiveRoomName + " as active");

      RoomDefinition newActiveRoomDefinition;
      if (!_allLoadedRooms.TryGetValue(newActiveRoomName, out newActiveRoomDefinition))
      {
        throw new SMSceneManagerSystemExceptionCantInstanceRoom("Error: room " + newActiveRoomName + " does not exist in loaded world state.");
      }

      List<RoomDefinition> previouslyLoadedRooms = new List<RoomDefinition>(_currentlyInstancedRooms);
      _currentlyInstancedRooms.Clear();

      SMConsole.Log(tag: "[SCENE MANAGER]", log: "---------- Beggining instancing of room graph, root from " + newActiveRoomName + " -----------");
      InstanceRoomsFromRoot(newActiveRoomDefinition);
      SMConsole.Log(tag: "[SCENE MANAGER]", log: "---------- Finished Instancing room graph -----------");

      _activeRoom = newActiveRoomDefinition;

      //Compare newly instanced rooms to the previously loaded rooms, 
      //deletes any that exist in the previous version but not in the new.
      SaveAndUninstanceRooms(previouslyLoadedRooms, _currentlyInstancedRooms);
    }

    /// <summary>
    /// Safely saves and un-instances all rooms. Doesn't un-instance any room present in shouldIgnore
    /// </summary>
    private void SaveAndUninstanceRooms(List<RoomDefinition> roomsToDelete, List<RoomDefinition> shouldIgnore = null)
    {
      List<RoomDefinition> culledDeletionList = shouldIgnore != null ? roomsToDelete.FindAll(room => !shouldIgnore.Contains(room)) : roomsToDelete;

      foreach (RoomDefinition roomToDelete in culledDeletionList)
      {
        //Run updateRoomDefinition to update the definition according to the current world state of the room'
        RoomDefinition updatedDefinition = _roomFactory.UpdateRoomDefinition(roomToDelete);
        _allLoadedRooms[roomToDelete.roomName] = updatedDefinition;

        SMConsole.Log(tag: "[SCENE MANAGER]", log: "[SCENE MANAGER] Deleting room " + roomToDelete.roomName);

        if (!_roomFactory.IsRoomUnderConstruction(roomToDelete))
          _roomFactory.DestroyRoomInstance(roomToDelete);
        else
          _roomsInQueueToBeDeleted.Add(roomToDelete);
      }
    }

    /// <summary>
    /// Traverses the room graph instancing of all rooms until it hits the depth limit.
    /// The traversal is done breadth first, as we always want to instance the closest rooms to the player first, to avoid empty space
    /// </summary>
    private void InstanceRoomsFromRoot(RoomDefinition rootRoom, RoomDefinition parentRoom = null, int currentDepth = 0)
    {
      if (!_currentlyInstancedRooms.Contains(rootRoom))
      {
        SMConsole.Log(tag: "[SCENE MANAGER]", log: new string(' ', currentDepth * 2) + "Creating room " + rootRoom.roomName + "...");
        _roomFactory.CreateRoomInstance(rootRoom, parentRoom);
        _currentlyInstancedRooms.Add(rootRoom);
      }

      //If we haven't reached the nstancing depth limit, create all child rooms to current root
      if (currentDepth < _roomInstancingDepthLimit)
      {
        SMConsole.Log(tag: "[SCENE MANAGER]", log: new string(' ', currentDepth * 4) + "This room " + rootRoom.roomName + " has  " + rootRoom.gateways.Count + " connections.");

        List<RoomDefinition> tempNewlyCreatedChildren = new List<RoomDefinition>();

        //Create all Child Rooms
        foreach (RoomObjectGatewayDefinition gateToChild in rootRoom.gateways)
        {
          if (_allLoadedRooms.ContainsKey(gateToChild.connectsToRoom))
          {
            if (!_currentlyInstancedRooms.Contains(_allLoadedRooms[gateToChild.connectsToRoom]))
            {
              RoomDefinition childRoom = _allLoadedRooms[gateToChild.connectsToRoom];
              SMConsole.Log(tag: "[SCENE MANAGER]", log: new string(' ', (currentDepth + 1) * 4) + "Creating room " + childRoom.roomName + "...");

              _roomFactory.CreateRoomInstance(childRoom, rootRoom);
              _currentlyInstancedRooms.Add(childRoom);
              tempNewlyCreatedChildren.Add(childRoom);
            }
            else
            {
              SMConsole.Log(tag: "[SCENE MANAGER]", log: new string(' ', (currentDepth + 1) * 4) + "Room " + gateToChild.connectsToRoom + " already exists, skipping");
            }
          }
          else
            throw new SMSceneManagerSystemExceptionCantInstanceRoom("Error: Room Instancing at Depth: " + (currentDepth + 1) + ", Room " + gateToChild.connectsToRoom + " does not exist in loaded world state.");
        }

        //Instances the grandchild rooms
        foreach (RoomDefinition newlyCreated in tempNewlyCreatedChildren)
          InstanceRoomsFromRoot(newlyCreated, rootRoom, currentDepth + 1);
      }
    }

    private void DeleteScheduledRooms()
    {
      List<RoomDefinition> deletedRooms = new List<RoomDefinition>();
      foreach (RoomDefinition roomDef in _roomsInQueueToBeDeleted)
      {
        if (!_roomFactory.IsRoomUnderConstruction(roomDef) && !_currentlyInstancedRooms.Contains(roomDef))
        {
          _roomFactory.DestroyRoomInstance(roomDef);
          deletedRooms.Add(roomDef);
        }
      }
      _roomsInQueueToBeDeleted.RemoveAll(room => deletedRooms.Contains(room));
    }
    #endregion
  }
}