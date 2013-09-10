using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SaveSystem : MonoBehaviour
{
  public void Save(KeyValuePair<string, List<RoomDefinition>> rooms)
  {
    SaveState saveState = new SaveState();

    saveState.activeRoom = rooms.Key;

    List<string> paths = new List<string>();

    foreach(RoomDefinition room in rooms.Value)
    {
      XMLSerializer.Serialize<RoomDefinition>(room, "Assets/Levels/Saves/" + room.roomName + ".lvl");
      paths.Add("Assets/Levels/Saves/" + room.roomName + ".lvl");
    }

    saveState.roomPaths = paths;

    Transform player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().transform;

    saveState.playerPosition = player.position;
    saveState.playerRotation = player.eulerAngles;

    XMLSerializer.Serialize<SaveState>(saveState, "Assets/Levels/Saves/SaveState.lvl");
  }

  public KeyValuePair<string, List<RoomDefinition>> Load()
  {
    SaveState saveState = XMLSerializer.Deserialize<SaveState>("Assets/Levels/Saves/SaveState.lvl");

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
