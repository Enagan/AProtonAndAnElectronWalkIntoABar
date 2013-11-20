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
		
   GUISkin mySkin = (GUISkin) Resources.Load("GUI/Skins/PlayerPanelSkin");
   _commandLineStyle = mySkin.box;
		
	
  }
	
  public override void DrawHUD()
  {
    if (_currentlyDisplayedTip != null)
    {
      float height = 25 + 25*(_currentlyDisplayedTip.Length / 40);
      GUI.Label(new Rect(Screen.width/2-_commandLineStyle.fixedWidth/2, 0, 300, height), _currentlyDisplayedTip, _commandLineStyle);
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
    _currentlyDisplayedTip = "> Advisor Subroutine:\n" +"> "+ tutorialMessage;
    _alreadyPresentedTips.Add(tutorialMessage);
  }
  #endregion
}
