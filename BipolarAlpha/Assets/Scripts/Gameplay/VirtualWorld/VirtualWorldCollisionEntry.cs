using UnityEngine;
using System.Collections;

[RequireComponent(typeof(VirtualWorldEntryPoint))]
[RequireComponent(typeof(Collider))]
public class VirtualWorldCollisionEntry : MonoBehaviour
{


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            gameObject.GetComponent<VirtualWorldEntryPoint>().spawnPlayerCopy(other.GetComponent<PlayerController>());
        }
    }

}
