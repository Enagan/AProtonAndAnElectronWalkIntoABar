using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class VirtualWorldMovePoint : MonoBehaviour
{

    [SerializeField]
    protected VirtualWorldArrivalPoint _arrival;

    void OnTriggerEnter(Collider other)
    {

        other.GetComponent<PlayerController>().PlayerActivation(false);
        if (other.tag == "Player")
        {
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
