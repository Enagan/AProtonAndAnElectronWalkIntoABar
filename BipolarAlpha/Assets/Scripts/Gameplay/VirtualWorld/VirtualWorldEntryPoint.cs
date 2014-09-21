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
          _originalPlayer.enabled = false; // so player's arms in special camera cannot be seen
          _arrival.spawnPlayerCopy(player, false);
        }
        else
        {
            player.PlayerActivation(false);
            _originalPlayer.enabled = true;
            _originalPlayer.PlayerActivation(true);
            Destroy(player.gameObject);
            _originalPlayer = null;
        }
    }
}
