using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// AbilityMagnetBoots provides use of a player's magnet boots
/// </summary>
public class AbilityMagnetBoots : Ability
{
  #region private fields
  private PlayerMagnet _playerMagnet;
  private Camera _playerCamera;

  #endregion

  /// <summary>
  /// Constructor for AbilityMagnetBoots
  /// </summary>
  public AbilityMagnetBoots(PlayerMagnet playerMagnet, Camera playerCamera)
  {
    _playerMagnet = playerMagnet;
    _playerCamera = playerCamera;
  }

  public void Use(PlayerController caller, string key = null)
  {
    //Does this every Update when pressing the ability button
    MagneticForce force = _playerMagnet.FireRayCast(_playerCamera.transform.position, Vector3.down);

    if (force != null)
    {
      force.ApplyOtherMagnetsForces(caller.rigidbody);
    }

  }

  public void KeyUp(string key = null)
  {
    _playerMagnet.isActivated = false;
    _playerMagnet.DisableMagnetHitHaloLight();
  }

  public void KeyDown(string key = null)
  {
    _playerMagnet.isActivated = true;
    _playerMagnet.EnableMagnetHitHaloLight();
  }

}
