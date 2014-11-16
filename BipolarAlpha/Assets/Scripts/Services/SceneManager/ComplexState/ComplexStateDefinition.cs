using UnityEngine;
using System.Collections;

namespace SMSceneManagerSystem
{
  /// <summary>
  /// A ComplexState is a collection of class specific variables that must be saved for the game to correctly behave when saving and loading world states.
  /// Subclasses should be created as needeed to represent different sets of variables to save
  /// </summary>
  public abstract class ComplexStateDefinition
  {
    private string _objectNameInHierarchy = "";

    /// <summary>
    /// Scene Graph Hierarchy path of the object this complex state pertains too
    /// WARNING!!! This property is public for seralization sake only. Modifying it otherwise will most likely break things unexpectedly
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

    public ComplexStateDefinition() { }


    public ComplexStateDefinition(GameObject complexStateSourceObject)
    {
      string pathToObject = complexStateSourceObject.transform.name;

      // Builds the game object scene graph path to the subobject with complex state
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

}
