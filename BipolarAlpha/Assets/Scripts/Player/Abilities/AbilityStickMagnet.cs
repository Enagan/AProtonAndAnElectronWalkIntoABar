//Owner Rui
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// AbilityStickMagnet provides use of a player's single directional magnet, and permiting sticking to magnets,
/// as well, as releasing a sticked magnet
/// </summary>
public class AbilityStickMagnet : Ability
{
  #region private fields
  private PlayerMagnet _playerMagnet;
  private Camera _playerCamera;

  private bool _stuckToMagnet = false;

  private static GameObject _magnetStuckToArm = null;
  private Transform _previousMagnetParent = null;

  //parent root handler
  private AnimationRootHandler _animHandler;
  #endregion

  /// <summary>
  /// Constructor for AbilityUseMagnet
  /// </summary>
  public AbilityStickMagnet(PlayerMagnet playerMagnet, Camera playerCamera)
  {
    _playerMagnet = playerMagnet;
    _playerCamera = playerCamera;

    //I blame people who arent passing playerController by. Ivo.
    // Also blame the people that are trying to call animations in a brother hiearchy
    _animHandler = _playerMagnet.transform.parent.parent.parent.GetComponent<AnimationRootHandler>();
  }

  /// <summary>
  /// Activates the associated directional Magnet with the forward direction of the player's camera
  /// </summary>
  public void Use(PlayerController caller, string key = null)
  {
    if (key.Contains("Release"))
    {
      if (_stuckToMagnet)
      {
        FreeFromMagnet(caller,key);
      }
      if (_magnetStuckToArm != null)
      {
        ReleaseMagnetStuckToPlayer(key);
      }
    }
    if ((key.Contains("Fire")) && _playerMagnet.isAvailable)
    {
      if (caller.magnetColliding &&
        _playerMagnet.charge != caller.magnetCollidingWith.charge &&
        caller.magnetCollidingWith.isHoldable &&
        _magnetStuckToArm == null)
      {
        ServiceLocator.GetAudioSystem().StopLoopingSFX("MagnetHitBuzz", _playerMagnet.gameObject.transform);
        StickMagnetToPlayer(caller, caller.magnetCollidingWith,key);
      }


      //Does this every Update when pressing the ability button
      MagneticForce force = _playerMagnet.FireRayCast(_playerCamera.transform.position, _playerCamera.transform.forward);
      
      if (force != null)
      {
        BipolarConsole.EnganaLog(force.affectingMagnets);
        _playerMagnet.EnableMagnetHitHaloLight();
        ServiceLocator.GetAudioSystem().PlayLoopingSFX("MagnetHitBuzz", _playerMagnet.gameObject.transform,50);

        List<MagneticForce> effectiveMagnets = force.affectingMagnets;
        
        foreach (MagneticForce otherMagnetForce in effectiveMagnets)
        {
          
          // In the case, during the magnet's activity, the player collides with a magnet, they stick
          if (caller.magnetColliding && _playerMagnet.charge != otherMagnetForce.charge)
          {
            
            if (otherMagnetForce.isHoldable)
            {
              
              if (_magnetStuckToArm == null)
              {

                ServiceLocator.GetAudioSystem().StopLoopingSFX("MagnetHitBuzz", _playerMagnet.gameObject.transform);
                StickMagnetToPlayer(caller, otherMagnetForce, key);
              }
            }
            else if (!_stuckToMagnet)
            {
              //Pretty hackish way to get the string
              ServiceLocator.GetEventHandlerSystem().SendTutorialMessageTriggerEvent("I see you used your handy spike to\n" +
                                          "Stick yourself to the magnet!\n" +
                                          "To release spike and fall back down, press\n" +
                                            "\"Q\" or \"E\"\n");
              ServiceLocator.GetAudioSystem().StopLoopingSFX("MagnetHitBuzz", _playerMagnet.gameObject.transform);
              StickToMagnet(otherMagnetForce, caller, key);
            }
            // Deactives the player magnet while it's sticking to another magnet
            _playerMagnet.isActivated = false;
            break;
          }

          // When the player is holding a magnet but uses the same magnet charge to push it away
          if (otherMagnetForce.transform.parent.gameObject == _magnetStuckToArm &&
            _playerMagnet.charge == otherMagnetForce.charge)
          {
            ReleaseMagnetStuckToPlayer(key, true);
            
            
            //_playerMagnet.transform.parent.parent.FindChild("Left Player Magnet").FindChild("ClawMagnet").FindChild("Spike").GetComponent<Animation>().Play("RetractSpike");

            // _animHandler.playChildAnimation("LeftSpike", "RetractSpike");

            //_playerMagnet.transform.parent.parent.FindChild("Right Player Magnet").FindChild("ClawMagnet").FindChild("Spike").GetComponent<Animation>().Play("RetractSpike");

            // _animHandler.playChildAnimation("RightSpike", "RetractSpike");
          }
        }
        force.ApplyOtherMagnetsForces(caller.rigidbody);
      }
      else
      {
        _playerMagnet.DisableMagnetHitHaloLight();
        ServiceLocator.GetAudioSystem().StopLoopingSFX("MagnetHitBuzz", _playerMagnet.gameObject.transform);
      }
    }
  }

  /// <summary>
  /// Sticks the magnet associated with the Magnetic force to the player
  /// </summary>
  private void StickMagnetToPlayer(PlayerController caller, MagneticForce magnet, string key)
  {
    //TODO Hackish Way to trigger anim, should be changed
    //_playerMagnet.transform.parent.FindChild("ClawMagnet").FindChild("Spike").GetComponent<Animation>().Play("UseSpike");

    if (key.CompareTo("Fire1")==0)//left
    {
      // _animHandler.playChildAnimation("LeftSpike", "UseSpike");
    }
    else if (key.CompareTo("Fire2") == 0)//Right
    {
      // _animHandler.playChildAnimation("LeftSpike", "UseSpike");
    }


    GameObject magnetParent = magnet.transform.parent.gameObject;
    magnetParent.GetComponent<Rigidbody>().isKinematic = true;
    magnetParent.layer = LayerMask.NameToLayer("HeldByPlayerMagnet");

    _previousMagnetParent = magnetParent.transform.parent;
    magnetParent.transform.parent = caller.transform.FindChild("Camera");

    magnetParent.transform.localPosition = new Vector3(0, 0, 1.5f);
    _magnetStuckToArm = magnetParent;
  }

  /// <summary>
  /// Releases the currently held magnet from the players grasp
  /// </summary>
  private void ReleaseMagnetStuckToPlayer( string key, bool initialRepulsion = false)
  {
    if (_magnetStuckToArm == null)
    {
      return;
    }

    
    //_playerMagnet.transform.parent.FindChild("ClawMagnet").FindChild("Spike").GetComponent<Animation>().Play("RetractSpike");
    if(key.CompareTo("Fire1")==0) //left
    {
      // _animHandler.playChildAnimation("LeftSpike", "RetractSpike");
    } 
    else if(key.CompareTo("Fire2")==0)//right
    {
      // _animHandler.playChildAnimation("RightSpike", "RetractSpike");
    }
    


    _magnetStuckToArm.transform.parent = _previousMagnetParent;
    _previousMagnetParent = null;
    _magnetStuckToArm.GetComponent<Rigidbody>().isKinematic = false;
    _magnetStuckToArm.layer = LayerMask.NameToLayer("Default");
    if (initialRepulsion)
    {
      _magnetStuckToArm.rigidbody.AddForce(_playerCamera.transform.forward * _playerMagnet.getForceValue(_playerMagnet.force));
    }
    _magnetStuckToArm = null;
  }

  /// <summary>
  /// Stops the player's movement, in the case it sticks to a static (non-moveable) magnet
  /// </summary>
  private void StickToMagnet(MagneticForce otherMagnet, PlayerController caller, string key)
  {
    if (otherMagnet == null)
    {
      return;
    }
    //TODO Hackish Way to trigger anim, should be changed
    //_playerMagnet.transform.parent.FindChild("ClawMagnet").FindChild("Spike").GetComponent<Animation>().Play("UseSpike");
    if (key.CompareTo("Fire1") == 0) //left
    {
   //   _animHandler.playChildAnimation("LeftSpike", "UseSpike");
    }
    else if (key.CompareTo("Fire2") == 0)//right
    {
   //   _animHandler.playChildAnimation("RightSpike", "UseSpike");
    }

    _stuckToMagnet = true;
    caller.rigidbody.velocity = Vector3.zero;
    caller.rigidbody.angularVelocity = Vector3.zero;
    caller.transform.parent = otherMagnet.transform;
    caller.rigidbody.constraints = RigidbodyConstraints.FreezeAll;

    /*caller.transform.rotation = rot;*/
  }

  /// <summary>
  /// Makes the player movable again when released from a (static) magnet, that it was sticking to 
  /// </summary>
  private void FreeFromMagnet(PlayerController caller, string key)
  {
    if (!_stuckToMagnet)
    {
      return;
    }
    //TODO Hackish Way to trigger anim, should be changed
    //_playerMagnet.transform.parent.FindChild("ClawMagnet").FindChild("Spike").GetComponent<Animation>().Play("RetractSpike");
    if (key.CompareTo("Fire1") == 0) //left
    {
      // _animHandler.playChildAnimation("LeftSpike", "UseSpike");
    }
    else if (key.CompareTo("Fire2") == 0)//right
    {
      // _animHandler.playChildAnimation("RightSpike", "UseSpike");
    }

    _stuckToMagnet = false;
    
    caller.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    caller.transform.parent = null;
  }

  public void KeyUp(string key = null)
  {
    if (!key.Contains("Release"))
    {
      //leftMagnet
      if (key.CompareTo("Fire1") == 0)
      {
  //      _animHandler.getChildAnimation("LeftClaw").Stop();
      }
      else if (key.CompareTo("Fire2") == 0) // Right
      {
    //    _animHandler.getChildAnimation("RightClaw").Stop();
      }
    }

    _playerMagnet.isActivated = false;
    _playerMagnet.currentHitPoint = Vector3.zero;
    _playerMagnet.magnetHitPoint = Vector3.zero;

    _playerMagnet.DisableMagnetHitHaloLight();
    ServiceLocator.GetAudioSystem().StopLoopingSFX("MagnetHitBuzz", _playerMagnet.gameObject.transform);
  }

  public void KeyDown(string key = null)
  {
    if (!key.Contains("Release"))
    {
      //leftMagnet
      if (key.CompareTo("Fire1") == 0)
      {
  //      _animHandler.getChildAnimation("LeftClaw").CrossFade("MagnetActive");
      }
      else if (key.CompareTo("Fire2") == 0) // Right
      {
  //      _animHandler.getChildAnimation("RightClaw").CrossFade("MagnetActiveReverse");
      }
    }

    _playerMagnet.isActivated = true;
  }
}
