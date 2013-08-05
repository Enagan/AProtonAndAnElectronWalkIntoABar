//Owner: Lousada
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//
// WARNING - DO NOT put this script on the magnetic layer 
//
public class PlayerMagnet : MagneticForce {

    // This was not tested yet
    /// <summary>
    /// Fires a raycast that will make a magnetic object have influence over the player if one is hit
    /// Requiers the direction the player/camera is facing 
    /// </summary>
    /// <returns> The Magnetic Force of the object that was hit </returns>
    public MagneticForce FireRayCast(Vector3 direction) {
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, direction, out hit) && hit.collider.CompareTag("Magnet")) {
            Magnet otherMagnet = (Magnet) hit.collider.gameObject.GetComponent("Magnet");
            Transform otherTransform = hit.collider.gameObject.transform;
            MagneticForce otherMagneticForce = (MagneticForce) otherTransform.FindChild("Magnetism").GetComponent("MagneticForce");
           
            otherMagnet.AddMagneticForce(this);
            this.AffectedBy(otherMagneticForce);
            return otherMagneticForce;
            }
        return null; //Maybe change this
    }




    //ToDO: Change this to affect the player rather than the magnet
    public override void ApplyOtherMagnetsForces() {
        foreach (MagneticForce otherMagnet in affectingMagnets) {
            if (isActivated && otherMagnet.isActivated) {

                Vector3 forceDirection = otherMagnet.transform.position - this.transform.position;
                float distance = Vector3.Distance(otherMagnet.transform.position, this.transform.position);
                float forceFactor = 0.0f;
                float totalForce = 0.0f;

                if (otherMagnet.charge == this.charge) {
                    forceDirection = (-1) * forceDirection;
                }

                
                    switch (force) {
                        case Force.LOW:
                            forceFactor = low_force_factor;
                            break;
                        case Force.MEDIUM:
                            forceFactor = medium_force_factor;
                            break;
                        case Force.HIGH:
                            forceFactor = high_force_factor;
                            break;
                        default:
                            //throw exception perhaps
                            break;
                    }
                
                totalForce = (1 / (distance * distance) * forceFactor);
                this.transform.parent.rigidbody.AddForce(totalForce * forceDirection);  //change here
            }

        }

    }
}
