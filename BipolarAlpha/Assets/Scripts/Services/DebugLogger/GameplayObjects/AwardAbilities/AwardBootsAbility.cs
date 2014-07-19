using UnityEngine;
using System.Collections;

/// <summary>
/// Awards the player with boots ability when collided with
///  Requires a Collision Volume with onTrigger enabled
/// </summary>
public class AwardBootsAbility : AwardPlayerAbility {

  /// <summary>
  ///  Called by parent when player collides, awards player the MagnetBoots left and right abilities
  /// </summary>
  public override void awardAbility(PlayerController player)
  {
    Debug.Log("Awarding Boots Ability");
    player.AddAbility("Boot1", new AbilityMagnetBoots(GameObject.Find("LeftBootMagnetism").GetComponent<PlayerMagnet>(), player));
    player.AddAbility("Boot2", new AbilityMagnetBoots(GameObject.Find("RightBootMagnetism").GetComponent<PlayerMagnet>(), player));
  }
}
