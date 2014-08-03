using UnityEngine;
using System.Collections;

public class RoomFactoryAsync : RoomFactory {

  private const string ROOM_PARENT_OBJECT_PREFIX = "ParentObject";
  protected RoomFactoryInstancedObjectsRegistry _instancedObjects = new RoomFactoryInstancedObjectsRegistry();

  public void CreateRoom(RoomDefinition newRoom, RoomDefinition fromRoom = null)
  {
    if (!_instancedObjects.RoomIsRegistered(newRoom))
    {
      if (fromRoom == null)
      {
        ServiceLocator.GetSceneManager().StartCoroutine(CreateFirstRoom(newRoom));
      }
      else
      {   
        ServiceLocator.GetSceneManager().StartCoroutine(CreateAdjancentRoom(newRoom, fromRoom));
      }
    }
  }
	
  private IEnumerator CreateFirstRoom(RoomDefinition room)
  {
    string roomParentObjectName = ROOM_PARENT_OBJECT_PREFIX + room.roomName;
    BipolarConsole.AllLog (roomParentObjectName);
    GameObject player = GameObject.Find ("Player");
    player.SetActive (false);
    AsyncOperation op = Application.LoadLevelAdditiveAsync (room.roomName + " - Copy");
    yield return op;
    player.SetActive (true);
    GameObject roomParentObject = GameObject.Find(roomParentObjectName);
    roomParentObject.transform.position = Vector3.zero;
    //roomParentObject.SetActiveRecursively(true);
    _instancedObjects.RegisterRoom(room, roomParentObject);

    //Instances all gateways in the room definition
    foreach (RoomObjectGatewayDefinition gate in room.gateways)
    {
      GameObject instancedObject = InstanceObject(gate);

      _instancedObjects.RegisterObjectInRoom(room, gate, instancedObject);

      instancedObject.SetActive(false);
    }
    
    room.constructionFinished = true;
    room.inConstruction = false;
  }

  private IEnumerator CreateAdjancentRoom(RoomDefinition newRoom, RoomDefinition from)
  {
    BipolarConsole.AllLog ("CREATING ADJ ROOM IN ASYNC");
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

    AsyncOperation roomConstructionOperation = Application.LoadLevelAdditiveAsync (newRoom.roomName + " - Copy");
    yield return roomConstructionOperation; //wait for operation to finish
 
    //Finds the room parent object
    GameObject roomParentObject = GameObject.Find(ROOM_PARENT_OBJECT_PREFIX + newRoom.roomName);
    _instancedObjects.RegisterRoom(newRoom, roomParentObject);

    //Instances all gateways in the room definition
    foreach (RoomObjectGatewayDefinition gate in newRoom.gateways)
    {
      GameObject instancedObject = InstanceObject(gate);
      
      _instancedObjects.RegisterObjectInRoom(newRoom, gate, instancedObject);
      
      instancedObject.SetActive(false);
    }
        
    //Retrive the "from" rooms' gate position and rotation, as these will be the starting position of the new room
    Vector3 fromGateWorldPosition = _instancedObjects.GetGameObjectFromDefinition(from, fromGate).transform.position;
    Vector3 fromGateWorldRotation = _instancedObjects.GetGameObjectFromDefinition(from, fromGate).transform.eulerAngles;

    //Positions and orients the parent object to match and connect with the from room gateway
    roomParentObject.transform.position = fromGateWorldPosition;

    Vector3 fromGateRelativePositionToNewRoom = roomParentObject.transform.TransformPoint (newRoomGate.position);

    roomParentObject.transform.position -= fromGateRelativePositionToNewRoom;

    roomParentObject.transform.eulerAngles = OppositeVector(fromGateWorldRotation);
   
    
    newRoom.constructionFinished = true;
    newRoom.inConstruction = false;
  }
}
