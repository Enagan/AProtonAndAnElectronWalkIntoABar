//Made By: Engana
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Room Factory is a helper class for the Scene Mananger, 
/// it handles the low level details of instancing new rooms 
/// and correctly placing them in relation to one another
/// </summary>
public class RoomFactory
{
  private RoomFactoryInstancedObjectsRegistry _instancedObjects = new RoomFactoryInstancedObjectsRegistry();

  #region [Public Methods] Room Creation and Destruction
  /// <summary>
  /// Instances a new room. In case the room is already instanced, the function does nothing.
  /// If a "from" room is provided, the new room will be placed connected to the from room
  /// at their shared gateway. In case a gateway doesn't exist between the rooms, the function does nothing.
  /// </summary>
  public void CreateRoom(RoomDefinition newRoom, RoomDefinition fromRoom = null)
  {
    if (!_instancedObjects.RoomIsRegistered(newRoom))
    {
      if (fromRoom == null)
      {
        CreateFirstRoom(newRoom);
      }
      else
      {
        CreateAdjancentRoom(newRoom, fromRoom);
      }
    }
  }

  /// <summary>
  /// Destroys an already instanced room. In case the room in non-existant, an error is launched
  /// </summary>
  public void DestroyRoom(RoomDefinition room)
  {
    if (_instancedObjects.RoomIsRegistered(room))
    {
      GameObject toDestroy = _instancedObjects.getRoomParentObject(room);
      _instancedObjects.RemoveRoomFromRegistry(room);
      GameObject.Destroy(toDestroy);
    }
    else
    {
      BipolarConsole.AllLog("Error: Deletion of room " + room.roomName + " failed. Room does not exist in registry");
    }
  }
  #endregion

  #region [Private] Room Creation Auxiliary functions
  /// <summary>
  /// Creates room as the original room, centered at the world origin
  /// </summary>
  private void CreateFirstRoom(RoomDefinition room)
  {
    //Creates the room parent object
    GameObject roomParentObject = new GameObject(room.roomName);
    _instancedObjects.RegisterRoom(room, roomParentObject);

    //Instances all objects present in the room definition 
    foreach (RoomObjectDefinition obj in room.objectsInRoom)
    {
      GameObject instancedObject = InstanceObject(obj, roomParentObject.transform, Vector3.zero);

      _instancedObjects.RegisterObjectInRoom(room, obj, instancedObject);
    }

    //Instances all gateways in the room definition
    foreach (RoomObjectGatewayDefinition gate in room.gateways)
    {
      GameObject instancedObject = InstanceObject(gate, roomParentObject.transform, Vector3.zero);

      _instancedObjects.RegisterObjectInRoom(room, gate, instancedObject);
    }

  }

  /// <summary>
  /// Creates new room adjacent to the "from" room.
  /// Rooms will be connected via their respective gateways to each other.
  /// Execution will halt in case a connection does not exist.
  /// </summary>
  private void CreateAdjancentRoom(RoomDefinition newRoom, RoomDefinition from)
  {
    RoomObjectGatewayDefinition fromGate;
    RoomObjectGatewayDefinition newRoomGate;

    //Retrives the gateways between rooms.
    //TODO Exceptioning
    if ((fromGate = from.GetGatewayTo(newRoom)) == null)
    {
      BipolarConsole.AllLog("Error: Gateway between rooms " + from.roomName + " and " + newRoom.roomName + " not found");
      return;
    }
    if ((newRoomGate = newRoom.GetGatewayTo(from)) == null)
    {
      BipolarConsole.AllLog("Error: Gateway between rooms " + newRoom.roomName + " and " + from.roomName + " not found");
      return;
    }

    //Retrive the "from" rooms' gate position and rotation, as these will be the starting position of the new room
    Vector3 fromGateWorldPosition = _instancedObjects.GetGameObjectFromDefinition(from, fromGate).transform.position;
    Vector3 fromGateWorldRotation = _instancedObjects.GetGameObjectFromDefinition(from, fromGate).transform.eulerAngles;

    //Creates the room parent object
    GameObject roomParentObject = new GameObject(newRoom.roomName);
    _instancedObjects.RegisterRoom(newRoom, roomParentObject);

    //Orients the parent object to the new room gateway, as their centers will coincide
    roomParentObject.transform.eulerAngles = newRoomGate.eulerAngles;

    //Instances all objects in room definition, in a position relative 
    //to the newRoomGate (The local origin)
    foreach (RoomObjectDefinition obj in newRoom.objectsInRoom)
    {
      GameObject instancedObject = InstanceObject(obj, roomParentObject.transform, newRoomGate.position);

      _instancedObjects.RegisterObjectInRoom(newRoom, obj, instancedObject);
    }
    //Instances all gateways in room definition, in a position relative 
    //to the newRoomGate (The local origin)
    foreach (RoomObjectGatewayDefinition gate in newRoom.gateways)
    {
      GameObject instancedObject = InstanceObject(gate, roomParentObject.transform, newRoomGate.position);

      _instancedObjects.RegisterObjectInRoom(newRoom, gate, instancedObject);
    }

    //Positions and orients the parent object to match and connect with the from room gateway
    roomParentObject.transform.position = fromGateWorldPosition;
    roomParentObject.transform.eulerAngles = OppositeVector(fromGateWorldRotation);
  }

  /// <summary>
  /// Instances an object from an object definition, assigning him a parent and positioning 
  /// him relative to the relativeOrigin provided, in case these arguments are used
  /// </summary>
  private GameObject InstanceObject(RoomObjectDefinition obj, Transform parentTransform = null, Vector3 relativeOrigin = default(Vector3))
  {
    GameObject instancedObject = PrefabInstancer.instanceOf(obj.objectPrefabPath);

    instancedObject.transform.localPosition = WorldPositionInRelationTo(obj.position, relativeOrigin);
    instancedObject.transform.localScale = obj.scale;
    instancedObject.transform.localEulerAngles = obj.eulerAngles;

    instancedObject.transform.parent = parentTransform;

    return instancedObject;
  }

  /// <summary>
  /// Returns a position in relation of another position
  /// </summary>
  private Vector3 WorldPositionInRelationTo(Vector3 originalObjectPosition,
    Vector3 localRelationalObjectPositon)
  {
    return (originalObjectPosition - localRelationalObjectPositon);
  }

  /// <summary>
  /// Returns the opposite vector
  /// </summary>
  private Vector3 OppositeVector(Vector3 vec)
  {
    return new Vector3(-vec.x, vec.y + 180, -vec.z);
  }

  #endregion
}
