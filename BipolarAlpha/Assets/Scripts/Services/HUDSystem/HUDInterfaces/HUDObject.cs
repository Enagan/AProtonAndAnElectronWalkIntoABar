using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// HUDObject is a leaf element of the HUD
/// All visible HUD elements should inherit from HUDObject
/// Rendering code needs be made in the DrawHUD method and overriden by child classes
/// </summary>
public abstract class HUDObject
{
  
  #region private variables
	
  // Priority of the Object, used to calculate overlaping order
  protected int _priority=-1;
	
  #endregion
  
  #region HUDObject methods
  //DrawHUD needs to be overriden and contain GUI calls to render on the screen
  abstract public void DrawHUD();
  
  protected HUDObject(int priority)
  {
    _priority = priority;
  }

  public int getPriority()
  {
    return _priority;
  }

  #endregion

}
