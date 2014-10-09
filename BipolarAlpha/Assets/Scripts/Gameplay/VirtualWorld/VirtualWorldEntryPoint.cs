using UnityEngine;
using System.Collections;

public class VirtualWorldEntryPoint : VirtualWorldArrivalPoint
{

    [SerializeField]
    Material _virtualSkyBox;

    Material _originalSkyBox;

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
           _originalSkyBox = RenderSettings.skybox;
           RenderSettings.skybox = _virtualSkyBox;
          _originalPlayer = player;
          _arrival.spawnPlayerCopy(player, false);
        }
        else
        {
            RenderSettings.skybox = _originalSkyBox;
            _originalSkyBox = null;
            player.swapPlayer(_originalPlayer);
            Destroy(player.gameObject);
            _originalPlayer = null;
        }
    }
}
