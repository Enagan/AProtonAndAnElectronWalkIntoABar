using UnityEngine;
using System.Collections;

public class RailgunChangeScene : MonoBehaviour {


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Application.LoadLevel("CutsceneSpaaaace");
        }
    }
}
