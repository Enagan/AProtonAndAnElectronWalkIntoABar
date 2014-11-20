//Made By: Pedro Engana
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SMSceneManagerSystem
{
  /// <summary>
  /// A room object definition contains all the information necessary to build an object in the state it was previously saved at.
  /// Here we define basic properties such as position scale and angles, as well as the unity prefab that represents the object.
  /// In case an object is a bitmore complex, we use the Complex States, for every sub-object that wishes to declare additional properties.
  /// The Complex States are containers for these specific properties and can be loaded and saved from their originating objects by use of a kind of visitor pattern.
  /// </summary>
  public class RoomObjectDefinition
  {
    private string _prefabPathForInstancing;

    private Vector3 _position;
    private Vector3 _scale;
    private Vector3 _eulerAngles;

    // If we're sure the object won't change at all during gameplay
    private bool _isStatic;

    private List<ComplexStateDefinition> _complexStates = new List<ComplexStateDefinition>();

    public string prefabPathForInstancing
    {
      get
      {
        return _prefabPathForInstancing;
      }
      set
      {
        _prefabPathForInstancing = value;
      }
    }

    public Vector3 position
    {
      get
      {
        return _position;
      }
      set
      {
        _position = value;
      }
    }

    public Vector3 scale
    {
      get
      {
        return _scale;
      }
      set
      {
        _scale = value;
      }
    }

    public Vector3 eulerAngles
    {
      get
      {
        return _eulerAngles;
      }
      set
      {
        _eulerAngles = value;
      }
    }

    public bool isStatic
    {
      get
      {
        return _isStatic;
      }
      set
      {
        _isStatic = value;
      }
    }

    public List<ComplexStateDefinition> complexStates
    {
      get
      {
        return _complexStates;
      }
      set
      {
        _complexStates = value;
      }
    }

    public RoomObjectDefinition() { }

    public RoomObjectDefinition(string prefabPath, Vector3 position, Vector3 scale, Vector3 eulerAngles, bool isStatic = false)
    {
      _prefabPathForInstancing = prefabPath;
      _position = position;
      _scale = scale;
      _eulerAngles = eulerAngles;
      _isStatic = isStatic;
    }

    /// <summary>
    /// Attach a new Complex State to be saved in this object
    /// </summary>
    public void AddComplexState(ComplexStateDefinition state)
    {
      // Check if this complex state conflicts with another added before
      // A conflict happens when we try to add 2 complex states refrencing the same sub-object
      foreach (ComplexStateDefinition previouslyAddedState in _complexStates)
      {
        if (previouslyAddedState.objectNameInHierarchy.Equals(state.objectNameInHierarchy))
        {
          throw new BipolarExceptionSamePathInHierarchyAsAnotherComplexState("Complex state: " + state.GetComplexStateName() +
                                                                              " in path " + state.objectNameInHierarchy +
                                                                              " is invalid. Another Object with an equal path already registered in this object");
        }
      }
      _complexStates.Add(state);
    }

  }
}
