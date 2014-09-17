using UnityEngine;
using System.Collections;

public class PlayerToggler : MonoBehaviour {

    [SerializeField]
    Spawner _spawner;

    void OnTriggerEnter(Collider other)
    {
       
       other.GetComponent<PlayerController>().PlayerActivation(false);
        if(other.tag == "Player")
        {
            if (_spawner == null)
                return;

            _spawner.spawnPlayer(other.GetComponent<PlayerController>());
        }
    }
}
