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
        Vector3 forceDirection = otherMagnet.transform.position - this.transform.position;
        forceDirection.Normalize();
        if (otherMagnet.charge == this.charge)
        {
          forceDirection = (-1) * forceDirection;
        }
        float totalForce = getTotalForce(otherMagnet);
        BipolarConsole.LousadaLog("force direction is:" + forceDirection);
        magnetBody.AddRelativeTorque(Vector3.up * totalForce );
      }
    }

  }
}
