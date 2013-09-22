﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;

// Class that processes the saving and loading the state of the world
public class SaveSystem : MonoBehaviour
{
  /// <summary>
  /// Saves the received list of existing room states into .lvl files (in XML format), and saves
  /// the (received) name of the active room, the paths where the room definitions are being saved
  /// and the player's position and rotation into a SaveState class.
  /// Saves the SaveState class as a .lvl file, in XML format.
  /// </summary>
  public void Save(KeyValuePair<string, List<RoomDefinition>> rooms)
  {
    SaveState saveState = new SaveState();

    saveState.activeRoom = rooms.Key;

    List<string> paths = new List<string>();

    foreach(RoomDefinition room in rooms.Value)
    {
      XMLSerializer.Serialize<RoomDefinition>(room, "Assets/Resources/Levels/Saves/" + room.roomName + ".lvl");
      paths.Add("Assets/Resources/Levels/Saves/" + room.roomName + ".lvl");
    }

    saveState.roomPaths = paths;

    Transform player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().transform;

    saveState.playerPosition = player.position;
    saveState.playerRotation = player.eulerAngles;

    XMLSerializer.Serialize<SaveState>(saveState, "Assets/Resources/Levels/Saves/SaveState.lvl");
  }

  /// <summary>
  /// Retrieves the last saved world state from an existing .lvl file.  
  /// Resets the player's position and rotation and returns what the active room was and the list of room definitions.
  /// </summary>
  /// <returns></returns>
  private KeyValuePair<string, List<RoomDefinition>> Load(string saveStatePath)
  {
    SaveState saveState = XMLSerializer.Deserialize<SaveState>(saveStatePath);

    if (saveState == null)
    {
      throw new BipolarExceptionSaveStateNotFound("No Save State was found");
    }

    List<RoomDefinition> loadedRooms = new List<RoomDefinition>();

    RoomDefinition savedRoom;

    foreach (string path in saveState.roomPaths)
    {
      savedRoom = XMLSerializer.Deserialize<RoomDefinition>(path);
      loadedRooms.Add(savedRoom);
    }

    Transform player = GameObject.FindGameObjectWithTag("Player").transform;
    player.position = saveState.playerPosition + new Vector3(0, 0.1f, 0);
    player.eulerAngles = saveState.playerRotation;

    return new KeyValuePair<string, List<RoomDefinition>>(saveState.activeRoom, loadedRooms);
  }

  public KeyValuePair<string, List<RoomDefinition>> LoadSaveState()
  {
    return Load("Assets/Resources/Levels/Saves/SaveState.lvl");
  }

  public KeyValuePair<string, List<RoomDefinition>> LoadInitialState()
  {
    return Load("Assets/Resources/Levels/SaveState.lvl");
  }

  void Start()
  {
    ServiceLocator.ProvideSaveSystem(this);
  }

  void Update()
  {
    if (Input.GetKey(KeyCode.F)) { ServiceLocator.GetSceneManager().SaveRooms(); }
    if (Input.GetKey(KeyCode.R)) { ServiceLocator.GetSceneManager().LoadRooms(); }
  }
}
