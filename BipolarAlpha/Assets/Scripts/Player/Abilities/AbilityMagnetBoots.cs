using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// AbilityMagnetBoots provides use of a player's magnet boots
/// </summary>
public class AbilityMagnetBoots : AbilityUseMagnet
{
  #region private fields

  #endregion

  /// <summary>
  /// Constructor for AbilityMagnetBoots
  /// </summary>
  public AbilityMagnetBoots(PlayerMagnet playerMagnet, Camera playerCamera, PlayerController player)
    : base(playerMagnet,playerCamera,player)
  {
  }

  public override void Use(PlayerController caller, string key = null)
  {
    //Does this every Update when pressing the ability button
    ApplyForces(caller, Vector3.down);
  }

  public override void KeyUp(string key = null)
  {
    _playerMagnet.isActivated = false;
    _playerMagnet.DisableMagnetHitHaloLight();
  }

  public override void KeyDown(string key = null)
  {
    _playerMagnet.isActivated = true;
    _playerMagnet.EnableMagnetHitHaloLight();
  }

}
