using UnityEngine;
using System.Collections;

public class JackedInDoorUnlockerController : JackedInRemoteController
{
  public override void Left()
  {
    PlayerController player = ServiceLocator.GetPlayerController();
    if (player)
      foreach (GameObject obj in _activatableObjects)
      {
        obj.GetComponent<SecureProximityDoor>().unlockDoor(player.playerSecurityProtocol);
      }
  }

  public override void Right()
  {
    PlayerController player = ServiceLocator.GetPlayerController();
    if(player)
      foreach (GameObject obj in _activatableObjects)
      {
        obj.GetComponent<SecureProximityDoor>().lockDoor(player.playerSecurityProtocol);
      }
  }
}