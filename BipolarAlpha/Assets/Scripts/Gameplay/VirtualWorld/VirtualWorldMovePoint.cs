using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class VirtualWorldMovePoint : MonoBehaviour
{

    [SerializeField]
    protected VirtualWorldArrivalPoint _arrival;

    void OnTriggerEnter(Collider other)
    {
        
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerController>().PlayerActivation(false);
            movePlayer(other.GetComponent <PlayerController>());
        }
    }

    protected void movePlayer(PlayerController player)
    {
        if (_arrival == null || player == null)
            return;

        _arrival.spawnPlayerCopy(player);
    }

    
}
