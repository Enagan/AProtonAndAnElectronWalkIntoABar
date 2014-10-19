using UnityEngine;
using System.Collections;

/// <summary>
/// Awards the player with spike ability when collided with
///  Requires a Collision Volume with onTrigger enabled
/// </summary>
public class AwardSpikeAbility : AwardPlayerAbility 
{

  /// <summary>
  ///  Called by parent when player collides, awards player the SpikeAbility
  /// </summary>
  public override void awardAbility(PlayerController player)
  {
      SMConsole.Log("Awarding Spike Ability","Abillities");
    player.addStickyMagnetAbilities();

  }
}
