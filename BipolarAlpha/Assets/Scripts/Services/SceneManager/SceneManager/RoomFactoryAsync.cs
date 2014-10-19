using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomFactoryAsync : RoomFactory {

  private const string ROOM_PARENT_OBJECT_PREFIX = "ParentObject";
  private Dictionary<RoomDefinition, Dictionary<string, GameObject>> _gatewayRegistry = new Dictionary<RoomDefinition, Dictionary<string, GameObject>>();
  private Dictionary<RoomDefinition, GameObject> _instancedRoomsRegistry = new Dictionary<RoomDefinition, GameObject>();

  private List<RoomDefinition> _roomsMidInstancing = new List<RoomDefinition>();

  public void CreateRoom(RoomDefinition newRoom, RoomDefinition fromRoom = null)
  {
    if (!_instancedObjects.RoomIsRegistered(newRoom))
    {
      if (fromRoom == null)
      {
        ServiceLocator.GetSceneManager().StartCoroutine(
        CreateFirstRoom(newRoom)
        );
      }
      else
      {
        ServiceLocator.GetSceneManager().StartCoroutine(
        CreateAdjancentRoom(newRoom, fromRoom)
        );
      }
    }
  }
	
  private IEnumerator CreateFirstRoom(RoomDefinition room)
  {
    if (_instancedRoomsRegistry.ContainsKey(room) || _roomsMidInstancing.Contains(room))
    {
      Debug.Log("[ROOM FACTORY ASYNC] Room already instanced, skipping");
    }
    else
    {
      _roomsMidInstancing.Add(room);

      string roomParentObjectName = ROOM_PARENT_OBJECT_PREFIX + room.roomName;
      BipolarConsole.AllLog(roomParentObjectName);
      GameObject player = GameObject.Find("Player");
      player.SetActive(false);
      AsyncOperation op = Application.LoadLevelAdditiveAsync(room.roomName);
      yield return op;
      player.SetActive(true);
      GameObject roomParentObject = GameObject.Find(roomParentObjectName);
      roomParentObject.transform.position = Vector3.zero;
      //roomParentObject.SetActiveRecursively(true);

      if (_gatewayRegistry.ContainsKey(room))
        _gatewayRegistry.Remove(room);

      _gatewayRegistry.Add(room, new Dictionary<string, GameObject>());

      foreach (GatewayTriggerScript gateway in BPUtil.GetComponentsInHierarchy<GatewayTriggerScript>(roomParentObject.transform))
      {
        _gatewayRegistry[room].Add(gateway.connectsTo, gateway.gameObject);
      }

      _instancedRoomsRegistry.Add(room, roomParentObject);

      //_instancedObjects.RegisterRoom(room, roomParentObject);

      ////Instances all gateways in the room definition
      //foreach (RoomObjectGatewayDefinition gate in room.gateways)
      //{
      //  GameObject instancedObject = InstanceObject(gate);

      //  _instancedObjects.RegisterObjectInRoom(room, gate, instancedObject);

      //  instancedObject.SetActive(false);
      //}

      room.constructionFinished = true;
      room.inConstruction = false;
    }
  }

  private IEnumerator CreateAdjancentRoom(RoomDefinition newRoom, RoomDefinition from)
  {
    if (_instancedRoomsRegistry.ContainsKey(newRoom) || _roomsMidInstancing.Contains(newRoom))
    {
      Debug.Log("[ROOM FACTORY ASYNC] Room already instanced, skipping");
    }
    else
    {
      _roomsMidInstancing.Add(newRoom);

      Debug.Log("CREATING ADJ ROOM IN ASYNC -" + newRoom.roomName);
      newRoom.inConstruction = true;

      RoomObjectGatewayDefinition fromGate;
      RoomObjectGatewayDefinition newRoomGate;

      //Retrives the gateways between rooms.
      //TODO Exceptioning
      if ((fromGate = from.GetGatewayTo(newRoom)) == null)
      {
        Debug.Log("Error: Gateway between rooms " + from.roomName + " and " + newRoom.roomName + " not found");
        yield break;
        //return;
      }
      if ((newRoomGate = newRoom.GetGatewayTo(from)) == null)
      {
        Debug.Log("Error: Gateway between rooms " + newRoom.roomName + " and " + from.roomName + " not found");
        yield break;
        //return;
      }

      AsyncOperation roomConstructionOperation = Application.LoadLevelAdditiveAsync(newRoom.roomName);
      yield return roomConstructionOperation; //wait for operation to finish

      //Finds the room parent object
      GameObject roomParentObject = GameObject.Find(ROOM_PARENT_OBJECT_PREFIX + newRoom.roomName);
      //_instancedObjects.RegisterRoom(newRoom, roomParentObject);

      if (_gatewayRegistry.ContainsKey(newRoom))
        _gatewayRegistry.Remove(newRoom);

      _gatewayRegistry.Add(newRoom, new Dictionary<string, GameObject>());

      foreach (GatewayTriggerScript gateway in BPUtil.GetComponentsInHierarchy<GatewayTriggerScript>(roomParentObject.transform))
      {
        _gatewayRegistry[newRoom].Add(gateway.connectsTo, gateway.gameObject);
      }

      _instancedRoomsRegistry.Add(newRoom, roomParentObject);

      //Instances all gateways in the room definition
      //foreach (RoomObjectGatewayDefinition gate in newRoom.gateways)
      //{
      //  GameObject instancedObject = InstanceObject(gate);
      //  _instancedObjects.RegisterObjectInRoom(newRoom, gate, instancedObject);
      //  instancedObject.SetActive(false);
      //}

      yield return from.constructionFinished;

      //Retrive the "from" rooms' gate position and rotation, as these will be the starting position of the new room
      Vector3 fromGateWorldPosition = _gatewayRegistry[from][newRoom.roomName].transform.position;
      Vector3 fromGateWorldRotation = _gatewayRegistry[from][newRoom.roomName].transform.eulerAngles;

      Vector3 newGateLocalPosition = _gatewayRegistry[newRoom][from.roomName].transform.localPosition;
     
      Vector3 newGateWorldRotation = _gatewayRegistry[newRoom][from.roomName].transform.eulerAngles;

      //Positions and orients the parent object to match and connect with the from room gateway
      roomParentObject.transform.eulerAngles = OppositeVector(fromGateWorldRotation) - newGateWorldRotation;

      Vector3 newGateWorldPosition = _gatewayRegistry[newRoom][from.roomName].transform.position;


      roomParentObject.transform.position = fromGateWorldPosition - newGateWorldPosition;

      //Vector3 fromGateRelativePositionToNewRoom = roomParentObject.transform.TransformPoint (newRoomGate.position);

      //roomParentObject.transform.position -= fromGateRelativePositionToNewRoom;

      

      newRoom.constructionFinished = true;
      newRoom.inConstruction = false;
    }
  }

  public RoomDefinition UpdateRoomDefinition(RoomDefinition roomDef)
  {
    return roomDef;
  }

  public void DestroyRoom(RoomDefinition roomDef)
  {
    if (_instancedRoomsRegistry.ContainsKey(roomDef))
    {
      string roomParentObjectName = ROOM_PARENT_OBJECT_PREFIX + roomDef.roomName;
      GameObject toDestroy = GameObject.Find(roomParentObjectName);
      _instancedRoomsRegistry.Remove(roomDef);
      roomDef.colliders.Clear();
      roomDef.renderers.Clear();
      roomDef.maxDepth = 0;
      GameObject.Destroy(toDestroy);
    }
    else
    {
      Debug.Log("Error: Deletion of room " + roomDef.roomName + " failed. Room does not exist in registry");
    }
  }
}
