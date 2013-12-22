//Owner Ivo
using UnityEngine;
using System.Collections;


/// <summary>
/// AbilityUseMagnet provides use of a player's single directional magnet
/// </summary>
public class AbilityUseMagnet : Ability
{

  #region protected fields

  //Enum that indicates whether ability reffers to left or right arms
  protected enum Arm { Unkown, Left, Right };
  protected Arm _arm = Arm.Unkown;

  protected PlayerController _player;
  protected PlayerMagnet _playerMagnet;
  protected Camera _playerCamera;
  
  // Player Animation handler
  protected AnimationRootHandler _animHandler;
  
  #endregion

  #region Constructor

  /// <summary>
  /// Constructor for AbilityUseMagnet
  /// </summary>

  public AbilityUseMagnet(PlayerMagnet playerMagnet, Camera playerCamera, PlayerController player)
  {
    if (_playerMagnet) Debug.Log("Its not null!");
    this._playerMagnet = playerMagnet;
    this._playerCamera = playerCamera;
    this._player = player;
    _animHandler = player.GetComponent<AnimationRootHandler>();
  }

  #endregion

  #region Ability methods

  /// <summary>
  /// Activates the associated directional Magnet with the forward direction of the player's camera
  /// </summary> 
  public virtual void Use(PlayerController caller, string key = null)
  {
    //For first use defines which arm abillity reffers to
    setArm(key);

    //Applies magnetic forces
    ApplyForces(caller, _playerCamera.transform.forward);
  }

  public virtual void KeyUp(string key = null)
  {
      //Deactivates magnet
       _playerMagnet.isActivated = false;
       StopAnimation(key);
  }

  public virtual void KeyDown(string key = null)
  {
    //Activates magnet
    _playerMagnet.isActivated = true;
    UseAnimation(key);
  }

  #endregion

  #region Animation
  // Does Release Magnet Animation
  protected void StopAnimation(string key)
  {
    if (!key.Contains("Release"))
    {
      switch (_arm)
      {
        case Arm.Left:
          _animHandler.getChildAnimation("LeftClaw").CrossFade("MagnetInitial");
          break;
        case Arm.Right:
          _animHandler.getChildAnimation("RightClaw").CrossFade("MagnetInitialReverse");
          break;
      }
      _playerMagnet.DisableMagnetHitHaloLight();
    }
  }

  //Does Use Claw animation;
  protected void UseAnimation(string key)
  {
    if (!key.Contains("Release"))
    {
      switch(_arm)
      {
        case Arm.Left:
          _animHandler.getChildAnimation("LeftClaw").CrossFade("MagnetActive");
          break;
        case Arm.Right:
          _animHandler.getChildAnimation("RightClaw").CrossFade("MagnetActiveReverse");
          break;
      }
      _playerMagnet.EnableMagnetHitHaloLight();
    }
  }
  #endregion

  #region helper methods

  /// <summary>
  /// Applies forces on player and other magnets according to used player magnet
  /// </summary>
  /// <param name="caller">The Player</param>
  /// <returns></returns>
  protected MagneticForce ApplyForces(PlayerController caller, Vector3 direction)
  {
    //Cast ray to check for magnets
    MagneticForce force = _playerMagnet.FireRayCast(_playerCamera.transform.position, direction);

    //Apply force on player
    if (force != null)
    {
      force.ApplyOtherMagnetsForces(caller.rigidbody);
    }

    return force;
  }

    protected void setArm(string key)
    {
      if (_arm == Arm.Unkown)
      {
        if (key.CompareTo("Fire1") == 0 || key.CompareTo("Release1")==0)
        {
          _arm = Arm.Left;
        }
        else if (key.CompareTo("Fire2") == 0 || key.CompareTo("Release2") == 0)
        {
          _arm = Arm.Right;
        }
      }
    }
  #endregion
}
