using UnityEngine;
using System.Collections;

/// <summary>
/// Player ability to pause the game, delagates pausing to HUDSystem
/// </summary>
public class AbilityPauseGame : Ability 
{
  public AbilityPauseGame()
  {
    
  }

  /// <summary>
  /// PauseController
  /// </summary>
  public void Use(PlayerController caller, string key = null)
  {
   
  }

  public void KeyUp(string key = null) {
    
    // Calls HUDSystem to do pause/unpause events
    ServiceLocator.GetHUDSystem().pauseEvent();
    
  }

  public void KeyDown(string key = null) {

  }

 
}
