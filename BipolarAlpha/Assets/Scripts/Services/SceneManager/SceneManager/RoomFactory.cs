//Made By: Engana
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Room Factory is a helper class for the Scene Mananger, it handles the low level details of instancing new rooms and correctly placing them in relation to one another
/// It uses the RoomFactoryInstancedObjectsRegistry to keep track of all instanced rooms and their definitions
/// </summary>
public class RoomFactory
{
  protected RoomFactoryInstancedObjectsRegistry _instancedObjects = new RoomFactoryInstancedObjectsRegistry();

  private const float COLLIDER_ACTIVATION_COOLDOWN = 0.2f;
  private const float RENDER_WAIT_TIME = 0.05f;

  #region [Public Methods] Room Instancing, Destruction and Definition Update
  /// <summary>
  /// Instances a new room. In case the room is already instanced, the function does nothing.
  /// If a 'from' room is provided, the new room will be placed connected to the 'from' room at their shared gateway. In case a gateway doesn't exist between the rooms, the function does nothing.
  /// </summary>
  public virtual void CreateRoomInstance(RoomDefinition newRoom, RoomDefinition fromRoom = null)
  {
    if (!_instancedObjects.RoomIsRegistered(newRoom))
    {
      if (fromRoom == null)
        // First room cannot run in a co-routine as all following rooms depend on it being created
        InstanceFirstRoom(newRoom);
      else
        // A Co-routine is launched for adjacent rooms as many can be instanced simultaneously
        ServiceLocator.GetSceneManager().StartCoroutine(CreateAdjancentRoom(newRoom, fromRoom));
    }
  }
  /// <summary>
  /// Destroys an already instanced room. In case the room in non-existant, nothing is done.
  /// </summary>
  public virtual void DestroyRoomInstance(RoomDefinition roomToDeleteDef)
  {
    if (_instancedObjects.RoomIsRegistered(roomToDeleteDef))
    {
      GameObject toDestroy = _instancedObjects.getRoomParentObject(roomToDeleteDef);
      _instancedObjects.RemoveRoomFromRegistry(roomToDeleteDef);
      roomToDeleteDef.colliders.Clear();
      roomToDeleteDef.renderers.Clear();
      roomToDeleteDef.maxDepth = 0;
      GameObject.Destroy(toDestroy);
    }
    else
    {
      SMConsole.Log(tag: "[ROOM FACTORY]", log: "Deletion of room " + roomToDeleteDef.roomName + " failed. Room does not exist in registry");
    }
  }

  /// <summary>
  /// Updates the definition of a specific room, applying the current state of all objects to their definition instance
  /// </summary>
  public virtual RoomDefinition UpdateRoomDefinition(RoomDefinition roomDef)
  {
    if (_instancedObjects.RoomIsRegistered(roomDef))
    {
      List<RoomObjectDefinition> updatedObjectDefinitions = new List<RoomObjectDefinition>();
      List<RoomObjectGatewayDefinition> updatedGatewayDefinitions = new List<RoomObjectGatewayDefinition>();

      // Update all Objects & Gateways
      foreach (KeyValuePair<RoomObjectDefinition, GameObject> objectInRoom in _instancedObjects.GetAllGameObjectsFromRoom(roomDef))
      {
        RoomObjectDefinition objectInRoomDef = objectInRoom.Key;
        GameObject objectInRoomGameObject = objectInRoom.Value;

        // Update Base object parameters
        objectInRoomDef.position = objectInRoomGameObject.transform.position;
        objectInRoomDef.scale = objectInRoomGameObject.transform.localScale;
        objectInRoomDef.eulerAngles = objectInRoomGameObject.transform.eulerAngles;

        // Update Complex States inside object
        List<ComplexState> updatedComplexStates = new List<ComplexState>();
        foreach (ComplexState complexStateInObject in objectInRoomDef.complexStates)
        {
          Transform childObjectWithComplexState = objectInRoomGameObject.transform.Find(complexStateInObject.objectNameInHierarchy);
          if (childObjectWithComplexState)
          {
            IHasComplexState instanceContainingComplexState = (childObjectWithComplexState.GetComponent(complexStateInObject.GetComplexStateName()) as IHasComplexState);

            if (!(instanceContainingComplexState == null))
              updatedComplexStates.Add(instanceContainingComplexState.UpdateComplexState(complexStateInObject));
            else
              SMConsole.Log(tag: "[ROOM FACTORY]", type: SMLogType.ERROR,
                            log: "Error: On room" + roomDef.roomName + ": Component with complex state " + complexStateInObject.objectNameInHierarchy + " could not be found in object " + childObjectWithComplexState);
          }
          else
          {
            SMConsole.Log(tag: "[ROOM FACTORY]", type: SMLogType.ERROR,
                          log: "Error on room " + roomDef.roomName + ": Complex state " + complexStateInObject.objectNameInHierarchy + " could not be found in hierarchy");
          }
          objectInRoomDef.complexStates = updatedComplexStates;
        }

        //Add updated definition to List
        if (!(objectInRoomDef is RoomObjectGatewayDefinition))
          updatedObjectDefinitions.Add(objectInRoomDef);
        else
          updatedGatewayDefinitions.Add(objectInRoomDef as RoomObjectGatewayDefinition);
      }

      //Update Room Def lists
      roomDef.objectsInRoom = updatedObjectDefinitions;
      roomDef.gateways = updatedGatewayDefinitions;

      return roomDef;
    }
    else
    {
      SMConsole.Log(tag: "[ROOM FACTORY]", type: SMLogType.ERROR, log: "Error: Updating room " + roomDef.roomName + " failed. Room does not exist in registry");
      return null;
    }
  }

  /// <summary>
  /// When a movable object changes rooms, we need to update his definition as well as the room definition that object is tied too
  /// </summary>
  public void ChangeObjectRoom(RoomDefinition prevRoom, RoomDefinition newRoom, GameObject objectThatChangedRooms)
  {
    objectThatChangedRooms.transform.parent = _instancedObjects.getRoomParentObject(newRoom).transform;

    RoomObjectDefinition objectDef = _instancedObjects.GetDefinitionFromGameObject(prevRoom, objectThatChangedRooms);

    _instancedObjects.UnregisterObjectFromRoom(prevRoom, objectDef);
    _instancedObjects.RegisterObjectInRoom(newRoom, objectDef, objectThatChangedRooms);
  }
  #endregion

  #region [Private] Room Creation Auxiliary functions
  /// <summary>
  /// Creates room as the origin point room, centered at the world origin
  /// </summary>
  private void InstanceFirstRoom(RoomDefinition room)
  {
    room.inConstruction = true;

    //Prepares the room parent object
    GameObject roomParentObject = new GameObject(room.roomName);
    roomParentObject.SetActive(false);

    _instancedObjects.RegisterRoom(room, roomParentObject);

    //Instances all objects present in the room definition 
    foreach (RoomObjectDefinition objectInRoom in room.objectsInRoom)
    {
      GameObject instancedObject = InstanceObject(objectInRoom, roomParentObject.transform, Vector3.zero);
      FindCollidersAndRenderers(room, instancedObject);

      _instancedObjects.RegisterObjectInRoom(room, objectInRoom, instancedObject);
    }

    //Instances all gateways in the room definition
    foreach (RoomObjectGatewayDefinition gate in room.gateways)
    {
      GameObject instancedGateway = InstanceObject(gate, roomParentObject.transform, Vector3.zero);
      instancedGateway.GetComponent<GatewayTriggerScript>().connectsTo = gate.connectedToRoom;

      _instancedObjects.RegisterObjectInRoom(room, gate, instancedGateway);
    }

    roomParentObject.SetActiveRecursively(true);
    ActivateCollidersAndRenderers(room);

    room.inConstruction = false;
  }

  /// <summary>
  /// Creates new room adjacent to the "from" room. Rooms will be connected and oriented by their respective gateways.
  /// Execution will halt in case a connection does not exist.
  /// </summary>
  private IEnumerator CreateAdjancentRoom(RoomDefinition newRoom, RoomDefinition fromRoom)
  {
    newRoom.inConstruction = true;

    RoomObjectGatewayDefinition fromGate;
    RoomObjectGatewayDefinition newRoomGate;

    //Retrives the gateways between rooms.
    if ((fromGate = fromRoom.GetGatewayTo(newRoom)) == null)
    {
      SMConsole.Log(tag: "[ROOM FACTORY]", type: SMLogType.ERROR, log: "Error: Gateway between rooms " + fromRoom.roomName + " and " + newRoom.roomName + " not found");
      throw new RoomFactoryExceptionCantInstanceRoomNoConnectionFound("Error: Gateway between rooms " + fromRoom.roomName + " and " + newRoom.roomName + " not found");
    }
    if ((newRoomGate = newRoom.GetGatewayTo(fromRoom)) == null)
    {
      SMConsole.Log(tag: "[ROOM FACTORY]", type: SMLogType.ERROR, log: "Error: Gateway between rooms " + newRoom.roomName + " and " + fromRoom.roomName + " not found");
      throw new RoomFactoryExceptionCantInstanceRoomNoConnectionFound("Error: Gateway between rooms " + newRoom.roomName + " and " + fromRoom.roomName + " not found");
    }

    while (fromRoom.inConstruction)
    {
      // If "fromRoom" is still in construction, wait a bit.
      yield return new WaitForSeconds(0.5f);
    }

    //Creates the room parent object
    GameObject roomParentObject = new GameObject(newRoom.roomName);
    roomParentObject.SetActive(false);
    _instancedObjects.RegisterRoom(newRoom, roomParentObject);

    //Orients the parent object to the new room gateway, as their centers will coincide
    roomParentObject.transform.eulerAngles = newRoomGate.eulerAngles;

    //Instances all gateways in room definition, in a position relative to the newRoomGate (The local origin)
    foreach (RoomObjectGatewayDefinition gateDefinition in newRoom.gateways)
    {
      GameObject instancedGateway = InstanceObject(gateDefinition, roomParentObject.transform, newRoomGate.position);
      instancedGateway.GetComponent<GatewayTriggerScript>().connectsTo = gateDefinition.connectedToRoom;

      _instancedObjects.RegisterObjectInRoom(newRoom, gateDefinition, instancedGateway);
    }

    //Clear previous meshes colliders
    newRoom.colliders.Clear();

    //Instances all objects in room definition, in a position relative to the newRoomGate (The local origin)
    foreach (RoomObjectDefinition objectDefinition in newRoom.objectsInRoom)
    {
      GameObject instancedObject = InstanceObject(objectDefinition, roomParentObject.transform, newRoomGate.position);
      FindCollidersAndRenderers(newRoom, instancedObject);

      _instancedObjects.RegisterObjectInRoom(newRoom, objectDefinition, instancedObject);

      //We yield from object creation to allow other co-routines to run, this allows rooms with a smaller bject count to end up instanced first
      yield return new WaitForSeconds(0.05f);
    }

    //Retrive the "from" rooms' gate position and rotation, as these will be the starting parameters of the new room
    Vector3 fromGateWorldPosition = _instancedObjects.GetGameObjectFromDefinition(fromRoom, fromGate).transform.position;
    Vector3 fromGateWorldRotation = _instancedObjects.GetGameObjectFromDefinition(fromRoom, fromGate).transform.eulerAngles;

    //Positions and orients the parent object to match and connect with the from room gateway
    roomParentObject.transform.position = fromGateWorldPosition;
    roomParentObject.transform.eulerAngles = OppositeVector(fromGateWorldRotation);

    roomParentObject.SetActiveRecursively(true);

    float lastTime = Time.time;
    float accumulatedTime = 0;
    for (int i = newRoom.maxDepth; i >= 0; i--)
    {
      List<Collider> cols;
      if (!newRoom.colliders.TryGetValue(i, out cols))
      {
        continue;
      }


      foreach (Collider col in cols)
      {
        col.enabled = true;
        if (accumulatedTime <= 0)
        {
          lastTime = Time.time;
          yield return new WaitForSeconds(COLLIDER_ACTIVATION_COOLDOWN);
          accumulatedTime = Time.time - lastTime;
        }
        else
        {
          accumulatedTime -= COLLIDER_ACTIVATION_COOLDOWN;
        }
      }
    }

    lastTime = Time.time;
    accumulatedTime = 0;
    foreach (Renderer ren in newRoom.renderers)
    {
      ren.enabled = true;
      if (accumulatedTime <= 0)
      {
        lastTime = Time.time;
        yield return new WaitForSeconds(RENDER_WAIT_TIME);
        accumulatedTime = Time.time - lastTime;

      }
      else
      {
        accumulatedTime -= RENDER_WAIT_TIME;
      }
    }

    newRoom.inConstruction = false;
  }

  /// <summary>
  /// Instances an object from an object definition, assigning him a parent and positioning him relative to the relativeOrigin provided
  /// </summary>
  protected GameObject InstanceObject(RoomObjectDefinition objectDefinition, Transform parentTransform = null, Vector3 relativeOrigin = default(Vector3))
  {
    // Instances the prefab using the Resource System, which caches instances for use and re-use
    GameObject instancedObject = ServiceLocator.GetResourceSystem().InstanceOf(objectDefinition.objectPrefabPath, active: false);

    // Base properties
    instancedObject.transform.localPosition = WorldPositionInRelationTo(objectDefinition.position, relativeOrigin);
    instancedObject.transform.localScale = objectDefinition.scale;
    instancedObject.transform.localEulerAngles = objectDefinition.eulerAngles;

    instancedObject.transform.parent = parentTransform;

    // Complex States
    foreach (ComplexState complexStateInObject in objectDefinition.complexStates)
    {
      string stateName = complexStateInObject.GetComplexStateName();
      Transform childObjectWithComplexState = instancedObject.transform.Find(complexStateInObject.objectNameInHierarchy);
      if (childObjectWithComplexState)
      {
        IHasComplexState instanceContainingComplexState = (childObjectWithComplexState.GetComponent(complexStateInObject.GetComplexStateName()) as IHasComplexState);
        instanceContainingComplexState.LoadComplexState(complexStateInObject);
      }
      else
      {
        SMConsole.Log(tag: "[ROOM FACTORY]", type: SMLogType.ERROR,
                      log: "Error applying complex state to instanced object " + objectDefinition.objectPrefabPath + ": Complex state " + complexStateInObject.objectNameInHierarchy + " could not be found in object");
      }
    }
    return instancedObject;
  }

  #region Room Factory Utils
  protected Vector3 WorldPositionInRelationTo(Vector3 originalObjectPosition, Vector3 newOrigin)
  {
    return (originalObjectPosition - newOrigin);
  }

  /// <summary>
  /// Returns the opposite vector in x and z, maintaining the "up vector" intact
  /// </summary>
  protected Vector3 OppositeVector(Vector3 vector)
  {
    return new Vector3(-vector.x, vector.y + 180, -vector.z);
  }

  /// <summary>
  /// Finds all colliders and renderers and saves them into the room instance
  /// </summary>
  private void FindCollidersAndRenderers(RoomDefinition room, GameObject obj, int depth = 0)
  {
    room.maxDepth = room.maxDepth < depth ? depth : room.maxDepth;
    Dictionary<int, List<Collider>> cols = room.colliders;

    List<Collider> colliders;

    if (!cols.TryGetValue(depth, out colliders))
    {
      colliders = new List<Collider>();
      cols.Add(depth, colliders);
    }

    MeshCollider collider = obj.GetComponent<MeshCollider>();
    if (collider && !collider.enabled)
    {
      colliders.Add(collider);
      collider.enabled = false;
    }

    Renderer renderer = obj.GetComponent<Renderer>();
    if (renderer && !renderer.enabled)
    {
      room.renderers.Add(renderer);
    }

    foreach (Transform trans in obj.transform)
    {
      FindCollidersAndRenderers(room, trans.gameObject, depth + 1);
    }

  }

  /// <summary>
  /// Activates all colliders and renderers
  /// </summary>
  private void ActivateCollidersAndRenderers(RoomDefinition room)
  {
    for (int i = room.maxDepth; i >= 0; i--)
    {
      List<Collider> cols;
      if (!room.colliders.TryGetValue(i, out cols))
      {
        continue;
      }
      cols.ForEach(col => col.enabled = true);
    }

    room.renderers.ForEach(renderer => renderer.enabled = true);
  }
  #endregion

  #endregion


}
