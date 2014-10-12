using System;
using UnityEngine;

/// <summary>
/// Gameplay checkpoint entity that saves respawn conditions for player
/// Entity should contains a collision area to detect players and a dummy entity to set player position and rotation
/// </summary>
[RequireComponent (typeof (Transform))]
public class CheckPoint : MonoBehaviour
{

  #region private variables

  private Vector3 _checkpointedPlayerPosition;
  private Vector3 _checkpointedPlayerRotation;

  #pragma warning disable 414
  //For quick modifiability should contains a transform helper
  private Transform _transformComponent;

  // Checkpoint needs to know if it is the active checkpoint to avoid Event message spams when player is in collision area
  private bool _isActive;

  #endregion

  #region properties

  public bool isActive
  {
    get
    {
      return _isActive;
    }
    set
    {
      _isActive = value;
    }
  }

  //Saved Player position = checkPointPos + inside pos
  public Vector3 savedPlayerPosition
  {
    get
    {
      // for some reason position is being doubled
      return  _checkpointedPlayerPosition;
    }
  }

  //Saved Player rotation = checkPointRot + inside rot
  public Vector3 savedPlayerRotation
  {
    get
    {
      return _checkpointedPlayerRotation;
    }
  }

  #endregion

  #region Monobehavior methods

  private void Start()
  {
    _transformComponent = GetComponent<Transform>();

    // Find Dummy Player used to set respawn position and direction
    Transform child = transform.FindChild("PlayerDummy");
    if (child != null)
    {
        _checkpointedPlayerPosition = child.position;
        _checkpointedPlayerRotation = child.eulerAngles;
    }
  }
  #endregion

  #region Player interface methods

  void OnTriggerEnter(Collider other)
  {
    // Currently checkpoint is activated by having the player pass a collision area
    if (!_isActive && other.tag == "Player")
    {
      _isActive = true;
      //Warn Checkpoint system of new activated Checkpoint
      ServiceLocator.GetEventHandlerSystem().SendCheckPointRegisterEvent(this);
    }
  }

  #endregion
}
