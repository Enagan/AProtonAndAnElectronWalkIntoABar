using UnityEngine;
using System.Collections;

public class AbilityJackedIn : Ability
{

  protected Camera _playerCamera;
  private int _raycastMask = ~((1 << 8) | (1 << 13) | (1 << 15) | (1 << 17));  //Ignore objects in layer 8 (Magnetic Force) and 13 (Triggers) and 15 (Magnetic Blockers)

  public AbilityJackedIn(Camera playerCamera)
  {
    this._playerCamera = playerCamera;
  }
  public void KeyUp(string key = null) { }

  public void Use(PlayerController caller, string key = null) { }


  public void KeyDown(string key = null)
  {
    RaycastHit hit;
    if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out hit, Mathf.Infinity, _raycastMask) && hit.collider.CompareTag("Console"))
    {
      if((Vector3.Distance(_playerCamera.transform.position, hit.transform.position) < 3.0f)){
       hit.collider.gameObject.GetComponent<Console>().ActivateJackedIn();
      }
    }
  }
}

