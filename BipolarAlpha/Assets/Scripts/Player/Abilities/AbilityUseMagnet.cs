//Owner Ivo
using UnityEngine;
using System.Collections;


/// <summary>
/// AbilityUseMagnet provides use of a player's single directional magnet
/// </summary>
public class AbilityUseMagnet : Ability
{

  #region private fields

  private PlayerMagnet _playerMagnet;
  private Camera _playerCamera;

  #endregion

  /// <summary>
    /// Constructor for AbilityUseMagnet
    /// </summary>

  public AbilityUseMagnet(PlayerMagnet playerMagnet, Camera playerCamera, PlayerController player)
  {
    this._playerMagnet = playerMagnet;
    this._playerCamera = playerCamera;
  }
  /// <summary>
    /// Activates the associated directional Magnet with the forward direction of the player's camera
    /// </summary> 
  public void Use(PlayerController caller, string key = null)
  {
    //Does this every Update when pressing the ability button
    MagneticForce force = _playerMagnet.FireRayCast(_playerCamera.transform.position, _playerCamera.transform.forward);
       
    if (force != null)
    {
      force.ApplyOtherMagnetsForces(caller.rigidbody);
    }
	
  }

  public void KeyUp(string key = null)
  {
    _playerMagnet.isActivated = false;
    //TODO Hackish way to trigger anim, should consider change
    if (!key.Contains("Release"))
    {
      _playerMagnet.transform.parent.FindChild("ClawMagnet").FindChild("ClawJoint").GetComponent<Animation>().Stop();
    }
  }

  public void KeyDown(string key = null)
  {
    _playerMagnet.isActivated = true;
    //TODO Hackish way to trigger anim, should consider change
    if (!key.Contains("Release"))
    {
      _playerMagnet.transform.parent.FindChild("ClawMagnet").FindChild("ClawJoint").GetComponent<Animation>().Play();
    }
    
  }
}
