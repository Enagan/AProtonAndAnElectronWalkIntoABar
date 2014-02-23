using UnityEngine;
using System.Collections;

public class HoopAnimationTrigger : MonoBehaviour {

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Magnet")
        {
            this.transform.parent.GetComponentInChildren<Animation>().Play();
        }
    }
}
