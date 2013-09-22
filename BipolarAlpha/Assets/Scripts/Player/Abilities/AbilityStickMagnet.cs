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
  #endregion

  /// <summary>
  /// Constructor for AbilityUseMagnet
  /// </summary>
  public AbilityStickMagnet(PlayerMagnet playerMagnet, Camera playerCamera)
  {
    _playerMagnet = playerMagnet;
    _playerCamera = playerCamera;
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
        FreeFromMagnet(caller);
      }
      if (_magnetStuckToArm != null)
      {
        ReleaseMagnetStuckToPlayer();
      }
    }
    if ((key.Contains("Fire")) && _playerMagnet.isAvailable)
    {
      //Does this every Update when pressing the ability button
      MagneticForce force = _playerMagnet.FireRayCast(_playerCamera.transform.position, _playerCamera.transform.forward);

      if (force != null)
      {
        _playerMagnet.EnableMagnetHitHaloLight();
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
                StickMagnetToPlayer(caller, otherMagnetForce);
              }
            }
            else if (!_stuckToMagnet)
            {
              StickToMagnet(otherMagnetForce, caller);
            }
            // Deactives the player magnet while it's sticking to another magnet
            _playerMagnet.isActivated = false;
            break;
          }

          // When the player is holding a magnet but uses the same magnet charge to push it away
          if (otherMagnetForce.transform.parent.gameObject == _magnetStuckToArm &&
            _playerMagnet.charge == otherMagnetForce.charge)
          {
            ReleaseMagnetStuckToPlayer();

            //TODO SUPA Hackish Way to trigger anim, should be changed
            _playerMagnet.transform.parent.parent.FindChild("Left Player Magnet").FindChild("ClawMagnet").FindChild("Spike").GetComponent<Animation>().Play("RetractSpike");
            _playerMagnet.transform.parent.parent.FindChild("Right Player Magnet").FindChild("ClawMagnet").FindChild("Spike").GetComponent<Animation>().Play("RetractSpike");
          }
        }
        force.ApplyOtherMagnetsForces(caller.rigidbody);
      }
      else
      {
        _playerMagnet.DisableMagnetHitHaloLight();
      }
    }
  }

  /// <summary>
  /// Sticks the magnet associated with the Magnetic force to the player
  /// </summary>
  private void StickMagnetToPlayer(PlayerController caller, MagneticForce magnet)
  {
    //TODO Hackish Way to trigger anim, should be changed
    _playerMagnet.transform.parent.FindChild("ClawMagnet").FindChild("Spike").GetComponent<Animation>().Play("UseSpike");

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
  private void ReleaseMagnetStuckToPlayer()
  {
    if (_magnetStuckToArm == null)
    {
      return;
    }

    //TODO Hackish Way to trigger anim, should be changed
    _playerMagnet.transform.parent.FindChild("ClawMagnet").FindChild("Spike").GetComponent<Animation>().Play("RetractSpike");

    _magnetStuckToArm.transform.parent = _previousMagnetParent;
    _previousMagnetParent = null;
    _magnetStuckToArm.GetComponent<Rigidbody>().isKinematic = false;
    _magnetStuckToArm.layer = LayerMask.NameToLayer("Default");
    _magnetStuckToArm = null;
  }

  /// <summary>
  /// Stops the player's movement, in the case it sticks to a static (non-moveable) magnet
  /// </summary>
  private void StickToMagnet(MagneticForce otherMagnet, PlayerController caller)
  {
    if (otherMagnet == null)
    {
      return;
    }
    //TODO Hackish Way to trigger anim, should be changed
    _playerMagnet.transform.parent.FindChild("ClawMagnet").FindChild("Spike").GetComponent<Animation>().Play("UseSpike");

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
  private void FreeFromMagnet(PlayerController caller)
  {
    if (!_stuckToMagnet)
    {
      return;
    }
    //TODO Hackish Way to trigger anim, should be changed
    _playerMagnet.transform.parent.FindChild("ClawMagnet").FindChild("Spike").GetComponent<Animation>().Play("RetractSpike");

    _stuckToMagnet = false;
    
    caller.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    caller.transform.parent = null;
  }

  public void KeyUp()
  {
    //TODO Hackish Way to trigger anim, should be changed
    _playerMagnet.transform.parent.FindChild("ClawMagnet").FindChild("ClawJoint").GetComponent<Animation>().Stop();

    _playerMagnet.isActivated = false;
    _playerMagnet.currentHitPoint = Vector3.zero;
    _playerMagnet.magnetHitPoint = Vector3.zero;

    _playerMagnet.DisableMagnetHitHaloLight();
  }

  public void KeyDown()
  {
    //TODO Hackish Way to trigger anim, should be changed
    _playerMagnet.transform.parent.FindChild("ClawMagnet").FindChild("ClawJoint").GetComponent<Animation>().Play();

    _playerMagnet.isActivated = true;
  }
}
