using UnityEngine;
using System.Collections;

/// <summary>
/// AbilityJump provides vertical propulsion when needed
/// </summary>
public class AbilityScan: Ability {

  private bool _scanning=false;

  /// <summary>
  /// Scans the area sending an Event to all ScanObjets
  /// </summary>
  public void Use(string key = null)
  {    
  }

  public void KeyUp(string key = null) { }

  public void KeyDown(string key = null)
  {
    _scanning = !_scanning;
    ServiceLocator.GetEventHandlerSystem().SendPlayerAbilityScanEvent(_scanning);
  }
}
