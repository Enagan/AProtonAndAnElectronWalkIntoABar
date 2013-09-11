using System;
using UnityEngine;
using UnityEditor;

class Upgrade : MonoBehaviour
{
  [SerializeField]
  // Name of the ability this object is keeping
  private string _abilityName;

  void OnTriggerEnter(Collider other)
  {
    if (other.tag == "Player")
    {
      ServiceLocator.GetEventHandlerSystem().SendPlayerAbilityObtainEvent(_abilityName);
      Destroy(this);
    }
  }
}
