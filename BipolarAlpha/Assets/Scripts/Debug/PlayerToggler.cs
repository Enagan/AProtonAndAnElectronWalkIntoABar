using UnityEngine;
using System.Collections;

public class PlayerToggler : MonoBehaviour {

    [SerializeField]
    Spawner _spawner;
    bool toggled = false;

    void OnTriggerEnter(Collider other)
    {
       
       other.GetComponent<PlayerController>().PlayerActivation(false);
        if(other.tag == "Player")
        {
            if (_spawner == null)
                return;

            if (!toggled)
                _spawner.spawnPlayer(other.GetComponent<PlayerController>());
            else
                _spawner.returnPlayer();

            toggled = !toggled;
        }
    }
}
