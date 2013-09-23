using System;
using UnityEngine;

class Hazard : MonoBehaviour
{
  void OnTriggerEnter(Collider other)
  {
    if (other.tag == "Player")
    {
      ServiceLocator.GetSceneManager().LoadRooms();
    }
  }
}
