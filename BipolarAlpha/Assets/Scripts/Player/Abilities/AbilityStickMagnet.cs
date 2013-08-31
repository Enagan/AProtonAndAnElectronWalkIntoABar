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
    if (key == "Release1")
    {
      // Condition to ensure the player is not releasing himself from a static (non-moveable) magnet,
      // sticking to the player's other magnet
      if (_player.leftStickedToMagnet)
      {
        _player.FreeFromMagnet();
      }
      _player.ReleaseMagnetStickingToLeft();
    }
    if (key == "Release2")
    {
      // Condition to ensure the player is not releasing himself from a static (non-moveable) magnet,
      // sticking to the player's other magnet
      if (!_player.leftStickedToMagnet)
      {
        _player.FreeFromMagnet();
      }
      _player.ReleaseMagnetStickingToRight();
    }
    if (((key == "Fire1") || (key == "Fire2")) && _playerMagnet.isAvailable)
    {
      //Does this every Update when pressing the ability button
      MagneticForce force = _playerMagnet.FireRayCast(_playerCamera.transform.position, _playerCamera.transform.forward);

      if (force != null)
      {
        force.ApplyOtherMagnetsForces(_player.rigidbody);
        List<MagneticForce> effectiveMagnets = force.affectingMagnets;
        foreach (MagneticForce otherMagnetForce in effectiveMagnets)
        {
          float distance = Vector3.Distance(force.transform.position, otherMagnetForce.transform.position);
          // In the case, during the magnet's activity, the player collides with a magnet, they stick
          if (distance < 1f || _player.magnetColliding)
          {
            if (otherMagnetForce.isMoveable)
            { 
              if (key == "Fire1")
              {
                _player.MagnetSticksToLeft(otherMagnetForce);
              }
              else if (key == "Fire2")
              {
                _player.MagnetSticksToRight(otherMagnetForce);
              }
            }
            else
            {
              _player.StickToMagnet(otherMagnetForce);
              if (key == "Fire1")
              {
                _player.SetLeftStickedToMagnet();
              }
            }
            // Deactives the player magnet while it's sticking to another magnet
            _playerMagnet.isActivated = false;
            _playerMagnet.isAvailable = false;
            break;
          }
        }
      }
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
