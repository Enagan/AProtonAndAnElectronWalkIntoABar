//Made By: Engana
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents an object within a room
/// </summary>
public class RoomObjectDefinition
{
  private string _prefabPath;

  private Vector3 _position;
  private Vector3 _scale;
  private Vector3 _eulerAngles;

  private List<ComplexState> _complexStates = new List<ComplexState>();

  /// <summary>
  /// Prefab path in the file structure that cooresponds to this object
  /// </summary>
  public string objectPrefabPath
  {
    get
    {
      return _prefabPath;
    }
    set
    {
      _prefabPath = value;
    }
  }

  /// <summary>
  /// Objects position in the room
  /// </summary>
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

  /// <summary>
  /// Objects scale in the room
  /// </summary>
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

  /// <summary>
  /// Objects rotation in the room
  /// </summary>
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

  /// <summary>
  /// ComplexStates attached to this object
  /// </summary>
  public List<ComplexState> complexStates
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

  // Needed for Serialization
  public RoomObjectDefinition() { }

  public RoomObjectDefinition(string prefabPath, Vector3 position, Vector3 scale, Vector3 eulerAngles)
  {
    _prefabPath = prefabPath;
    _position = position;
    _scale = scale;
    _eulerAngles = eulerAngles;
  }

  /// <summary>
  /// Attach a new Complex State to be saved by this object
  /// </summary>
  public void AddComplexState(ComplexState state)
  {
    foreach (ComplexState previouslyAddedState in _complexStates)
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
