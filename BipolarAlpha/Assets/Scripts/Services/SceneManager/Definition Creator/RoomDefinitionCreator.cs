//Made By: Engana
#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// The Room definition creator creates and serializes to a file all the prefab objects present in the scene where it's placed
/// </summary>
public class RoomDefinitionCreator : MonoBehaviour 
{
  [SerializeField]
  private string _roomName = "";

  private List<GameObject> _objectsInRoom = new List<GameObject>();

	private void Start () 
  {
    _objectsInRoom = ReadScene();
    RoomDefinition roomDef = createRoomDefinition(_roomName, _objectsInRoom);
    XMLSerializer.Serialize<RoomDefinition>(roomDef, "Assets/Resources/Levels/" + roomDef.roomName + ".lvl");
	}

  /// <summary>
  /// Reads the current scene and returns all currently instanced parent objects.
  /// </summary>
  private List<GameObject> ReadScene()
  {
    List<GameObject> readObjects = new List<GameObject>();

    object[] allObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject));

    foreach (object thisObject in allObjects)
    {
      GameObject castObject = ((GameObject)thisObject);
      if (castObject.activeInHierarchy && castObject.transform.parent == null)
      {
        readObjects.Add(castObject);
      }
    }
    return readObjects;
  }

  /// <summary>
  /// Creates the room definition, complete with all its objects whih are instanceable prefabs
  /// </summary>
  private RoomDefinition createRoomDefinition(string roomName, List<GameObject> objectsInRoom)
  {
    RoomDefinition roomDef = new RoomDefinition(roomName);

    foreach (GameObject obj in objectsInRoom)
    {
      //Does not add non-prefab objects to the room definition, as these cannot be instanced in the game scene
      Object prefabParent = PrefabUtility.GetPrefabParent(obj);
      if (prefabParent == null)
      {
        continue;
      }

      //Retrieved the prefab path and trims it for instancer usage
      string path = AssetDatabase.GetAssetPath(prefabParent);
      path = path.Replace(".prefab", "");
      path = path.Replace("Assets/Resources/", "");

      //If the object is a gateway, we must retrieve it's connection from the gateway script
      if (obj.tag == "Gateway")
      {
        string gatewayConnection = obj.GetComponent<GatewayTriggerScript>().connectsTo;
        roomDef.AddGateway(new RoomObjectGatewayDefinition(gatewayConnection,
                                  path,
                                  obj.transform.position,
                                  obj.transform.localScale,
                                  obj.transform.eulerAngles));
      }
      else
      {
        roomDef.AddObject(new RoomObjectDefinition(path,
                                  obj.transform.position,
                                  obj.transform.localScale,
                                  obj.transform.eulerAngles));
      }
    }
    
    return roomDef;
  }

}

#endif
