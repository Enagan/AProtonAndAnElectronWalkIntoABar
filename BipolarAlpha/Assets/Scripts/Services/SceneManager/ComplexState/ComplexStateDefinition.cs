//---------------------------------------------
// Bipolar
// Written by: Pedro Engana
//---------------------------------------------

using UnityEngine;
using System.Collections;

namespace SMSceneManagerSystem
{
  /// <summary>
  /// A ComplexStateDefinition is a collection of class specific variables that must be saved in order for their game objects to be correctly instanced.
  /// This is the abstract superclass for all Complex State definitions, subclasses should be created as needeed to represent different sets of variables to save.
  /// All subclasses must use base, serializable variables, so complex data sets must be translated into simpler constructs, or into several different complex states.
  /// </summary>
  public abstract class ComplexStateDefinition
  {
    private string _objectNameInHierarchy = "";

    /// <summary>
    /// Scene Graph Hierarchy path of the object this complex state pertains too.
    /// The complex state might not be at the parent object of a given prefab, sometimes it's deep within it's hierarchy.
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
