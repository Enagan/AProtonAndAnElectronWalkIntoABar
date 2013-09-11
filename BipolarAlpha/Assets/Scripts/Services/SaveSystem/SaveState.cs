using System;
using System.Collections.Generic;
using UnityEngine;

// Class that records information of the state of the world
public class SaveState
{
  #region Private Variables
  // Paths where the room save files are kept
  private List<string> _roomPaths;

  // Room where the player was
  private string _activeRoom;

  // State of the player
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