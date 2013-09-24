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
    if (parentToAffect == null)
    {
      base.ApplyOtherMagnetsForces(this.transform.parent.transform.parent.rigidbody);
    }
    else
    {
      base.ApplyOtherMagnetsForces(parentToAffect.rigidbody);
    }

  }

  /// <summary>
  /// Applies the influence other objects have over this one
  /// Overrides the method from the base class in order to induse rotation on the rotary magnet
  /// </summary>
  public override void ApplyForces(Rigidbody magnetBody, MagneticForce otherMagnet, Vector3 hit = default(Vector3))
  {
      if (base.isActivated && otherMagnet.isActivated 
         && otherMagnet.transform.parent.transform.parent != this.transform.parent.transform.parent)  //so they won't affect each other if they belong to the same generator
      {
        Vector3 localForceDirection = this.transform.position - otherMagnet.transform.position;
          
          //this.transform.InverseTransformPoint(otherMagnet.transform.position); // represent's the currect object position on local coordinations
        localForceDirection.Normalize();

        if(otherMagnet.charge == this.charge){
          localForceDirection = (-1) * localForceDirection;
        }

        Vector3 toCenterVector = magnetBody.transform.position - this.transform.position;
        toCenterVector.Normalize();

        Vector3 rotationDir = Vector3.Cross(toCenterVector, localForceDirection);

        float totalForce = getTotalForce(otherMagnet);

        Vector3 localTorque = rotationDir * totalForce * Time.deltaTime;
        //totalForce * (1 - localForceDirection.y) * Time.deltaTime
        //magnetBody.AddForceAtPosition(totalForce * localForceDirection, hit);

        //magnetBody.AddForceAtPosition(totalForce * localForceDirection * Time.deltaTime, hit);

        magnetBody.AddTorque( localTorque); // apply torque only on the local y axis of the rotary magnet
        
      }
  }
}
