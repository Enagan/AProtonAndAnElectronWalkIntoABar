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
  private AnimationRootHandler _animHandler;

  #endregion

  /// <summary>
    /// Constructor for AbilityUseMagnet
    /// </summary>

  public AbilityUseMagnet(PlayerMagnet playerMagnet, Camera playerCamera, PlayerController player)
  {
    this._playerMagnet = playerMagnet;
    this._playerCamera = playerCamera;
    _animHandler = player.GetComponent<AnimationRootHandler>();
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

    BipolarConsole.AllLog("KEY" + key);

    if (!key.Contains("Release"))
    {
      //leftMagnet
      if (key.CompareTo("Fire1") == 0)
      {
        _animHandler.getChildAnimation("LeftClaw").Stop();
      }
      else if (key.CompareTo("Fire2") == 0) // Right
      {
        _animHandler.getChildAnimation("RightClaw").Stop();
      }
    }
  }

  public void KeyDown(string key = null)
  {
    _playerMagnet.isActivated = true;
    //TODO Hackish way to trigger anim, should consider change
    if (!key.Contains("Release"))
    {
      //leftMagnet
      if (key.CompareTo("Fire1") == 0)
      {
        _animHandler.getChildAnimation("LeftClaw").CrossFade("MagnetActive");
      }
      else if (key.CompareTo("Fire2") == 0) // Right
      {
        _animHandler.getChildAnimation("RightClaw").CrossFade("MagnetActiveReverse");
      }
    }
    
  }
}
