using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DispenserAnimationActivator : AnimationActivator
{
  // Initializes openclose children by calling addChild(string anim)
  override public void initializeHandler()
  {
    addChild("TopLid");
    addChild("BottomLid");
  }

  #region Mono Methods

  public override void Start()
  {
    base.Start();
  }
  //Override of activate opens all children
  override public void Activate()
  {
    base.Activate();
    this.transform.GetComponentInChildren<MagnetDispenser>().Activate();
  }

  //Override of deactivate closes all children
  override public void Deactivate()
  {
    base.Deactivate();
  }
  #endregion
  /*
  //call initializeHandler
  public override void initializeHandler()
  {
    //add all children to list
    //addChild("TopLid");
    //addChild("BottomLid");
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
      //base.Activate();
      //this.transform.GetComponentInChildren<MagnetDispenser>().Activate();

  }*/
}
