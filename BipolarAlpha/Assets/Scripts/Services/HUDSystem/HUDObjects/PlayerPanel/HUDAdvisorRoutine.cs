//Made By: Ivo, Lousada
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// HUDAdvisorRoutine handles rendering a text tooltip on screen
/// </summary>
public class HUDAdvisorRoutine : HUDObject, IPlayerRoomChangeListner, ITutorialMessageTriggerListener {

  #region private variables
  // Text style elements
  private GUIStyle _commandLineStyle;
  private Font _commandLineFont;

  // Current and stored tooltips
  private string _currentlyDisplayedTip = null;
  private List<string> _alreadyPresentedTips = new List<string>();
  #endregion

  #region HUD methods
	
  // Constructor
  public HUDAdvisorRoutine(int priority) : base(priority)
  {
   // Register self as Listener
   ServiceLocator.GetEventHandlerSystem().RegisterPlayerRoomChangeListner(this);
   ServiceLocator.GetEventHandlerSystem().RegisterTutorialMessageTriggerListener(this);
		
   // Set text style
   _commandLineStyle = new GUIStyle();
   _commandLineStyle.font = _commandLineFont;
   _commandLineStyle.normal.textColor = Color.white;
   _commandLineStyle.border = new RectOffset(10, 10, 10, 10);
  }
	
  public override void DrawHUD()
  {
    if (_currentlyDisplayedTip != null)
    {
      float height = 25 + 25*(_currentlyDisplayedTip.Length / 40);
      GUI.Label(new Rect(Screen.width/2 - 150, Screen.height - 10 - height, 300, height), _currentlyDisplayedTip, _commandLineStyle);
    }
  }
	
  #endregion
	
  #region EventListner interface methods
  public void ListenPlayerRoomChange(string newRoomName)
  {
    _currentlyDisplayedTip = "> Positioning System - Current Location:\n    " + newRoomName + "\n(press \"z\" to dismiss command line)";
  }

  public void ListenTutorialMessageTrigger(string tutorialMessage)
  {
    if (_alreadyPresentedTips.Contains(tutorialMessage))
    {
      return;
    }
    _currentlyDisplayedTip = "> Advisor Subroutine:\n  " + tutorialMessage + "\n(press \"z\" to dismiss command line)";
    _alreadyPresentedTips.Add(tutorialMessage);
  }
  #endregion
}
