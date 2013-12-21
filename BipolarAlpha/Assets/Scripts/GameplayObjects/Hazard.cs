using System;
using UnityEngine;

/// <summary>
/// Kills the player and sends him to checkpoint
/// </summary>
class Hazard : MonoBehaviour
{
  void OnTriggerEnter(Collider other)
  {
    if (other.tag == "Player")
    {
      ServiceLocator.GetCheckPointSystem().respawnPlayer(other.GetComponent<PlayerController>());
      //ServiceLocator.GetSceneManager().LoadRooms();
    }
  }
}
