using UnityEngine;
using System.Collections;

public class VirtualWorldEntryPoint : VirtualWorldArrivalPoint
{

    [SerializeField]
    protected VirtualWorldArrivalPoint _arrival;

    PlayerController _originalPlayer = null;

    void Start()
    {
     //   base.Start();

    }

    public override void spawnPlayerCopy(PlayerController player)
    {
        if (_originalPlayer == null)
        {
          _originalPlayer = player;
          _arrival.spawnPlayerCopy(player, false);
        }
        else
        {
            player.swapPlayer(_originalPlayer);
            Destroy(player.gameObject);
            _originalPlayer = null;
        }
    }
}
