using UnityEngine;
using System.Collections;

public class AwardLeftArmAbility : AwardPlayerAbility {

  public AwardLeftArmAbility()
  {
    _destroyOnPickup = false;
  }
  /// <summary>
  ///  Called by parent when player collides, awards player the SpikeAbility
  /// </summary>
  public override void awardAbility(PlayerController player)
  {
      SMConsole.Log("Awarding Left Arm Ability" + player,"Player");
        player.addLeftArmMagnetAbility();
  }

}
