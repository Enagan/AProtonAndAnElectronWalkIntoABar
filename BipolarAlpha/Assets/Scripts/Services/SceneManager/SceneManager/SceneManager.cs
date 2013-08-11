// Made by: Engana
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The Scene Manager class is responsible to instance the various rooms the game has, as well as their connections
/// By using it's RoomFactory, the scene manager dynamically loads and uloads rooms as the player walks through the world
/// Giving the illusion of an open world.
/// </summary>
public class SceneManager : MonoBehaviour 
{
  // Adjacent rooms instancing depth
  [SerializeField]
  private int _adjacentInstancingDepth = 1;

  private RoomDefinition _activeRoom;
  private List<RoomDefinition> _currentlyCreatedRooms = new List<RoomDefinition>();
  private Dictionary<string,RoomDefinition> _allRooms = new Dictionary<string,RoomDefinition>();

  private RoomFactory _roomFactory = new RoomFactory();

	void Start () 
  {
    //TEST FUNCTIONS BEFORE XML DETAILING EXISTING ROOMS
    //Loads room definitions from files.
    RoomDefinition firstRoom = XMLSerializer.Deserialize<RoomDefinition>("Assets/Levels/1stRoom.lvl");
    RoomDefinition secondRoom = XMLSerializer.Deserialize<RoomDefinition>("Assets/Levels/2ndRoom.lvl");
    RoomDefinition thirdRoom = XMLSerializer.Deserialize<RoomDefinition>("Assets/Levels/3rdRoom.lvl");
    RoomDefinition fourthRoom = XMLSerializer.Deserialize<RoomDefinition>("Assets/Levels/4thRoom.lvl");
    RoomDefinition fifthRoom = XMLSerializer.Deserialize<RoomDefinition>("Assets/Levels/5thRoom.lvl");

    _allRooms.Add(firstRoom.roomName, firstRoom);
    _allRooms.Add(secondRoom.roomName, secondRoom);
    _allRooms.Add(thirdRoom.roomName, thirdRoom);
    _allRooms.Add(fourthRoom.roomName, fourthRoom);
    _allRooms.Add(fifthRoom.roomName, fifthRoom);

    ServiceLocator.ProvideSceneMananger(this);

    //instance the first room
    setActiveRoom("1stRoom");
	}

  /// <summary>
  /// Receives a room name and orders it to be 
  /// instanced as well as it's in-range adjacent rooms.
  /// Deletes previous rooms that may have become out-of-bounds
  /// Sets the room as the currently active room
  /// </summary>
  private void setActiveRoom(string roomName)
  {
    RoomDefinition roomToCreate;
    //Search for room in the loaded rooms Dictionary
    if ((roomToCreate = _allRooms[roomName]) == null)
    {
      BipolarConsole.AllLog("Error: room " + roomName + " does not exist in knowledgebase.");
      //TODO should throw exception
      return;
    }

    //Backup previously loaded rooms and clear the list for the new rooms
    List<RoomDefinition> previousCreatedRooms = new List<RoomDefinition>(_currentlyCreatedRooms);
    _currentlyCreatedRooms.Clear();

    //Order room creation starting in roomToCreate
    CreateRoomTree(roomToCreate);
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
        _roomFactory.DestroyRoom(oldRoomDef);
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
      _roomFactory.CreateRoom(root);
      _currentlyCreatedRooms.Add(root);
    }
    else
    {
      _roomFactory.CreateRoom(root, parent);
      _currentlyCreatedRooms.Add(root);
    }

    if (currentDepth < _adjacentInstancingDepth)
    {
      foreach (RoomObjectGatewayDefinition gate in root.gateways)
      {
        CreateRoomTree(_allRooms[gate.connectedToRoom], currentDepth + 1, root);
      }
    }
  }

  //TODO Should be replaced by event system
  /// <summary>
  /// Notifies the scene manager class that a player 
  /// has entered a new room
  /// </summary>
  public void SignalPlayerEnterRoom(string roomName)
  {
    if (roomName != _activeRoom.roomName)
    {
      setActiveRoom(roomName);
    }
  }

}
