using UnityEngine;
using System.Collections;

/// <summary>
/// THROWAWAY CLASS BEFORE RESOURCE MANAGER
/// </summary>
public class PrefabInstancer{

  /// <summary>
  /// Creates an instance of the given prefab filepath
  /// </summary>
  /// <param name="path"></param>
  /// <returns></returns>
    public static GameObject instanceOf(string path)
    {
        GameObject prefab_location = Resources.Load(path, typeof(GameObject)) as GameObject;
        GameObject instance = GameObject.Instantiate(prefab_location) as GameObject;
        return instance;
    }
}
