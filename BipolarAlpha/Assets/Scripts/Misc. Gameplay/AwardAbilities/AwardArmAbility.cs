using UnityEngine;
using System.Collections;

enum ArmSideToAward { LeftArm, RightArm}

public class AwardArmAbility : AwardPlayerAbility {

    [SerializeField]
  private ArmSideToAward _awardArm;

  public AwardArmAbility()
  {
    _destroyOnPickup = false;
  }
  /// <summary>
  ///  Called by parent when player collides, awards player the SpikeAbility
  /// </summary>
  public override void awardAbility(PlayerController player)
  {
      SMConsole.Log("Awarding Left Arm Ability" + player,"Player");

      if (_awardArm == ArmSideToAward.LeftArm)
          player.addLeftArmMagnetAbility();
      else
          player.addRightArmMagnetAbility();
  }

}
