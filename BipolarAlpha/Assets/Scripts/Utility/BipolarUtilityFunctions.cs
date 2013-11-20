using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BipolarUtilityFunctions 
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
	
}
