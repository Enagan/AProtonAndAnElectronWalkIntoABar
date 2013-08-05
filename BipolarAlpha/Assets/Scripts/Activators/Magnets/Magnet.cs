//Owner: Lousada
using UnityEngine;
using System.Collections;
/// <summary>
/// The class Magnet is used to make the connection between an object and it's magnetic force
/// All objects that have childs with the MagneticForce script on them must have this script aswell
/// </summary>
public class Magnet : MonoBehaviour{

    //
    // Might be usefull down the line
    //
    /*
    void OnCollisionEnter(Collision col) {
        if(col.collider.CompareTag("Magnet")){
            BipolarConsole.LousadaLog("entering collision");
            Magnet otherMagnet = (Magnet) col.collider.gameObject.GetComponent("Magnet");
            otherMagnet.RemoveMagneticForce( (MagneticForce) this.transform.FindChild("Magnetism").GetComponent("MagneticForce"));
            this.rigidbody.velocity = new Vector3(0, 0, 0);
        }
    }

    
    void OnCollisionExit(Collision col) {
        if (col.collider.CompareTag("Magnet")) {
            BipolarConsole.LousadaLog("exiting collision");
            Magnet otherMagnet = (Magnet)col.collider.gameObject.GetComponent("Magnet");
            otherMagnet.AddMagneticForce((MagneticForce)this.transform.FindChild("Magnetism").GetComponent("MagneticForce"));
        }
    }
    
    public void RemoveMagneticForce(MagneticForce mf) {
        MagneticForce myMagneticForce = (MagneticForce) this.transform.FindChild("Magnetism").GetComponent("MagneticForce");
        myMagneticForce.NoLongerAffectedBy(mf);
    }
    */

    /// <summary>
    /// Function used to add a MagneticForce that will affect this object
    /// </summary>
    public void AddMagneticForce(MagneticForce mf) {
        MagneticForce myMagneticForce = (MagneticForce)this.transform.FindChild("Magnetism").GetComponent("MagneticForce");
        myMagneticForce.AffectedBy(mf);
    }

}
