//Made By: Pedro Engana
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SMSceneManagerSystem
{
  /// <summary>
  /// Room Factory is a helper class for the Scene Mananger, it handles the low level details of instancing new rooms and correctly placing them in relation to one another
  /// It uses the RoomFactoryInstancedObjectsRegistry to keep track of all instanced rooms and their definitions
  /// </summary>
  public class RoomFactory
  {
    private const float COLLIDER_ACTIVATION_COOLDOWN = 0.2f;
    private const float RENDER_WAIT_TIME = 0.05f;

    protected RoomFactoryRegistry _instancedObjects;
    protected List<RoomDefinition> _roomsUnderConstruction;

    public RoomFactory()
    {
      _instancedObjects = new RoomFactoryRegistry();
      _roomsUnderConstruction = new List<RoomDefinition>();
    }

    #region [Public Methods] Room Instancing, Destruction and Definition Update
    /// <summary>
    /// Instances a new room. In case the room is already instanced, the function does nothing.
    /// If a 'from' room is provided, the new room will be placed connected to the 'from' room at their shared gateway. In case a gateway doesn't exist between the rooms, the function does nothing.
    /// </summary>
    public virtual void CreateRoomInstance(RoomDefinition newRoom, RoomDefinition fromRoom = null)
    {
      if (!_instancedObjects.IsRoomRegistered(newRoom))
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
      if (!_instancedObjects.IsRoomRegistered(roomToDeleteDef))
      {
        SMConsole.Log(tag: "[ROOM FACTORY]", log: "Deletion of room " + roomToDeleteDef.roomName + " failed. Room does not exist in registry");
        return;
      }
      else
      {
        GameObject toDestroy = _instancedObjects.GetParentObjectForRoom(roomToDeleteDef);
        _instancedObjects.UnregisterRoom(roomToDeleteDef);
        roomToDeleteDef.colliders.Clear();
        roomToDeleteDef.renderers.Clear();
        roomToDeleteDef.maxDepth = 0;
        GameObject.Destroy(toDestroy);
      }
    }

    public bool IsRoomUnderConstruction(RoomDefinition roomDef)
    {
      return _roomsUnderConstruction.Contains(roomDef);
    }

    /// <summary>
    /// Updates the definition of a specific room, applying the current state of all objects to their definition instance
    /// </summary>
    public virtual RoomDefinition UpdateRoomDefinition(RoomDefinition roomDef)
    {
      if (!_instancedObjects.IsRoomRegistered(roomDef))
      {
        SMConsole.Log(tag: "[ROOM FACTORY]", type: SMLogType.ERROR, log: "Error: Updating room " + roomDef.roomName + " failed. Room does not exist in registry");
        return null;
      }
      else
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
          // Objects sometimes have other parameters we need to save, so for that we use complex states.
          // We find each sub-object that declared a complex state, and we ask them to update those values in the definition, according to their current simulation state.
          List<ComplexStateDefinition> updatedComplexStates = new List<ComplexStateDefinition>();
          foreach (ComplexStateDefinition complexStateInObject in objectInRoomDef.complexStates)
          {
            Transform childObjectWithComplexState = objectInRoomGameObject.transform.Find(complexStateInObject.objectNameInHierarchy);
            if (childObjectWithComplexState)
            {
              IHasComplexState instanceContainingComplexState = (childObjectWithComplexState.GetComponent(complexStateInObject.GetComplexStateName()) as IHasComplexState);

              if (!(instanceContainingComplexState == null))
                updatedComplexStates.Add(instanceContainingComplexState.UpdateComplexStateDefinition(complexStateInObject));
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
    }

    /// <summary>
    /// When a movable object changes rooms, we need to update his definition as well as the room definition that object is tied too
    /// </summary>
    public void ChangeObjectRoom(RoomDefinition prevRoom, RoomDefinition newRoom, GameObject objectThatChangedRooms)
    {
      if (!_instancedObjects.IsRoomRegistered(prevRoom))
      {
        SMConsole.Log(tag: "[ROOM FACTORY]", type: SMLogType.ERROR, log: "Error: Updating objects parent room. Previous room" + prevRoom.roomName + " does not exist in registry");
        return;
      }
      if (!_instancedObjects.IsRoomRegistered(newRoom))
      {
        SMConsole.Log(tag: "[ROOM FACTORY]", type: SMLogType.ERROR, log: "Error: Updating objects parent room. New room" + prevRoom.roomName + " does not exist in registry");
        return;
      }
      if (objectThatChangedRooms == null)
      {
        SMConsole.Log(tag: "[ROOM FACTORY]", type: SMLogType.ERROR, log: "Error: Updating objects parent room. Object is null");
        return;
      }

      objectThatChangedRooms.transform.parent = _instancedObjects.GetParentObjectForRoom(newRoom).transform;

      RoomObjectDefinition objectDef = _instancedObjects.GetObjectDefinitionInRoomFromGameObject(prevRoom, objectThatChangedRooms);

      _instancedObjects.UnregisterObjectFromRoom(prevRoom, objectDef);
      _instancedObjects.RegisterObjectInRoom(newRoom, objectDef, objectThatChangedRooms);
    }


    #endregion

    #region [Private] Room Creation Auxiliary functions
    /// <summary>
    /// Creates room as the origin point room, centered at the world origin
    /// </summary>
    private void InstanceFirstRoom(RoomDefinition newRoomDef)
    {
      _roomsUnderConstruction.Add(newRoomDef);

      //Prepares the room parent object
      GameObject roomParentObject = new GameObject(newRoomDef.roomName);
      roomParentObject.SetActive(false);

      _instancedObjects.RegisterRoom(newRoomDef, roomParentObject);

      //Instances all objects present in the room definition 
      foreach (RoomObjectDefinition objectInRoom in newRoomDef.objectsInRoom)
      {
        GameObject instancedObject = InstanceObject(objectInRoom, roomParentObject.transform, Vector3.zero);
        FindCollidersAndRenderers(newRoomDef, instancedObject);

        _instancedObjects.RegisterObjectInRoom(newRoomDef, objectInRoom, instancedObject);
      }

      //Instances all gateways in the room definition
      foreach (RoomObjectGatewayDefinition gate in newRoomDef.gateways)
      {
        GameObject instancedGateway = InstanceObject(gate, roomParentObject.transform, Vector3.zero);
        instancedGateway.GetComponent<GatewayTriggerScript>().connectsTo = gate.connectsToRoom;

        _instancedObjects.RegisterObjectInRoom(newRoomDef, gate, instancedGateway);
      }

      roomParentObject.SetActiveRecursively(true);
      ActivateCollidersAndRenderers(newRoomDef);

      _roomsUnderConstruction.Remove(newRoomDef);
    }

    /// <summary>
    /// Creates new room adjacent to the "from" room. Rooms will be connected and oriented by their respective gateways.
    /// Execution will halt in case a connection does not exist.
    /// </summary>
    private IEnumerator CreateAdjancentRoom(RoomDefinition newRoomDef, RoomDefinition fromRoomDef)
    {
      _roomsUnderConstruction.Add(newRoomDef);

      RoomObjectGatewayDefinition fromGateDef;
      RoomObjectGatewayDefinition newRoomGateDef;

      //Retrives the gateways between rooms.
      if ((fromGateDef = fromRoomDef.GetDefinitionOfGatewayToRoom(newRoomDef)) == null)
      {
        SMConsole.Log(tag: "[ROOM FACTORY]", type: SMLogType.ERROR, log: "Error: Gateway between rooms " + fromRoomDef.roomName + " and " + newRoomDef.roomName + " not found");
        throw new SMSceneManagerSystemExceptionCantInstanceRoom("Error: Gateway between rooms " + fromRoomDef.roomName + " and " + newRoomDef.roomName + " not found");
      }
      if ((newRoomGateDef = newRoomDef.GetDefinitionOfGatewayToRoom(fromRoomDef)) == null)
      {
        SMConsole.Log(tag: "[ROOM FACTORY]", type: SMLogType.ERROR, log: "Error: Gateway between rooms " + newRoomDef.roomName + " and " + fromRoomDef.roomName + " not found");
        throw new SMSceneManagerSystemExceptionCantInstanceRoom("Error: Gateway between rooms " + newRoomDef.roomName + " and " + fromRoomDef.roomName + " not found");
      }

      while (IsRoomUnderConstruction(fromRoomDef))
      {
        // We can't connect this room to it's parent if it isn't constructed yet
        yield return new WaitForSeconds(0.5f);
      }

      //Creates the room parent object
      GameObject roomParentObject = new GameObject(newRoomDef.roomName);
      roomParentObject.SetActive(false);
      _instancedObjects.RegisterRoom(newRoomDef, roomParentObject);

      //Orients the parent object to the new room gateway, as their centers will coincide
      roomParentObject.transform.eulerAngles = newRoomGateDef.eulerAngles;

      //Instances all gateways in room definition, in a position relative to the newRoomGate (The local origin)
      foreach (RoomObjectGatewayDefinition gateDefinition in newRoomDef.gateways)
      {
        GameObject instancedGateway = InstanceObject(gateDefinition, roomParentObject.transform, newRoomGateDef.position);
        instancedGateway.GetComponent<GatewayTriggerScript>().connectsTo = gateDefinition.connectsToRoom;

        _instancedObjects.RegisterObjectInRoom(newRoomDef, gateDefinition, instancedGateway);
      }

      //Clear previous meshes colliders
      newRoomDef.colliders.Clear();

      //Instances all objects in room definition, in a position relative to the newRoomGate (The local origin)
      foreach (RoomObjectDefinition objectDefinition in newRoomDef.objectsInRoom)
      {
        GameObject instancedObject = InstanceObject(objectDefinition, roomParentObject.transform, newRoomGateDef.position);
        FindCollidersAndRenderers(newRoomDef, instancedObject);

        _instancedObjects.RegisterObjectInRoom(newRoomDef, objectDefinition, instancedObject);

        //We yield from object creation to allow other co-routines to run, this allows rooms with a smaller bject count to end up instanced first
        yield return new WaitForSeconds(0.05f);
      }

      //Retrive the "from" rooms' gate position and rotation, as these will be the starting parameters of the new room
      Vector3 fromGateWorldPosition = _instancedObjects.GetGameObjectInRoomFromDefinition(fromRoomDef, fromGateDef).transform.position;
      Vector3 fromGateWorldRotation = _instancedObjects.GetGameObjectInRoomFromDefinition(fromRoomDef, fromGateDef).transform.eulerAngles;

      //Positions and orients the parent object to match and connect with the from room gateway
      roomParentObject.transform.position = fromGateWorldPosition;
      roomParentObject.transform.eulerAngles = BPUtil.OppositeVector(fromGateWorldRotation);

      roomParentObject.SetActiveRecursively(true);

      float lastTime = Time.time;
      float accumulatedTime = 0;
      for (int i = newRoomDef.maxDepth; i >= 0; i--)
      {
        List<Collider> cols;
        if (!newRoomDef.colliders.TryGetValue(i, out cols))
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
      foreach (Renderer ren in newRoomDef.renderers)
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

      _roomsUnderConstruction.Remove(newRoomDef);
    }

    /// <summary>
    /// Instances an object from an object definition, assigning him a parent and positioning him relative to the relativeOrigin provided
    /// </summary>
    protected GameObject InstanceObject(RoomObjectDefinition objectDefinition, Transform parentTransform = null, Vector3 relativeOrigin = default(Vector3))
    {
      // Instances the prefab using the Resource System, which caches instances for use and re-use
      GameObject instancedObject = ServiceLocator.GetResourceSystem().InstanceOf(objectDefinition.prefabPathForInstancing, active: false);

      // Base properties
      instancedObject.transform.localPosition = BPUtil.WorldPositionInRelationTo(objectDefinition.position, relativeOrigin);
      instancedObject.transform.localScale = objectDefinition.scale;
      instancedObject.transform.localEulerAngles = objectDefinition.eulerAngles;

      instancedObject.transform.parent = parentTransform;

      // Complex States
      // Objects sometimes have other parameters we need to save and load, so for that we use complex states.
      // After fetching the complex states from the definition, we find their respective sub-object, in the object hierarchy, and we pass it the complex state definition for it to load the values into himself
      foreach (ComplexStateDefinition complexStateInObject in objectDefinition.complexStates)
      {
        string stateName = complexStateInObject.GetComplexStateName();
        Transform childObjectWithComplexState = instancedObject.transform.Find(complexStateInObject.objectNameInHierarchy);
        if (childObjectWithComplexState)
        {
          IHasComplexState instanceContainingComplexState = (childObjectWithComplexState.GetComponent(complexStateInObject.GetComplexStateName()) as IHasComplexState);
          instanceContainingComplexState.LoadComplexStateDefinition(complexStateInObject);
        }
        else
        {
          SMConsole.Log(tag: "[ROOM FACTORY]", type: SMLogType.ERROR,
                        log: "Error applying complex state to instanced object " + objectDefinition.prefabPathForInstancing + ": Complex state " + complexStateInObject.objectNameInHierarchy + " could not be found in object");
        }
      }
      return instancedObject;
    }

    #region Room Factory Utils

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

}
