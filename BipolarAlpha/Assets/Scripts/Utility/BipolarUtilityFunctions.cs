using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BPUtil 
{
  /// <summary>
  /// Returns all Components of type T in the entire hierarchy going down from root
  /// </summary>
  public static List<T> GetComponentsInHierarchy<T>(Transform root) where T : class
  {
    List<T> extractedComponents = new List<T>();

    foreach (Component checkForExtraction in root.GetComponents<Component>())
    {
      if (checkForExtraction is T)
      {
        extractedComponents.Add(checkForExtraction as T);
      }
      //def.AddComplexState(hasComplexState.WriteComplexState());
    }

    foreach (Transform child in root)
    {
      extractedComponents.AddRange(GetComponentsInHierarchy<T>(child));
    }
    return extractedComponents;
  }

  public static List<GameObject> GetDirectChildren(GameObject parentObject)
  {
    List<GameObject> children = new List<GameObject>();

    foreach (Transform childTransform in parentObject.GetComponentsInChildren<Transform>())
    {
      if(childTransform.parent == parentObject.transform && childTransform.parent != null)
        children.Add(childTransform.gameObject);
    }

    return children;
  }

  public static List<GameObject> GetAllChildrenAtAnyDepth(GameObject parentObject)
  {
    List<GameObject> children = new List<GameObject>();

    foreach (Transform childTransform in GetComponentsInHierarchy<Transform>(parentObject.transform))
    {
      if (childTransform.parent != null)
        children.Add(childTransform.gameObject);
    }

    return children;
  }
	
}
