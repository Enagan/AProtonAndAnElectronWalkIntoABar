using System;
using UnityEngine;

class CheckPoint : MonoBehaviour
{
  void OnTriggerEnter(Collider other)
  {
    if (other.tag == "Player")
    {
      ServiceLocator.GetSceneManager().SaveRooms();
    }
  }
}