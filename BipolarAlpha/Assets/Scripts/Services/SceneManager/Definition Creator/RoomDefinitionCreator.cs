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
  public string _roomName = "";
  private int circuitSystemCounter = 0;

  private List<GameObject> _objectsInRoom = new List<GameObject>();

	private void Start () 
  {
    SerializeRoom();
	}

  public void SerializeRoom()
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
  /// Creates the room definition, complete with all its objects which are instanceable prefabs
  /// </summary>
  private RoomDefinition createRoomDefinition(string roomName, List<GameObject> objectsInRoom)
  {
    RoomDefinition roomDef = new RoomDefinition(roomName);
    List<GameObject> objectsExceptCircuits = new List<GameObject>();

    List<GameObject> createdCircuitSystems = new List<GameObject>();

    foreach (GameObject obj in objectsInRoom)
    {
      //Does not add non-prefab objects to the room definition, as these cannot be instanced in the game scene
      Object prefabParent = PrefabUtility.GetPrefabParent(obj);
      if (prefabParent == null)
      {
        continue;
      }

      //Doesn't consider objects that might have previously been inserted into a Circuit System
      if (obj.transform.parent != null && createdCircuitSystems.Contains(obj.transform.parent.gameObject))
      {
        continue;
      }

      //Retrieved the prefab path and trims it for instancer usage
      string path = AssetDatabase.GetAssetPath(prefabParent);
      path = path.Replace(".prefab", "");
      path = path.Replace("Assets/Resources/", "");

      //Checks if the current object has any circuits in its hierarchy
      if(BipolarUtilityFunctions.GetComponentsInHierarchy<Circuit>(obj.transform).Count > 0)
      {
        //Check if any of those is a generator. In case it is, we need to generate a circuit system prefab to keep the circuit connections
        List<CircuitGenerator> checkForGenerators = BipolarUtilityFunctions.GetComponentsInHierarchy<CircuitGenerator>(obj.transform);
        if (checkForGenerators.Count > 0)
        {
          //List of objects which are part of the current circuit system tree
          List<GameObject> circuitSystemObjects = new List<GameObject>();

          circuitSystemObjects = CircuitSystemNetworkBuilder(checkForGenerators[0]);

          //Creates an empty parent to be the prefab head
          GameObject circuitsParent = new GameObject(roomName + "CircuitSystem-" + circuitSystemCounter++);
          circuitsParent.transform.position = circuitSystemObjects[0].transform.position;

          //Adds all objects of discovered circuit tree as childs of the circuit parent.
          
          foreach(GameObject circuitObjects in circuitSystemObjects)
          {
            circuitObjects.transform.parent = circuitsParent.transform;
          }

          //Marks the circuit as having been created for future exclusion of its parts from this cycle
          createdCircuitSystems.Add(circuitsParent);
        
          //Creates the prefab
          PrefabUtility.CreatePrefab("Assets/Resources/Prefabs/CircuitSystems/"+circuitsParent.name+".prefab", circuitsParent);

          path = "Prefabs/CircuitSystems/" + circuitsParent.name;

          //Adds the object to the room definition
          roomDef = AddNewObjectDefinitionToRoom(roomDef, circuitsParent, path);
        }
        else
        {
          //In case there is no generator, we check if the circuits are disabled, because if they are, the object used might be just for scenery
          bool allInactive = true;
          foreach(Circuit c in BipolarUtilityFunctions.GetComponentsInHierarchy<Circuit>(obj.transform))
          {
            if(c.enabled)
            {
              allInactive = false;
            }
          }
          if(allInactive)
          {
            roomDef = AddNewObjectDefinitionToRoom(roomDef, obj, path);
          }

        }
      }
      else
      {
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
          roomDef = AddNewObjectDefinitionToRoom(roomDef,obj,path);
        }
      }

    }    
    return roomDef;
  }

  /// <summary>
  /// Adds a new game object (building its RoomObjectDefinition to a given room) with its prefab path being "path", returns the updated room definition
  /// </summary>
  private RoomDefinition AddNewObjectDefinitionToRoom(RoomDefinition room, GameObject obj, string path)
  {
    RoomObjectDefinition def = new RoomObjectDefinition(path,
                                    obj.transform.position,
                                    obj.transform.localScale,
                                    obj.transform.eulerAngles);

    ///Gets all instances of IHasComplexState Scripts in the given object's Hierarchy tree and generates their complex state definitions
    foreach (IHasComplexState hasComplexState in BipolarUtilityFunctions.GetComponentsInHierarchy<IHasComplexState>(obj.transform))
    {
      def.AddComplexState(hasComplexState.WriteComplexState());
    }
    room.AddObject(def);
    return room;
  }

  /// <summary>
  /// Traverses a circuit tree, registering all GameObjects part of that circuit system. Returns the list of all objects in that tree
  /// </summary>
  private List<GameObject> CircuitSystemNetworkBuilder(Circuit root, List<GameObject> registeredCircuits = null, Circuit rootParent = null)
  {
    if(registeredCircuits == null)
    {
      registeredCircuits = new List<GameObject>();
    }
    //List<GameObject> circuitSystem = new List<GameObject>();
    Transform currentTransform;

    for(currentTransform = root.transform; 
      currentTransform.parent != null; 
      currentTransform = currentTransform.parent){}

    GameObject circuitParent = currentTransform.gameObject;
    if (!registeredCircuits.Contains(circuitParent))
    {
      registeredCircuits.Add(circuitParent);
    }
    else
    {
      return registeredCircuits;
    }

    foreach(Circuit c in root.outputs)
    {
      registeredCircuits = CircuitSystemNetworkBuilder(c, registeredCircuits);
    }
    foreach (Circuit c in root.inputs)
    {
      registeredCircuits = CircuitSystemNetworkBuilder(c, registeredCircuits);
    }

    /*
    foreach (Circuit c in root.inputs)
    {
      if (rootParent == null || !c.Equals(rootParent))
      {
        foreach (GameObject obj in CircuitSystemNetworkBuilder(c))
        {
          if (!circuitSystem.Contains(obj))
          {
            circuitSystem.Add(obj);
          }
        }
      }
    }
     * */

    return registeredCircuits;
  }

}

#endif
