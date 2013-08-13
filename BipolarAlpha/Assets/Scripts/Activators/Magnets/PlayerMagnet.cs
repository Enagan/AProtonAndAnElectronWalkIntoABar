//Owner: Lousada
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//
// WARNING - DO NOT put this script on the magnetic layer 
//
public class PlayerMagnet : MagneticForce 
{
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
      
      otherMagnet.AddMagneticForce(this);
      this.AffectedBy(otherMagneticForce);
      return this; // leave handling for the ability
    }
    return null; //Maybe change this
    }
}
