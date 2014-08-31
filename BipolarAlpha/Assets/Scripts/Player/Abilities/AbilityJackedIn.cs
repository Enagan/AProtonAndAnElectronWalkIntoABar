using UnityEngine;
using System.Collections;

public class AbilityJackedIn : Ability
{

  private const float MAX_DISTANCE_FROM_CONSOLE_TO_ACTIVATE = 3.0f;
  protected Camera _playerCamera;
  private int _raycastMask = ~((1 << 8) | (1 << 13) | (1 << 15) | (1 << 17));  //Ignore objects in layer 8 (Magnetic Force) and 13 (Triggers) and 15 (Magnetic Blockers)

  public AbilityJackedIn(Camera playerCamera)
  {
    this._playerCamera = playerCamera;
  }
  public void KeyUp(string key = null) { }

  public void Use(string key = null) { }


  public void KeyDown(string key = null)
  {
    RaycastHit hit;
    if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out hit, MAX_DISTANCE_FROM_CONSOLE_TO_ACTIVATE, _raycastMask) && hit.collider.CompareTag("Console"))
    {
      hit.collider.gameObject.GetComponent<Console>().ActivateJackedIn();
    }
  }
}

