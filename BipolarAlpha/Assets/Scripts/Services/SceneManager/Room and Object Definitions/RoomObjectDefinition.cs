using UnityEngine;
using System.Collections;

/// <summary>
/// Represents an object within a room
/// </summary>
public class RoomObjectDefinition
{
  private string _prefabPath;

  private Vector3 _position;
  private Vector3 _scale;
  private Vector3 _eulerAngles;

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

  public RoomObjectDefinition() { }

  public RoomObjectDefinition(string prefabPath, Vector3 position, Vector3 scale, Vector3 eulerAngles)
  {
    _prefabPath = prefabPath;
    _position = position;
    _scale = scale;
    _eulerAngles = eulerAngles;
  }

}
