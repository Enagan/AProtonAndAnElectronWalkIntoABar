//Made by: Pedro Engana
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// RoomFactoryInstancedObjectsRegistry is used by the RoomFactory to register currently instanced rooms, and their objects.
/// It also keeps a connection between the instanced objects and their originating definitions
/// This class serves as a container class for the complex dictionary seen as it's variable. It allows for easy manipulation of it.
/// </summary>
public class RoomFactoryRegistry
{
  // Complex data type, should be seen as:
  // Key: RoomDef -> Value: { RoomParentObject, { Key:ObjectDef, Value:ObjectInstance } }
  private Dictionary<RoomDefinition, KeyValuePair<GameObject, Dictionary<RoomObjectDefinition, GameObject>>> _instancedRegistry;

  public RoomFactoryRegistry()
  {
    _instancedRegistry = new Dictionary<RoomDefinition, KeyValuePair<GameObject, Dictionary<RoomObjectDefinition, GameObject>>>();
  }

  #region Register & Unregister
  public void RegisterRoom(RoomDefinition roomDef, GameObject roomParentGameObject)
  {
    if (_instancedRegistry.ContainsKey(roomDef))
      SMConsole.Log(tag: "[ROOM FACTORY REGISTRY]", type: SMLogType.ERROR, 
                    log:"Error: Room " + roomDef.roomName + " already registered in the registry. Perhaps a room is not being cleaned on deletion?");
    else
      _instancedRegistry.Add(roomDef,
                          new KeyValuePair<GameObject, Dictionary<RoomObjectDefinition, GameObject>>(
                            roomParentGameObject,
                            new Dictionary<RoomObjectDefinition, GameObject>()));
  }

  public void RegisterObjectInRoom(RoomDefinition roomDef, RoomObjectDefinition objectDefinition, GameObject gameObject)
  {
    if (IsRoomRegistered(roomDef))
      GetAllGameObjectsFromRoom(roomDef).Add(objectDefinition, gameObject);
    else
      SMConsole.Log(tag: "[ROOM FACTORY REGISTRY]", type: SMLogType.ERROR, 
                    log:"Error: Object addition to room " + roomDef.roomName + " failed. Room does not exist in registry");
  }

  public void UnregisterRoom(RoomDefinition roomDef)
  {
    if (IsRoomRegistered(roomDef))
      _instancedRegistry.Remove(roomDef);
    else
      SMConsole.Log(tag: "[ROOM FACTORY REGISTRY]", type: SMLogType.ERROR, 
                    log:"Error: Removal of room " + roomDef.roomName + " failed. Room does not exist in registry");
  }

  public void UnregisterObjectFromRoom(RoomDefinition roomDef, RoomObjectDefinition objectDef)
  {
    if (IsRoomRegistered(roomDef))
      _instancedRegistry[roomDef].Value.Remove(objectDef);
    else
      SMConsole.Log(tag: "[ROOM FACTORY REGISTRY]", type: SMLogType.ERROR, 
                    log:"Error: Removal of object " + objectDef.objectPrefabPath + " failed. Room does not exist in registry");
  }
  #endregion

  #region Fetch Registry
  /// <summary>
  /// Retrieves the instanced game object from an object definition inside a room
  /// </summary>
  public GameObject GetGameObjectInRoomFromDefinition(RoomDefinition objectInRoom, RoomObjectDefinition objectDefinition)
  {
    if (IsRoomRegistered(objectInRoom))
      return _instancedRegistry[objectInRoom].Value[objectDefinition];
    else
    {
      SMConsole.Log(tag: "[ROOM FACTORY REGISTRY]", type: SMLogType.ERROR,
                    log: "Error: Object retrieval in room " + objectInRoom.roomName + " failed. Room does not exist in registry");
      return null;
    }
  }

  /// <summary>
  /// Retrieves the Object definition of a given GameObject inside a room
  /// </summary>
  public RoomObjectDefinition GetObjectDefinitionInRoomFromGameObject(RoomDefinition roomDef, GameObject gameObject)
  {
    if (!IsRoomRegistered(roomDef))
    {
      SMConsole.Log(tag: "[ROOM FACTORY REGISTRY]", type: SMLogType.ERROR, 
                    log:"Error: Object Definition retrieval in room " + roomDef.roomName + " failed. Room does not exist in registry");
      return null;
    }
    else
    {
      // We traverse all Key-Value pairs in the room dictionary, and we try to find a value equal to the gameobject, so we can return the key.
      foreach (KeyValuePair<RoomObjectDefinition, GameObject> objectsInRoom in GetAllGameObjectsFromRoom(roomDef))
      {
        if (objectsInRoom.Value == gameObject)
        {
          return objectsInRoom.Key;
        }
      }

      SMConsole.Log(tag: "[ROOM FACTORY REGISTRY]", type: SMLogType.ERROR, 
                    log:"Error: Could not find object " + gameObject.name + " definition in provided room");
      return null;
    }
  }

  public Dictionary<RoomObjectDefinition, GameObject> GetAllGameObjectsFromRoom(RoomDefinition roomDef)
  {
    if (IsRoomRegistered(roomDef))
      return _instancedRegistry[roomDef].Value;
    else
      SMConsole.Log(tag: "[ROOM FACTORY REGISTRY]", type: SMLogType.ERROR, 
                    log:"Error: Object retrieval in room " + roomDef.roomName + " failed. Room does not exist in registry");
    return null;
  }

  public GameObject GetParentObjectForRoom(RoomDefinition roomDef)
  {
    if (IsRoomRegistered(roomDef))
      return _instancedRegistry[roomDef].Key;
    else
      SMConsole.Log(tag: "[ROOM FACTORY REGISTRY]", type: SMLogType.ERROR,
                    log: "Error: Object retrieval in room " + roomDef.roomName + " failed. Room does not exist in registry");
    return null;
  }
  #endregion

  public bool IsRoomRegistered(RoomDefinition room)
  {
    return _instancedRegistry.ContainsKey(room);
  }

}
