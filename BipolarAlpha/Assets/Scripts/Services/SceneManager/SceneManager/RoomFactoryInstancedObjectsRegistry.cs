//Made by: Engana
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// RoomFactoryInstancedObjectsRegistry is used by the RoomFactory to register already instanced rooms, and their objects.
/// As well as keeping a connection between the instanced objects and their originating definitions
/// </summary>
public class RoomFactoryInstancedObjectsRegistry
{
  // Complex data type, should be seen as:
  // <RoomDef, <RoomParentObject, <ObjectDef,ObjectInstance>>>
  private Dictionary<RoomDefinition, KeyValuePair<GameObject,Dictionary<RoomObjectDefinition, GameObject>>> _instancedRegistry =
    new Dictionary<RoomDefinition, KeyValuePair<GameObject, Dictionary<RoomObjectDefinition, GameObject>>>();

  /// <summary>
  /// Adds a new room to the registry
  /// </summary>
  public void RegisterRoom(RoomDefinition room, GameObject roomParentObject)
  {
    _instancedRegistry.Add(room,
                          new KeyValuePair<GameObject, Dictionary<RoomObjectDefinition, GameObject>>(
                            roomParentObject,
                            new Dictionary<RoomObjectDefinition, GameObject>()));
  }

  /// <summary>
  /// Adds an object to a registered room
  /// </summary>
  public void RegisterObjectInRoom(RoomDefinition roomDef ,RoomObjectDefinition objDef, GameObject obj)
  {
    if (RoomIsRegistered(roomDef))
    {
      _instancedRegistry[roomDef].Value.Add(objDef, obj);
    }
    else
    {
      BipolarConsole.AllLog("Error: Object addition to room " + roomDef.roomName + " failed. Room does not exist in registry");
    }
  }

  /// <summary>
  /// Retrieves the instanced game object for a given object definition in a given room
  /// </summary>
  public GameObject GetGameObjectFromDefinition(RoomDefinition roomDef, RoomObjectDefinition objDef)
  {
    if (RoomIsRegistered(roomDef))
    {
      return _instancedRegistry[roomDef].Value[objDef];
    }
    else
    {
      BipolarConsole.AllLog("Error: Object retrieval in room " + roomDef.roomName + " failed. Room does not exist in registry");
      return null;
    }
  }

  /// <summary>
  /// Checks if a room is registered
  /// </summary>
  public bool RoomIsRegistered(RoomDefinition room)
  {
    return _instancedRegistry.ContainsKey(room);
  }

  /// <summary>
  /// Get the parent object of a registered room
  /// </summary>
  /// <param name="roomDef"></param>
  /// <returns></returns>
  public GameObject getRoomParentObject(RoomDefinition roomDef)
  {
    return RoomIsRegistered(roomDef) ? _instancedRegistry[roomDef].Key : null;
  }

  /// <summary>
  /// Removes a room and all it's objects from the registry
  /// </summary>
  public void RemoveRoomFromRegistry(RoomDefinition roomDef)
  {
    if (RoomIsRegistered(roomDef))
    {
      _instancedRegistry.Remove(roomDef);
    }
    else
    {
      BipolarConsole.AllLog("Error: Removal of room " + roomDef.roomName + " failed. Room does not exist in registry");
    }
  }
}
