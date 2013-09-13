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

  public void KeyUp()
  {
    _playerMagnet.isActivated = false;
  }

  public void KeyDown()
  {
    _playerMagnet.isActivated = true;
  }
}
