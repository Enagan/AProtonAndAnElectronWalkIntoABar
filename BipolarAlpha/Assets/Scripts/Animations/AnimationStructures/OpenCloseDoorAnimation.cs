using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This is a class to help call animations on circuit structures that open/close
/// Examples: Door, Dispenser
/// </summary>
public class OpenCloseDoorAnimation : AnimationActivator
{
  // Initializes openclose children by calling addChild(string anim)
  override public void initializeHandler()
  {
    addChild("BottomLeft");
    addChild("BottomRight");
    addChild("BottomCenter");
    addChild("TopRight");
    addChild("TopLeft");
    addChild("TopCenter");
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
  }

  //Override of deactivate closes all children
  override public void Deactivate()
  {
    base.Deactivate();
  }

  #endregion
}
