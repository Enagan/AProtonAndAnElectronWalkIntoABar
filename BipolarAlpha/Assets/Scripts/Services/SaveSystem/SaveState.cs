using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveState
{
  #region Private Variables
  private List<string> _roomPaths;

  private string _activeRoom;

  private Vector3 _playerPosition;

  private Vector3 _playerRotation;
  #endregion

  #region Public Properties
  public List<string> roomPaths
  {
    get
    {
      return _roomPaths;
    }
    set
    {
      _roomPaths = value;
    }
  }

  public string activeRoom
  {
    get
    {
      return _activeRoom;
    }
    set
    {
      _activeRoom = value;
    }
  }

  public Vector3 playerPosition
  {
    get
    {
      return _playerPosition;
    }
    set
    {
      _playerPosition = value;
    }
  }

  public Vector3 playerRotation
  {
    get
    {
      return _playerRotation;
    }
    set
    {
      _playerRotation = value;
    }
  }
  #endregion
}