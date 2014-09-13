using UnityEngine;
using System.Collections;

public class AwardArmAbility : AwardPlayerAbility {

    [SerializeField]
    private bool _awardLeftArm;

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

      if(_awardLeftArm)
          player.addLeftArmMagnetAbility();
      else
          player.addRightArmMagnetAbility();
  }

}
