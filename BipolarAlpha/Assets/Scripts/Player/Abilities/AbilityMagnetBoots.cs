using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// AbilityMagnetBoots provides use of a player's magnet boots
/// </summary>
public class AbilityMagnetBoots : Ability
{
  #region private fields
  protected PlayerController _player;
  protected MagnetBoots _magnetBoots;
  protected Camera _playerCamera;

  #endregion

  /// <summary>
  /// Constructor for AbilityMagnetBoots
  /// </summary>
  public AbilityMagnetBoots(MagnetBoots magnetBoots, Camera playerCamera, PlayerController player)
  {
    this._magnetBoots = magnetBoots;
    this._player = player;
    this._playerCamera = playerCamera;

  }


  public void Use(PlayerController caller, string key = null)
  {
    //Does this every Update when pressing the ability button
    MagneticForce force = _magnetBoots.FireRayCast(_playerCamera.transform.position);

    //Apply force on player
    if (force != null)
    {
      force.ApplyOtherMagnetsForces(caller.rigidbody);
    }

    }

  public void KeyUp(string key = null)
  {
    _magnetBoots.Deactivate();
  }

  public void KeyDown(string key = null)
  {
    _magnetBoots.isActivated = true;
    _magnetBoots.Activate();

  }

}
