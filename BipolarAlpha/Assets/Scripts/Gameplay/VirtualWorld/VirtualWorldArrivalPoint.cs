using UnityEngine;
using System.Collections;

public class VirtualWorldArrivalPoint : MonoBehaviour
{

    protected void Start()
    {
        // For debug
        GetComponent<MeshRenderer>().renderer.enabled = false;

    }

    public virtual void spawnPlayerCopy(PlayerController player)
    {
        spawnPlayerCopy(player, true);
    }

    public void spawnPlayerCopy(PlayerController player, bool destroyPrevious)
    {
          PlayerController spawnedPlayer = (PlayerController)Object.Instantiate(player);
          spawnedPlayer.rigidbody.useGravity = true;
          player.swapPlayer(spawnedPlayer);
          spawnedPlayer.transform.position = transform.position;

          if(destroyPrevious)
          {
              Destroy(player.gameObject);
          }
    }
    
}
