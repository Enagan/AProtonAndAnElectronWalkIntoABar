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
  private PlayerController _player;
  #endregion

  /// <summary>
    /// Constructor for AbilityUseMagnet
    /// </summary>
    /// <param name="playerMagnet">The magnet associated with this ability's trigger</param>
    /// 
    /// <param name="playerCamera">The camera associated with the player</param>
    ///  
    /// <param name="player">The player associated to this magnet</param>
  public AbilityUseMagnet(PlayerMagnet playerMagnet, Camera playerCamera, PlayerController player)
  {
    this._playerMagnet = playerMagnet;
    this._playerCamera = playerCamera;
    this._player = player;

  }
  /// <summary>
    /// Activates the associated directional Magnet with the forward direction of the player's camera
    /// </summary> 
  public void Use(PlayerController caller, string key = null)
  {

    //Does this every Update when pressing the ability button
    MagneticForce force = _playerMagnet.FireRayCast(_playerCamera.transform.forward);
       
    if (force != null)
    {
      force.ApplyOtherMagnetsForces(_player.rigidbody);

    }
	
  }

  public void KeyUp()
  {
    _playerMagnet.isActivated = false;
  }

  public void KeyDown()
  {
    _playerMagnet.isActivated = true;
  }
}
