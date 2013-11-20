using UnityEngine;
using System.Collections;

/// <summary>
/// A ComplexState is a collection of class specific variables that must be saved for the game to correctly behave when saving and loading world states.
/// </summary>
public class ComplexState {
  private string _objectNameInHierarchy = "";

  /// <summary>
  /// Scene Graph Hierarchy path of the object this complex state pertains too
  /// WARNING!!! This property is public for seralization sake only. NEVER modify it in runtime yourself
  /// </summary>
  public string objectNameInHierarchy
  {
    get
    {
      return _objectNameInHierarchy;
    }
    set
    {
      _objectNameInHierarchy = value;
    }
  }

  virtual public string GetComplexStateName()
  { 
    return "Base Complex State";
  }

  public ComplexState()
  {
    
  }

  /// <summary>
  /// Complex State first time construction. Receives the object which has a complex state in order to build the hierarchy path to it.
  /// </summary>
  public ComplexState(GameObject complexStateSourceObject) 
  { 
    string pathToObject = complexStateSourceObject.transform.name;
    Transform traversingHierarchyUpwards = complexStateSourceObject.transform.parent;
    if (traversingHierarchyUpwards != null)
    {
      while (traversingHierarchyUpwards.parent != null)
      {
        pathToObject = traversingHierarchyUpwards.name + "/" + pathToObject;
        traversingHierarchyUpwards = traversingHierarchyUpwards.parent;
      }
    }
    _objectNameInHierarchy = pathToObject;
  }
}
