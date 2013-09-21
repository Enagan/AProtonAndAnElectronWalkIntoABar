using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class ResourceSystem : MonoBehaviour
{
  // Limits the number of instances the Resource System should dinamically create
  [SerializeField]
  private int INSTANCE_LIMIT = 10;

  // Dictionary to store existing instances of game objects
  private Dictionary<string, List<GameObject>> _availableResources = new Dictionary<string,List<GameObject>>();

  //-- Add new resource
  //- AddNewResource(string pathName)
  // Adds a new object to the dictionary
  #region Add Resource
  /// <summary>
  /// Adds a new resource to the dictionary. Given a path name to an existing object, it creates a new instance of that object
  /// and adds the instance to a new list. It then adds the pair, consisting of the path name and the list, to the dictionary.
  /// </summary>
  private GameObject AddNewResource(string pathName)
  {
    List<GameObject> newPrefabList = new List<GameObject>();

    // Checks if the resource to be added already exists in the dictionary
    if (_availableResources.ContainsKey(pathName)) 
    {
      _availableResources.TryGetValue(pathName, out newPrefabList);
      return newPrefabList[0];
    }

    // Loads the requested object from the resources
    GameObject prefab = Resources.Load(pathName, typeof(GameObject)) as GameObject;

    // Creates a new instance of the requested object
    GameObject prefabInstance = GameObject.Instantiate(prefab) as GameObject;
    newPrefabList.Add(prefabInstance);
    prefabInstance.SetActive(false);

    // Adds the pair of the object's path name and the list of instances, to the dictionary (creates a new resource)
    _availableResources.Add(pathName, newPrefabList);

    return prefabInstance;
  }
  #endregion


  //-- Reuse Resource
  //- ReuseResource(string pathName, GameObject prefabInstance)
  // Stores an instance of an object in the respective (path name, instance list) pair in the dictionary for later reuse
  #region Reuse Resource
  /// <summary>
  /// Stores a prefabInstance, for later reuse, to the list of instances belonging to pathName in the dictionary,
  /// if pathName already exists in the dictionary. Otherwise, it adds a new (path name, instance list) pair to the dictionary.
  /// </summary>
  private GameObject ReuseResource(string pathName, GameObject prefabInstance) 
  {
    List<GameObject> instanceList = new List<GameObject>();

    prefabInstance.SetActive(false);

    if (_availableResources.ContainsKey(pathName))
    {
      // Obtains the resource (instance list) stored in the dictionary
      _availableResources.TryGetValue(pathName, out instanceList);
      // Adds the instance to the list in the dictionary
      instanceList.Add(prefabInstance);
    }
    else
    {
      instanceList.Add(prefabInstance);
      // Adds a new (path name, instance list) pair to the dictionary (creates a new resource)
      _availableResources.Add(pathName, instanceList);
    }

    return prefabInstance;
  }
  #endregion


  //-- Instatiate object
  //- public InstanceOf(string pathName, 
  //                    (optional) Vector3 position, 
  //                    (optional) Quaternion rotation,
  //                    (optional) bool active) 
  // Activates an inactive instance, or creates a new instance if there are no inactive instances, of the specified object
  #region Object Instantiation
  /// <summary>
  /// Activates an inactive instance of the specified object and returns it. It can also return an inactive instance,
  /// if the caller speficies it. In the case that there are no inactive instances, a new instance is created.
  /// </summary>
  /// <param name="active">(Optional) Defines if the requested object should be visible, or not. 
  /// It is visible(active = true) by default, if the parameter is not specified.</param>
  public GameObject InstanceOf(string pathName, 
                              Vector3 position = default(Vector3), 
                              Quaternion rotation = default(Quaternion),
                              bool active = true) 
  {
    // Obtains an instance of the requested object
    GameObject prefabInstance = SetSingleActive(pathName, position, rotation);
    // Ensures the requested object is visible or not, according to the caller's request
    prefabInstance.SetActive(active);

    return prefabInstance;
  }
  #endregion


  //-- Object instances activation
  //- SetSingleActive(string pathName, Vector3 position, Quaternion rotation)
  //- SetSingleInactive(string pathName, GameObject instance)
  //- SetAllActive(bool active)
  #region Object Instances Activation
  /// <summary>
  /// Sets a single instance of a given object as active, and returns that instance.
  /// If there isn't an available instance in the dictionary, it creates another.
  /// </summary>
  private GameObject SetSingleActive(string pathName, 
                                  Vector3 position = default(Vector3), 
                                  Quaternion rotation = default(Quaternion))
  {
    List<GameObject> prefabList;

    // Confirms the dictionary contains the requested resource
    if (_availableResources.ContainsKey(pathName))
    {
      // Obtains the resource (instance list) stored in the dictionary
      _availableResources.TryGetValue(pathName, out prefabList);

      foreach (GameObject prefab in prefabList)
      {
        if (!prefab.activeSelf)
        {
          // Activates an instance
          prefab.SetActive(true);
          prefab.transform.position = position;
          prefab.transform.rotation = rotation;
          prefabList.Remove(prefab);
          // Returns the activated instance
          return prefab;
        }
      }
    }
    else
    {
      // In case the dictionary does not have the requested resource, it creates a new one
      AddNewResource(pathName);
    }

    // Creates a new instance if there no available instances in the dictionary
    GameObject prefabObj = Resources.Load(pathName, typeof(GameObject)) as GameObject;
    GameObject newInstance = GameObject.Instantiate(prefabObj, position, rotation) as GameObject;
    //prefabList.Add(newInstance); -- might not be required
    newInstance.SetActive(true);

    return newInstance;
  }

  /// <summary>
  /// Sets a single instance of a given object as inactive.
  /// </summary>
  private void SetSingleInactive(string pathName, GameObject instance)
  {
    List<GameObject> prefabList;

    // Obtains the resource (instance list) stored in the dictionary
    _availableResources.TryGetValue(pathName, out prefabList);

    foreach (GameObject prefab in prefabList)
    {
      if (instance.Equals(prefab) && prefab.activeSelf)
      {
        // Deactivates the instance
        prefab.SetActive(false);
        return;
      }
    }
  }

  /// <summary>
  /// Sets all the instances of each object as inactive (default) or active.
  /// </summary>
  private void SetAllActive(bool active = false)
  {
    foreach (List<GameObject> prefabList in _availableResources.Values)
    {
      foreach (GameObject prefab in prefabList)
      {
        // Activates an instance
        prefab.SetActive(active);
      }
    }
  }
  #endregion


  //-- Unity Monobehaviour Functions
  // Start to connect the resource system to the Service Locator
  // Update, from Unity's class MonoBehaviour, to create new object instances dinamically
  #region Instance Creating
  private void PopulatePrefabDictionary()
  {
    Object[] allPrefabs = Resources.LoadAll("Prefabs", typeof(GameObject));
    foreach (GameObject prefab in allPrefabs)
    {
      //Retrieve the specific prefab path and trims it for instancer usage
      string path = AssetDatabase.GetAssetPath(prefab);
      path = path.Replace(".prefab", "");
      path = path.Replace("Assets/Resources/", "");

      _availableResources.Add(path, new List<GameObject>());
    }
  }

  void Start()
  {
    ServiceLocator.ProvideResourceSystem(this);
    PopulatePrefabDictionary();
  }

  void Update()
  {
    GameObject prefab;
    GameObject newInstance;

    SetAllActive(false);

    foreach(KeyValuePair<string, List<GameObject>> instancePair in _availableResources)
    {
      // Checks if the number of stored instances of an object has not exceeded the limit of instances Resource System can create
      if(instancePair.Value.Count < INSTANCE_LIMIT)
      {
        // Creates a new instance
        prefab = Resources.Load(instancePair.Key, typeof(GameObject)) as GameObject;
        newInstance = GameObject.Instantiate(prefab) as GameObject;

        // Adds the new instance to the list of instances of an object, stored in the dictionary
        instancePair.Value.Add(newInstance);
        newInstance.SetActive(false);
      }
    }
  }
  #endregion
}
