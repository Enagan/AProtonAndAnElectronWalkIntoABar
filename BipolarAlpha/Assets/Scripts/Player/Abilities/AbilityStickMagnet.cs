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
  private PlayerController _player;

  private bool _stuckToMagnet = false;

  private static GameObject _magnetStuckToArm = null;
  private Transform _previousMagnetParent = null;
  #endregion

  /// <summary>
  /// Constructor for AbilityUseMagnet
  /// </summary>
  /// <param name="playerMagnet">The magnet associated with this ability's trigger</param>
  /// 
  /// <param name="playerCamera">The camera associated with the player</param>
  ///  
  /// <param name="player">The player associated to this magnet</param>
  public AbilityStickMagnet(PlayerMagnet playerMagnet, Camera playerCamera, PlayerController player)
  {
    _playerMagnet = playerMagnet;
    _playerCamera = playerCamera;
    _player = player;
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
        List<MagneticForce> effectiveMagnets = force.affectingMagnets;
        foreach (MagneticForce otherMagnetForce in effectiveMagnets)
        {
          // In the case, during the magnet's activity, the player collides with a magnet, they stick
          if (_player.magnetColliding && _playerMagnet.charge != otherMagnetForce.charge)
          {
            if (otherMagnetForce.isMoveable)
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

          if (otherMagnetForce.transform.parent.gameObject == _magnetStuckToArm &&
            _playerMagnet.charge == otherMagnetForce.charge)
          {
            ReleaseMagnetStuckToPlayer();
          }
        }
        force.ApplyOtherMagnetsForces(_player.rigidbody);
      }
    }
  }

  private void StickMagnetToPlayer(PlayerController caller, MagneticForce magnet)
  {
    GameObject magnetParent = magnet.transform.parent.gameObject;
    magnetParent.GetComponent<Rigidbody>().isKinematic = true;

    BipolarConsole.EnganaLog(_previousMagnetParent);
    _previousMagnetParent = magnetParent.transform.parent;
    magnetParent.transform.parent = caller.transform.FindChild("Camera");

    magnetParent.transform.localPosition = new Vector3(0, 0, 1.5f);
    _magnetStuckToArm = magnetParent;
  }

  private void ReleaseMagnetStuckToPlayer()
  {
    _magnetStuckToArm.transform.parent = _previousMagnetParent;
    _previousMagnetParent = null;
    _magnetStuckToArm.GetComponent<Rigidbody>().isKinematic = false;
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
    _stuckToMagnet = true;
    //setGrounded();
    caller.rigidbody.velocity = Vector3.zero;
    caller.rigidbody.angularVelocity = Vector3.zero;
    caller.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    caller.transform.parent = otherMagnet.transform;
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
    //setAirborne();
    _stuckToMagnet = false;
    caller.transform.parent = null;
    caller.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
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
