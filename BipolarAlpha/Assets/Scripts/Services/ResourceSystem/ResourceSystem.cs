using UnityEngine;
using System.Collections.Generic;

public class ResourceSystem : MonoBehaviour
{
  // Limits the number of instances the Resource System should dinamically create
  private const uint INSTANCE_LIMIT = 10;

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
  public GameObject AddNewResource(string pathName)
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


  //-- Add new instance
  //- AddInstance(string pathName, GameObject prefabInstance)
  // Adds a new instance of an object to the respective (path name, instance list) pair in the dictionary
  #region Add Instance
  /// <summary>
  /// Adds a new prefabInstance to the list of instances belonging to pathName in the dictionary , if pathName already exists
  /// in the dictionary. Otherwise, it adds a new (path name, instance list) pair to the dictionary.
  /// </summary>
  public GameObject AddInstance(string pathName, GameObject prefabInstance) 
  {
    List<GameObject> instanceList = new List<GameObject>();

    if (_availableResources.ContainsKey(pathName))
    {
      // Obtains the resource (instance list) stored in the dictionary
      _availableResources.TryGetValue(pathName, out instanceList);
      // Adds the new instance to the list in the dictionary
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
  //- InactiveInstanceOf(string pathName)
  //- InactiveInstanceOf(string pathName, Vector3 position, Quaternion rotation)
  //- ActiveInstanceOf(string pathName)
  //- ActiveInstanceOf(string pathName, Vector3 position, Quaternion rotation)
  // Creates a new instance of the specified object
  #region Object Instantiation
  /// <summary>
  /// Creates a new (inactive) instance of a given object (located on pathName) and stores it in the dictionary.
  /// If the object does not yet exist in the dictionary, the object is added as a new resource.
  /// </summary>
  public GameObject InactiveInstanceOf(string pathName)
  {
    List<GameObject> instanceList;
    GameObject newInstance;

    if (_availableResources.ContainsKey(pathName)) 
    {
      // Obtains the resource (instance list) stored in the dictionary
      _availableResources.TryGetValue(pathName, out instanceList);
      GameObject prefabInstance = instanceList[0] as GameObject;

      // Creates a new instance of the requested object
      newInstance = GameObject.Instantiate(prefabInstance) as GameObject;

      // Adds the new instance to the list in the dictionary
      instanceList.Add(newInstance);
    }
    else 
    {
      // Adds the requested object to the dictionary as a new resource
      newInstance = AddNewResource(pathName);
    }

    newInstance.SetActive(false);

    return newInstance;
  }

  /// <summary>
  /// Creates a new (inactive) instance of a given object (located on pathName) in a given position and with a given rotation, 
  /// and stores it in the dictionary.
  /// </summary>
  public GameObject InactiveInstanceOf(string pathName, Vector3 position, Quaternion rotation) 
  {
    List<GameObject> instanceList;
    GameObject newInstance;

    if (_availableResources.ContainsKey(pathName))
    {
      // Obtains the resource (instance list) stored in the dictionary
      _availableResources.TryGetValue(pathName, out instanceList);
      GameObject prefabInstance = instanceList[0] as GameObject;

      // Creates a new instance of the requested object
      newInstance = GameObject.Instantiate(prefabInstance, position, rotation) as GameObject;

      // Adds the new instance to the list in the dictionary
      instanceList.Add(newInstance);
    }
    else
    {
      // Loads the requested object from the resources
      GameObject prefab = Resources.Load(pathName, typeof(GameObject)) as GameObject;

      // Creates a new instance of the requested object
      newInstance = GameObject.Instantiate(prefab, position, rotation) as GameObject;
      instanceList = new List<GameObject>();
      instanceList.Add(newInstance);

      // Adds a new (path name, instance list) pair to the dictionary (creates a new resource)
      _availableResources.Add(pathName, instanceList);
    }

    newInstance.SetActive(false);

    return newInstance;  
  }

  /// <summary>
  /// Creates a new (active) instance of a given object (located on pathName) and stores it in the dictionary.
  /// If the object does not yet exist in the dictionary, the object is added as a new resource.
  /// </summary>
  public GameObject ActiveInstanceOf(string pathName)
  {
    List<GameObject> instanceList;
    GameObject newInstance;

    if (_availableResources.ContainsKey(pathName))
    {
      // Obtains the resource (instance list) stored in the dictionary
      _availableResources.TryGetValue(pathName, out instanceList);
      GameObject prefabInstance = instanceList[0] as GameObject;

      // Creates a new instance of the requested object
      newInstance = GameObject.Instantiate(prefabInstance) as GameObject;

      // Adds the new instance to the list in the dictionary
      instanceList.Add(newInstance);
    }
    else
    {
      // Adds the requested object to the dictionary as a new resource
      newInstance = AddNewResource(pathName);
    }

    newInstance.SetActive(true);

    return newInstance;
  }

  /// <summary>
  /// Creates a new (active) instance of a given object (located on pathName) in a given position and with a given rotation, 
  /// and stores it in the dictionary.
  /// </summary>
  public GameObject ActiveInstanceOf(string pathName, Vector3 position, Quaternion rotation)
  {
    List<GameObject> instanceList;
    GameObject newInstance;

    if (_availableResources.ContainsKey(pathName))
    {
      // Obtains the resource (instance list) stored in the dictionary
      _availableResources.TryGetValue(pathName, out instanceList);
      GameObject prefabInstance = instanceList[0] as GameObject;

      // Creates a new instance of the requested object
      newInstance = GameObject.Instantiate(prefabInstance, position, rotation) as GameObject;

      // Adds the new instance to the list in the dictionary
      instanceList.Add(newInstance);
    }
    else
    {
      // Loads the requested object from the resources
      GameObject prefab = Resources.Load(pathName, typeof(GameObject)) as GameObject;

      // Creates a new instance of the requested object
      newInstance = GameObject.Instantiate(prefab, position, rotation) as GameObject;
      instanceList = new List<GameObject>();
      instanceList.Add(newInstance);

      // Adds a new (path name, instance list) pair to the dictionary (creates a new resource)
      _availableResources.Add(pathName, instanceList);
    }

    newInstance.SetActive(true);

    return newInstance;
  }
  #endregion


  //-- Deactivate object instances
  //- SetSingleInactive(string pathName, GameObject Instance)
  //- SetInactive(string pathName)
  //- SetAllInactive()
  #region Deactivate Object Instances
  /// <summary>
  /// Sets a single instance of a given object as inactive.
  /// </summary>
  public void SetSingleInactive(string pathName, GameObject instance)
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
  /// Sets every instance of a given object as inactive.
  /// </summary>
  public void SetInactive(string pathName)
  {
    List<GameObject> prefabList;

    // Obtains the resource (instance list) stored in the dictionary
    _availableResources.TryGetValue(pathName, out prefabList);

    foreach(GameObject prefab in prefabList)
    {
      // Deactivates an instance
      prefab.SetActive(false);
    }
  }

  /// <summary>
  /// Sets all the instances of each object as inactive.
  /// </summary>
  public void SetAllInactive()
  {
    foreach(List<GameObject> prefabList in _availableResources.Values)
    {
      foreach (GameObject prefab in prefabList) 
      {
        // Deactivates an instance
        prefab.SetActive(false);
      }
    }
  }
  #endregion


  //-- Activate object instances
  //- SetSingleActive(string pathName)
  //- SetActive(string pathName)
  //- SetAllActive()
  #region Activate Object Instances
  /// <summary>
  /// Sets a single instance of a given object as active, and returns that instance.
  /// If there isn't an available instance in the dictionary, it creates another.
  /// </summary>
  public GameObject SetSingleActive(string pathName)
  {
    List<GameObject> prefabList;

    // Obtains the resource (instance list) stored in the dictionary
    _availableResources.TryGetValue(pathName, out prefabList);

    foreach (GameObject prefab in prefabList)
    {
      if (!prefab.activeSelf) 
      {
        // Activates an instance
        prefab.SetActive(true);
        // Returns the activated instance
        return prefab;
      }
    }

    // Creates a new instance if there no available instances in the dictionary
    GameObject newInstance = GameObject.Instantiate(prefabList[0]) as GameObject;
    prefabList.Add(newInstance);
    newInstance.SetActive(true);

    return newInstance;
  }

  /// <summary>
  /// Sets every instance of a given object as active.
  /// </summary>
  public void SetActive(string pathName)
  {
    List<GameObject> prefabList;

    // Obtains the resource (instance list) stored in the dictionary
    _availableResources.TryGetValue(pathName, out prefabList);

    foreach (GameObject prefab in prefabList)
    {
      // Activates an instance
      prefab.SetActive(true);
    }
  }

  /// <summary>
  /// Sets all the instances of each object as active.
  /// </summary>
  public void SetAllActive()
  {
    foreach (List<GameObject> prefabList in _availableResources.Values)
    {
      foreach (GameObject prefab in prefabList)
      {
        // Activates an instance
        prefab.SetActive(true);
      }
    }
  }
  #endregion


  //-- Create new instances
  // Uses the function Update, from Unity's class MonoBehaviour, to create new object instances dinamically
  #region Instance Creating
  void Update()
  {
    GameObject newInstance;

    foreach(KeyValuePair<string, List<GameObject>> instancePair in _availableResources)
    {
      // Checks if the number of stored instances of an object has not exceeded the limit of instances Resource System can create
      if(instancePair.Value.Count < INSTANCE_LIMIT)
      {
        // Creates a new instance
        newInstance = GameObject.Instantiate(instancePair.Value[0]) as GameObject;

        // Adds the new instance to the list of instances of an object, stored in the dictionary
        instancePair.Value.Add(newInstance);
        newInstance.SetActive(false);
      }
    }
  }
  #endregion
}
