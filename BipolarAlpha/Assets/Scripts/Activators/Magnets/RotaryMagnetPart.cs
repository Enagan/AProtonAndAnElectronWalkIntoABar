using UnityEngine;
using System.Collections;

public class RotaryMagnetPart : MagneticForce
{

  public override void Update()
  {
    ApplyOtherMagnetsForces(this.transform.parent.transform.parent.rigidbody);

  }

  /// <summary>
  /// Applies the influence other objects have over this one
  /// </summary>
  public override void ApplyOtherMagnetsForces(Rigidbody magnetBody)
  {
    foreach (MagneticForce otherMagnet in base.affectingMagnets)
    {
      if (base.isActivated && otherMagnet.isActivated)
      {
        Vector3 localForceDirection = this.transform.InverseTransformPoint(otherMagnet.transform.position);
        localForceDirection.Normalize();
        if (otherMagnet.charge == this.charge)
        {
          localForceDirection = (-1) * localForceDirection;
        }
        float totalForce = getTotalForce(otherMagnet);

        BipolarConsole.LousadaLog("local direction is: " + localForceDirection);

        #warning not working
        magnetBody.AddRelativeTorque(transform.up * totalForce * localForceDirection.y); // apply torque only on the local y axis of the rotary magnet
        
      }
    }

  }
}
