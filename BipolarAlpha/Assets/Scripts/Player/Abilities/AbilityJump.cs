using UnityEngine;
using System.Collections;

/// <summary>
/// AbilityJump provides vertical propulsion when needed
/// </summary>
public class AbilityJump : Ability {

  private float _jumpStrength = 50f;

  /// <summary>
  /// JUMP!
  /// </summary>
  public void Use(PlayerController caller)
  {
    //Only jump if the caller isn't already in the air
    if (!caller.airborne)
    {
      caller.rigidbody.AddForce(Vector3.up * _jumpStrength * Time.deltaTime, ForceMode.VelocityChange);
    }
  }

}
