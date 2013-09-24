using System;
using UnityEngine;

/// <summary>
/// Hackish Implementation of a simple checkpoint system (non-time travely)
/// TODO, make this into a decent system
/// </summary>
class CheckPoint : MonoBehaviour
{
  private static Vector3 _checkpointedPlayerPosition;
  private static Vector3 _checkpointedPlayerRotation;

  public static Vector3 savedPlayerPosition
  {
    get
    {
      return _checkpointedPlayerPosition;
    }
  }
  public static Vector3 savedPlayerRotation
  {
    get
    {
      return _checkpointedPlayerRotation;
    }
  }

  void OnTriggerEnter(Collider other)
  {
    if (other.tag == "Player")
    {
      //ServiceLocator.GetSceneManager().SaveRooms();
      _checkpointedPlayerPosition = other.transform.position;
      _checkpointedPlayerRotation = other.transform.eulerAngles;
    }
  }
}