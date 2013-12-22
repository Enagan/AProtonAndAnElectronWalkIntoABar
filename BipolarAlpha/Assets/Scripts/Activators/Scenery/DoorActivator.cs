using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoorActivator : OpenCloseAnimationHelperCircuit
{

  //call initializeHandler
  public override void initializeHandler()
  {   
    //add all children to list
    addChild("BottomLeft");
    addChild("BottomRight");
    addChild("BottomCenter");
    addChild("TopRight");
    addChild("TopLeft");
    addChild("TopCenter");
  }

  /// <summary>
  /// Method that returns each circuit Name, used for debug
  /// </summary>
  public override string CircuitName()
  {
    return "Door_Circuit";
  }
}
