// Made by: Lousada
using UnityEngine;
using System.Collections;
/// <summary>
/// Class used on both negative and positive parts of a rotary magnet
/// </summary>
public class RotaryMagnetPart : MagneticForce
{

  public override void Update()
  {
    ApplyOtherMagnetsForces(this.transform.parent.transform.parent.rigidbody);

  }

  /// <summary>
  /// Applies the influence other objects have over this one
  /// Overrides the method from the base class in order to induse rotation on the rotary magnet
  /// </summary>
  public override void ApplyOtherMagnetsForces(Rigidbody magnetBody)
  {
    foreach (MagneticForce otherMagnet in base.affectingMagnets)
    {
      if (base.isActivated && otherMagnet.isActivated)
      {
        Vector3 localForceDirection = this.transform.InverseTransformPoint(otherMagnet.transform.position); // represent's the currect object position on local coordinations
        localForceDirection.Normalize();
     /*   if (otherMagnet.charge != this.charge)
        {
          localForceDirection = (-1) * localForceDirection;
        }*/
        if(otherMagnet.charge == Charge.NEGATIVE){
          localForceDirection = (-1) * localForceDirection;
        }

    
        float totalForce = getTotalForce(otherMagnet);

        float localTorque = totalForce * localForceDirection.z / 1000;

        magnetBody.AddRelativeTorque(0.0f, localTorque, 0.0f); // apply torque only on the local y axis of the rotary magnet
        
      }
    }

  }
}
