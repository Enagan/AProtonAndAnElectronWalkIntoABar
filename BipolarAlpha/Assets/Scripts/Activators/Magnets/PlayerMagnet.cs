//Owner: Lousada
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//
// WARNING - DO NOT put this script on the magnetic layer 
//
public class PlayerMagnet : MagneticForce 
{
  // Boolean required to confirm if the magnet isn't sticking to another magnet, and can be used
  private bool _isAvailable = true;

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
  public MagneticForce FireRayCast(Vector3 direction) 
  {
    RaycastHit hit;
    if (Physics.Raycast(this.transform.position, direction, out hit) && hit.collider.CompareTag("Magnet")) 
    {

      Magnet otherMagnet = (Magnet) hit.collider.gameObject.GetComponent("Magnet");
      Transform otherTransform = hit.collider.gameObject.transform;
      MagneticForce otherMagneticForce = (MagneticForce) otherTransform.FindChild("Magnetism").GetComponent("MagneticForce");

      if (!IsAlreadyAffectedBy(otherMagneticForce))
      {
        otherMagnet.AddMagneticForce(this);
        if (!otherMagneticForce.isMoveable)
        {
          this.AffectedBy(otherMagneticForce);
        }
      }
      return this; // leave handling for the ability
    }
    else
    {
      base.NoLongerAffectingMagnets();
    }
    return null; //Maybe change this
    }
}
