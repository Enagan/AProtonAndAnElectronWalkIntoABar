//Made By: Lousada
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//
// WARNING - DO NOT put this script on the magnetic layer 
//
public class PlayerMagnet : MagneticForce 
{
  // Boolean required to confirm if the magnet isn't sticking to another magnet, and can be used
  private Vector3 currentHitPoint = default(Vector3);
  private bool _isAvailable = true;
  private int _raycastMask = ~(1 << 8);  //Ignore objects in layer 8 (Magnetic Force)


  public bool isAvailable
  {
    get
    {
      return _isAvailable;
    }
    set
    {
      _isAvailable = value;
    }
  }

  /// <summary>
  /// Fires a raycast that will make a magnetic object have influence over the player if one is hit
  /// Requiers the direction the player/camera is facing 
  /// </summary>
  /// <returns> The Magnetic Force of the object that was hit </returns>
  public MagneticForce FireRayCast(Vector3 start, Vector3 direction) 
  {
    RaycastHit hit;
    if (Physics.Raycast(start, direction, out hit, Mathf.Infinity, _raycastMask) && hit.collider.CompareTag("Magnet"))  
    {
      Transform otherTransform = hit.collider.gameObject.transform;
      MagneticForce otherMagneticForce = (MagneticForce) otherTransform.FindChild("Magnetism").GetComponent("MagneticForce");

      if (!IsAlreadyAffectedBy(otherMagneticForce) )
      {
        currentHitPoint = hit.point;
        otherMagneticForce.AffectedBy(this);
        this.AffectedBy(otherMagneticForce);       
      }
      return this; // leave handling for the ability
    }
    else
    {
      base.NoLongerAffectingMagnets();
    }

    return null; //Maybe change this
  }

  /// <summary>
  /// Checks if magnets will influence each other
  /// </summary>
  public override void ApplyOtherMagnetsForces(Rigidbody magnetBody)
  {
    foreach (MagneticForce otherMagnet in base.affectingMagnets)
    {
      if (base.isActivated && otherMagnet.isActivated && !otherMagnet.isMoveable)
      {
        base.ApplyForces(magnetBody, otherMagnet, currentHitPoint);
      }
    }
  }

}
