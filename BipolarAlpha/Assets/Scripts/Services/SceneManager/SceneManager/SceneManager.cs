// Made by: Engana
#define LOG_SCENE_MANAGER

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The Scene Manager class is responsible to instance the various rooms the game has, as well as their connections
/// By using it's RoomFactory, the scene manager dynamically loads and unloads rooms as the player walks through the world
/// Giving the illusion of an open world.
/// </summary>
public class SceneManager : MonoBehaviour , IPlayerRoomChangeListner, IObjectRoomChangeListner
{
  // Adjacent rooms instancing depth
  [SerializeField]
  private int _adjacentInstancingDepth = 5;

  private RoomDefinition _activeRoom;
  private List<RoomDefinition> _currentlyCreatedRooms = new List<RoomDefinition>();
  private Dictionary<string,RoomDefinition> _allRooms = new Dictionary<string,RoomDefinition>();
  private List<RoomDefinition> _roomInQueueToDeletion = new List<RoomDefinition>();

/*#if UNITY_PRO_LICENSE
  private RoomFactoryAsync _roomFactory = new RoomFactoryAsync();
#else
  private RoomFactory _roomFactory = new RoomFactory();
#endif */

  private RoomFactory _roomFactory = new RoomFactory();

	private void Start () 
  {
    ServiceLocator.ProvideSceneMananger(this);
    ServiceLocator.GetEventHandlerSystem().RegisterPlayerRoomChangeListner(this);
    ServiceLocator.GetEventHandlerSystem().RegisterObjectRoomChangeListner(this);

    //TODO@Engana Ask Save Manager
    //Loads room definitions from files.
   /* RoomDefinition firstRoom = XMLSerializer.Deserialize<RoomDefinition>("Assets/Levels/1stRoom.lvl");
    RoomDefinition furnaceRoom = XMLSerializer.Deserialize<RoomDefinition>("Assets/Levels/FurnaceRoom.lvl");
    RoomDefinition testingFacilityHub = XMLSerializer.Deserialize<RoomDefinition>("Assets/Levels/TestingFacilityHub.lvl");
    RoomDefinition secondRoom = XMLSerializer.Deserialize<RoomDefinition>("Assets/Levels/2ndRoom.lvl");
    RoomDefinition thirdRoom = XMLSerializer.Deserialize<RoomDefinition>("Assets/Levels/3rdRoom.lvl");
    RoomDefinition fourthRoom = XMLSerializer.Deserialize<RoomDefinition>("Assets/Levels/4thRoom.lvl");
    RoomDefinition fifthRoom = XMLSerializer.Deserialize<RoomDefinition>("Assets/Levels/5thRoom.lvl");

    _allRooms.Add(firstRoom.roomName, firstRoom);
    _allRooms.Add(secondRoom.roomName, secondRoom);
    _allRooms.Add(thirdRoom.roomName, thirdRoom);
    _allRooms.Add(fourthRoom.roomName, fourthRoom);
    _allRooms.Add(fifthRoom.roomName, fifthRoom);
    _allRooms.Add(furnaceRoom.roomName, furnaceRoom);
    _allRooms.Add(testingFacilityHub.roomName, testingFacilityHub);
    */
    KeyValuePair<string,List<RoomDefinition>> _initState = ServiceLocator.GetSaveSystem().LoadInitialState();
    
    foreach (RoomDefinition room in _initState.Value)
    {
      _allRooms.Add(room.roomName, room);
    }

    //instance the first room, temporary
    setActiveRoom(_initState.Key);

    ServiceLocator.GetAudioSystem().PlayMusic("kahvi315z1_lackluster-sina");
	}

  private void Update()
  {
    RoomDefinition roomToDelete = null;
    foreach(RoomDefinition roomDef in _roomInQueueToDeletion)
    {
      if (!roomDef.inConstruction)
      {
        roomToDelete = roomDef;
      }
    }

    if (roomToDelete != null)
    {
      _roomInQueueToDeletion.Remove(roomToDelete);
      _roomFactory.DestroyRoom(roomToDelete);
    }

  }

  #region Room Creation and Deletion
  /// <summary>
  /// Receives a room name and orders it to be 
  /// instanced as well as it's in-range adjacent rooms.
  /// Deletes previous rooms that may have become out-of-bounds
  /// Sets the room as the currently active room
  /// </summary>
  private void setActiveRoom(string roomName)
  {
#if LOG_SCENE_MANAGER
    Debug.Log("[SCENE MANAGER] Setting room " + roomName + " as active");
#endif
    RoomDefinition roomToCreate;
    //Search for room in the loaded rooms Dictionary
    Debug.Log (roomName);
    if ((roomToCreate = _allRooms[roomName]) == null)
    {
      Debug.Log("Error: room " + roomName + " does not exist in knowledgebase.");
      //TODO should throw exception
      return;
    }

    //Backup previously loaded rooms and clear the list for the new rooms
    List<RoomDefinition> previousCreatedRooms = new List<RoomDefinition>(_currentlyCreatedRooms);
    _currentlyCreatedRooms.Clear();

    //Order room creation starting in roomToCreate
#if LOG_SCENE_MANAGER
    Debug.Log("[SCENE MANAGER]------- Beggining new room instancing tree ---------");
#endif

    CreateRoomTree(roomToCreate);

#if LOG_SCENE_MANAGER
    Debug.Log("[SCENE MANAGER]---------- Room tree finished instancing -----------");
#endif

    //Set room as active
    _activeRoom = roomToCreate;

    //Compare newly instanced rooms to the previous, 
    //deletes any that exists in the previous version but not in the new.
    DeleteOutOfRangeRooms(previousCreatedRooms, _currentlyCreatedRooms);
  }

  /// <summary>
  /// Compares two room lists, the old and the new
  /// Orders deletion on all rooms that exist in the old list but not the new
  /// </summary>
  private void DeleteOutOfRangeRooms(List<RoomDefinition> previousRooms, List<RoomDefinition> newRooms)
  {
    bool foundMatch;
    foreach (RoomDefinition oldRoomDef in previousRooms)
    {
      foundMatch = false;
      foreach (RoomDefinition newRoomDef in newRooms)
      {
        if (oldRoomDef.Equals(newRoomDef))
        {
          foundMatch = true;
        }
      }
      if (!foundMatch)
      {
        //Run updateRoomDefinition to receive a RoomDefinition updated to the rooms'
        //state right before deletion
        RoomDefinition updatedDef = _roomFactory.UpdateRoomDefinition(oldRoomDef);
        _allRooms[updatedDef.roomName] = updatedDef;

#if LOG_SCENE_MANAGER
        Debug.Log("[SCENE MANAGER] Deleting out of range room " + oldRoomDef.roomName);
#endif

        //Destroy the room if it's not in construction
        if (oldRoomDef.inConstruction)
        {
          _roomInQueueToDeletion.Add(oldRoomDef);
        }
        else
        {
          _roomFactory.DestroyRoom(oldRoomDef);
        }
      }
    }
  }

  /// <summary>
  /// Iterative function, starts at a root room, and orders it to be instanced,
  /// does a limited depth-first search on the room tree and instances all rooms
  /// under the Adjacent Instancing Depth (class variable)
  /// </summary>
  private void CreateRoomTree(RoomDefinition root, int currentDepth = 0, RoomDefinition parent = null)
  {

    //If a parent is not defined, room should be instanced by itself
    if (parent == null)
    {
#if LOG_SCENE_MANAGER
      Debug.Log("[SCENE MANAGER]   Creating ROOT room " + root.roomName);
#endif
      _roomFactory.CreateRoom(root);
      _currentlyCreatedRooms.Add(root);
    }
    //Otherwise instance room based in the previous "parent" room
    else
    {
          
#if LOG_SCENE_MANAGER
      Debug.Log("[SCENE MANAGER]   Creating ADJACENT room " + root.roomName + " adjacent to " + parent.roomName);
#endif
      _roomInQueueToDeletion.Remove(root);
      _roomFactory.CreateRoom(root, parent);
      _currentlyCreatedRooms.Add(root);
    }
    //If we haven't reached the current depth, create all adjacent rooms to current root
    if (currentDepth < _adjacentInstancingDepth)
    {
      foreach (RoomObjectGatewayDefinition gate in root.gateways)
      {
        if (_allRooms.ContainsKey(gate.connectedToRoom))
        {
          Debug.Log ("creating room" + gate.connectedToRoom);
          CreateRoomTree(_allRooms[gate.connectedToRoom], currentDepth + 1, root);
        }
        else
        {
          Debug.Log("Room " + gate.connectedToRoom + " not found in Save State Definition");
        }
      }
    }
  }
  #endregion

  #region Room State Saving and Loading
  /// <summary>
  /// Sends a pair consisting of the currently active room and the list of room definitions to
  /// the Save System, to record the current world state.
  /// </summary>
  public void SaveRooms()
  {
    List<RoomDefinition> roomsToSave = new List<RoomDefinition>();
    RoomDefinition roomToSave;
    foreach(RoomDefinition room in _allRooms.Values)
    {
#if LOG_SCENE_MANAGER
      Debug.Log("[SCENE MANAGER] Saving Room state for " + room.roomName );
#endif
      roomToSave = _roomFactory.UpdateRoomDefinition(room);
      if(roomToSave == null)
      {
        roomsToSave.Add(room);
      }
      else
      {
        roomsToSave.Add(roomToSave);
      }
    }

    ServiceLocator.GetSaveSystem().Save(new KeyValuePair<string, List<RoomDefinition>>(_activeRoom.roomName, roomsToSave));
  }

  /// <summary>
  /// Requests the SaveSystem to retrieve the last saved world state.
  /// </summary>
  public void LoadRooms()
  {
    KeyValuePair<string, List<RoomDefinition>> loadedState = ServiceLocator.GetSaveSystem().LoadSaveState();

    List<RoomDefinition> oldRooms = new List<RoomDefinition>();
    foreach(RoomDefinition room in loadedState.Value)
    {
#if LOG_SCENE_MANAGER
      Debug.Log("[SCENE MANAGER] Loading Room state for " + room.roomName);
#endif
      oldRooms.Add(_allRooms[room.roomName]);
      _allRooms[room.roomName] = room;
    }

    setActiveRoom(loadedState.Key);

    foreach(RoomDefinition oldRoom in oldRooms)
    {
      if(oldRoom != null)
      {
        _roomFactory.DestroyRoom(oldRoom);
      }
    }
  }
  #endregion

  #region Event Listners
  /// <summary>
  /// Interface Listner implementation
  /// Receives the event when the player changes rooms, with the name
  /// of the new room
  /// </summary>
  public void ListenPlayerRoomChange(string newRoomName)
  {
#if LOG_SCENE_MANAGER
    Debug.Log("[SCENE MANAGER] Player room changed to " + newRoomName + " updating instanced room tree");
#endif
    if (newRoomName != _activeRoom.roomName)
    {
      setActiveRoom(newRoomName);
    }
  }

  /// <summary>
  /// Interface Listner implementation
  /// Receives the event when an object changes rooms, with the name
  /// of the new room, and the object itself
  /// </summary>
  public void ListenObjectRoomChange(string  prevRoomName, string newRoomName, GameObject objectChangedRoom)
  {
#if LOG_SCENE_MANAGER
    Debug.Log("[SCENE MANAGER] Movable object" + objectChangedRoom.name + " room changed from " + prevRoomName + " to " + newRoomName + " updating object definition");
#endif
    if (newRoomName != _activeRoom.roomName)
    {
      _roomFactory.ChangeObjectRoom(_allRooms[prevRoomName], _allRooms[newRoomName], objectChangedRoom);
    }
  }
  #endregion
}
