using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilityActivateActivatable : MonoBehaviour, Ability
{
  private int _raycastMask = ((1 << 0) | (1 << 17)); //Only consider objects in layer 0 (Default) and 17 (Jacked-In)

  private Camera _jackedInCamera;

  public AbilityActivateActivatable(Camera cam)
  {
    _jackedInCamera = cam;
  }

  public void Use(PlayerController player, string key)
  {

  }

  public void KeyUp(string key)
  {

  }

  public void KeyDown(string key)
  {
    FireRayCast(_jackedInCamera.transform.position, _jackedInCamera.transform.forward, key);
  }

  public void FireRayCast(Vector3 start, Vector3 direction, string key)
  {
    RaycastHit hit;
    if (Physics.Raycast(start, direction, out hit, Mathf.Infinity, _raycastMask) && hit.collider.CompareTag("Activatable"))
    {
      // Activates an Activatable object hit by the raycast
      Transform otherTransform = hit.collider.gameObject.transform;
      JackedInRemoteController activatable = otherTransform.GetComponent<JackedInRemoteController>();
      activatable.Hit(key);
    }
  }
}