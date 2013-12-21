using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DispenserActivator : OpenCloseAnimationHelperCircuit
{

  //call initializeHandler
  public override void initializeHandler()
  {
    //add all children to list
    addChild("TopLid");
    addChild("BottomLid");
  }

  /// <summary>
  /// Method that returns each circuit Name, used for debug
  /// </summary>
  public override string CircuitName()
  {
    return "Door_Circuit";
  }

  public override void Activate()
  {
      base.Activate();
      this.transform.GetComponentInChildren<MagnetDispenser>().Activate();

  }
}
