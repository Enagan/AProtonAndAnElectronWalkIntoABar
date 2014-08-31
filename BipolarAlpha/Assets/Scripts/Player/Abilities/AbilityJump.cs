using UnityEngine;
using System.Collections;

/// <summary>
/// AbilityJump provides vertical propulsion when needed
/// </summary>
public class AbilityJump : Ability {

  private float _jumpStrength = 50f;

  private PlayerController _player;

  /// <summary>
  /// JUMP!
  /// </summary>
  public AbilityJump(PlayerController player) 
  {
    _player = player;
  }
  public void Use(string key = null)
  {
    //Only jump if the caller isn't already in the air
    if (!_player.airborne)
    {
      _player.rigidbody.AddForce(Vector3.up * _jumpStrength * Time.deltaTime, ForceMode.VelocityChange);
    }
  }

  public void KeyUp(string key = null) { }

  public void KeyDown(string key = null) { }
}
