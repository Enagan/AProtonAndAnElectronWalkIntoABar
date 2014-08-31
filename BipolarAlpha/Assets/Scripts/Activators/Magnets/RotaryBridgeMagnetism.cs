// Made by: Lousada
using UnityEngine;
using System.Collections;
/// <summary>
/// Class used on both parts of a rotary magnet
/// </summary>
public class RotaryBridgeMagnetism : MagneticForce
{

  [SerializeField]
  private float rotationDrag;


  [SerializeField]
  private float rotationBoost;

  public override void Start()
  {
    base.Start();
    rotationDrag = 30000.0f;
    rotationBoost = 10000.0f;
    if (parentToAffect == this.transform.parent.gameObject)
    {
      parentToAffect = this.transform.parent.transform.parent.gameObject;
    }
  }

  /// <summary>
  /// Applies the influence other objects have over this one
  /// Overrides the method from the base class in order to induse rotation on the bridge
  /// </summary>
  public override void ApplyForces(Rigidbody magnetBody, MagneticForce otherMagnet, Vector3 hit = default(Vector3))
  {
    if (base.isActivated && otherMagnet.isActivated)  
    {
      Vector3 localForceDirection = this.transform.position - otherMagnet.transform.position;

      localForceDirection.Normalize();

      if (otherMagnet.charge == this.charge)
      {
        localForceDirection = (-1) * localForceDirection;
      }

      Vector3 toCenterVector = magnetBody.transform.position - this.transform.position;
      toCenterVector.Normalize();

      Vector3 rotationDir = Vector3.Cross(toCenterVector, localForceDirection);

      float totalForce = getTotalForce(otherMagnet);

      Vector3 localTorque = rotationDir * totalForce * Time.deltaTime * 10.0f;

      if (localTorque != null &&
        !float.IsNaN(localTorque.x)
        && !float.IsNaN(localTorque.y)
         && !float.IsNaN(localTorque.z) && rotationDrag != 0)
        parentToAffect.rigidbody.AddTorque(localTorque / rotationDrag * rotationBoost); // apply torque only on the local y axis of the rotary bridge

    }
  }
}
