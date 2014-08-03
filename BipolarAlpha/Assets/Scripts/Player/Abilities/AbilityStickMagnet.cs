//Owner Rui
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// AbilityStickMagnet provides use of a player's single directional magnet, and permiting sticking to magnets,
/// as well, as releasing a sticked magnet
/// </summary>
public class AbilityStickMagnet : AbilityUseMagnet
{

  #region private Variables

  //For attaching magnets to player
  private static GameObject _magnetStuckToArm = null;
  private Transform _previousMagnetParent = null;
  private MagneticForce.Charge _previousMagnetCharge;

  // For attaching player to magnets
  private static bool _isStuck = false;

  //Player should be released after button is repressed
  private bool _canReleasePlayer = true;

  #endregion
  
  #region Constructor 

  /// <summary>
  /// Constructor for AbilityStickMagnet
  /// </summary>

  public AbilityStickMagnet(PlayerMagnet playerMagnet, PlayerController player)
    : base(playerMagnet, player)
  { }


  public AbilityStickMagnet(PlayerMagnet playerMagnet, Camera playerCamera, PlayerController player) :base(playerMagnet, playerCamera, player) 
  {}

  #endregion

  #region Ability methods

  public override void Use(PlayerController caller, string key=null)
  {
    //For first use defines which arm abillity reffers to
    setArm(key);

    //Check if needs to release player
    if (_canReleasePlayer && key.Contains("Release")) 
    {
      if (_isStuck)
      {
        //Release Player From Magnet
        unstickPlayerFromMagnet(caller);
        
      }
      else  if (_magnetStuckToArm != null)
      {
        //Release Magnet From Player
        if(_previousMagnetCharge == _playerMagnet.charge)
           unstickMagnetFromPlayer(true); // Unstick and apply initial repulsion
        else
           unstickMagnetFromPlayer(false); // Unstick
      }
      unstickAnimation();
      return;
    }

    //Apply Forces if not holding anything
    MagneticForce force = null;

    force = ApplyForces(caller, _playerCamera.transform.forward);

    // if the player is repeling the magnet it's holding
    if (force != null && force.transform.parent.gameObject == _magnetStuckToArm && force.charge == this._playerMagnet.charge)
    {
     
      unstickMagnetFromPlayer(true);
      unstickAnimation();
      return;
    }


    //Check if Collisions happened
    if (force != null)
    {
      //Check if player is colliding with a magnet with opposite charge (mutual attraction)
      if (caller.magnetColliding &&
       _playerMagnet.charge != caller.magnetCollidingWith.charge)
      {
        if (caller.magnetCollidingWith.isHoldable)
        {
          //Player Holds Magnets
          stickMagnetToPlayer(caller, caller.magnetCollidingWith);

        }
        else
        {
          //Magnets Hold Player
          stickPlayerToMagnet(caller, caller.magnetCollidingWith);
        }
        stickAnimation();
      }
    }
    
  }


  public override void KeyUp(string key = null)
  {
    //After use if player got stuck on next use he should be released
    if (_isStuck || _magnetStuckToArm != null)
      _canReleasePlayer = true;

    //Activates magnet
    base.KeyUp(key);
  }


  public override void KeyDown(string key = null)
  {
    // On Press if player isnt stuck nor has a magnet he cannot be released (nothing attached anyway)
    if (!_isStuck && _magnetStuckToArm == null)
      _canReleasePlayer = false;

    //Deactivates magnet
    base.KeyDown(key);
  }
  #endregion

  #region Stick/Unstick methods

  /// <summary>
  /// Attaches Player to a non Holdable Magnet
  /// </summary>
  /// <param name="caller">The Player</param>
  /// <param name="otherMagnet">Colliding Magnet</param>
  protected void stickPlayerToMagnet(PlayerController caller, MagneticForce otherMagnet)
  {
    //if no magnet returns
    if (otherMagnet == null)
    {
      return;
    }

    _isStuck = true;

    //Stops player
    caller.rigidbody.velocity = Vector3.zero;
    caller.rigidbody.angularVelocity = Vector3.zero;
    caller.transform.parent = otherMagnet.transform;
    caller.rigidbody.constraints = RigidbodyConstraints.FreezeAll;

  }

  /// <summary>
  /// Releases Player from being attached to a magnet
  /// </summary>
  /// <param name="caller">The Player</param>
  protected void unstickPlayerFromMagnet(PlayerController caller)
  {
    // if not stuck return
    if (!_isStuck)
    {
      return;
    }

    //Enables player movement
    caller.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    caller.transform.parent = null;
    _isStuck = false;
  }

  /// <summary>
  /// Sticks a Holdable Magnet to the player
  /// </summary>
  /// <param name="caller">The Player</param>
  /// <param name="magnet">Colliding Magnet</param>
  protected void stickMagnetToPlayer(PlayerController caller, MagneticForce magnet)
  {
    
    //Find Magnet Parent for later restore
    GameObject magnetParent = magnet.transform.parent.gameObject;
    magnetParent.GetComponent<Rigidbody>().isKinematic = true;
    magnetParent.layer = LayerMask.NameToLayer("HeldByPlayerMagnet");
    _previousMagnetParent = magnetParent.transform.parent;

    //Change magnet parent so it is carried about
    magnetParent.transform.parent = caller.transform.FindChild("Camera");

    //Place in hands
    magnetParent.transform.localPosition = new Vector3(0, 0, 1.5f);
    _magnetStuckToArm = magnetParent;

    _previousMagnetCharge = magnet.charge;

  }
  
  /// <summary>
  /// Unsticks magnet that player is holding
  /// </summary>
  /// <param name="initialRepulsion">Whether to apply repulsion or not</param>
  protected void unstickMagnetFromPlayer(bool initialRepulsion = false)
  {
    //Check if there is a magnet to release
    if (_magnetStuckToArm == null)
    {
      return;
    }


    //Restore previous parent
    _magnetStuckToArm.transform.parent = _previousMagnetParent;
    _previousMagnetParent = null;
    _magnetStuckToArm.GetComponent<Rigidbody>().isKinematic = false;
    _magnetStuckToArm.layer = LayerMask.NameToLayer("Default");

    if (initialRepulsion)
    {
      _magnetStuckToArm.rigidbody.AddForce( 3 * _playerCamera.transform.forward * _playerMagnet.getForceValue(_playerMagnet.force));
    }
    _magnetStuckToArm = null;

  }

  #endregion
 
  #region Animations

  /// <summary>
  /// Calls animations for using Spike
  /// </summary>
  protected void stickAnimation()
  {  
      switch (_arm)
      {
        case Arm.Left:
          _animHandler.getChildAnimation("LeftSpike").CrossFade("RetractSpike");
          break;
        case Arm.Right:
          _animHandler.getChildAnimation("RightSpike").CrossFade("RetractSpike");
          break;
      }
      //ServiceLocator.GetAudioSystem().PlayLoopingSFX("MagnetHitBuzz", _playerMagnet.gameObject.transform);
  }

  /// <summary>
  /// Calls animations for Retracting Spike
  /// </summary>
  protected void unstickAnimation()
  {
    //Does animation on both hands since it can be unstuck with both abilities
     _animHandler.getChildAnimation("LeftSpike").CrossFade("UseSpike");
     _animHandler.getChildAnimation("RightSpike").CrossFade("UseSpike");
     //ServiceLocator.GetAudioSystem().StopLoopingSFX("MagnetHitBuzz", _playerMagnet.gameObject.transform);
  }

#endregion
}

