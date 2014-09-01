using UnityEngine;
using System.Collections;

public class AwardSystemKey : AwardPlayerAbility
{
  public SecurityLevels levelGrantedByKey;

  public AwardSystemKey()
  {
    _destroyOnPickup = true;
  }
  /// <summary>
  ///  Called by parent when player collides, awards player the System Key
  /// </summary>
  public override void awardAbility(PlayerController player)
  {
    SMConsole.Log("Awarding System Key" + levelGrantedByKey, " to Player");
      player.playerSecurityProtocol = levelGrantedByKey;
  }
}
