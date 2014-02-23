//Made By: Ivo, Lousada
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// HUDAdvisorRoutine handles rendering a text tooltip on screen
/// </summary>
public class HUDAdvisorRoutine : HUDObject, IPlayerRoomChangeListner {

  #region private variables
  // Text style elements
  private GUIStyle _commandLineStyle;
  private GUIStyle _BIOSStyle;

  private string fullText = "";
  private float characterDelay = 0.01f;
  private float nextCharacterTimer = 0.0f;
  private string _currentlyDisplayedTip = "";

  #endregion

  #region HUD methods
	
  // Constructor
  public HUDAdvisorRoutine(int priority) : base(priority)
  {
  
   // Register self as Listener
   ServiceLocator.GetEventHandlerSystem().RegisterPlayerRoomChangeListner(this);
		
   // Set text style
		
   GUISkin mySkin = (GUISkin) Resources.Load("GUI/Skins/PlayerPanelSkin");
   _commandLineStyle = mySkin.box;


   GUISkin mySkin2 = (GUISkin)Resources.Load("GUI/Skins/BootSequence");
   _BIOSStyle = mySkin2.label;
	
  }
	
  public override void DrawHUD()
  {
    if (fullText.Length > 0 )
    {
       TextAnimation();
      float height = 25 + 25*(_currentlyDisplayedTip.Length / 40);
      GUI.Label(new Rect(Screen.width/2-_commandLineStyle.fixedWidth/2, 0, 400, height / 2), " ", _commandLineStyle);
      GUI.Label(new Rect(Screen.width / 2 - _commandLineStyle.fixedWidth / 2 + 5, 5, 900, height), _currentlyDisplayedTip, _BIOSStyle);

    }
  }

  void TextAnimation()
  {
      if (fullText.Length > 0)
      {
          if (Time.time > nextCharacterTimer)
          {
              if (fullText.StartsWith("\n"))
              {
                  nextCharacterTimer = Time.time + characterDelay * 150;
              }
              else 
              if (fullText.StartsWith("\t"))
              {
                  nextCharacterTimer = Time.time + characterDelay * 300;
              }
              else
              {
                  nextCharacterTimer = Time.time + characterDelay;
              }
              _currentlyDisplayedTip += fullText.Substring(0, 1);
              fullText = fullText.Remove(0, 1);
          }
      }
  }
  #endregion

	
  #region EventListner interface methods

  public void ListenPlayerRoomChange(string newRoomName)
  {
      fullText = "";
      if (newRoomName == "SPAAAACE")
      {
          fullText = "** MAGBOT9000 report immediately to the Quality Assurance Facility **\t\t\n";        
      }

      if (newRoomName == "DisposalElevator")
      {
          fullText = "The elevator seems to be out of power \n\nMaybe there is a way to activate it\t\n";
      }

      if (newRoomName == "QA-UNIT02Test1")
      {
          fullText = "You will now begin test 1 of Quality Assurance\t\n";
      }

      if (newRoomName == "QA-UNIT02Test2")
      {
          fullText = "You will now begin test 2 of Quality Assurance\t\n";
      }

      if (newRoomName == "QA-UNIT02Test3")
      {
          fullText = "You will now begin test 3 of Quality Assurance\t\n";
      }

      _currentlyDisplayedTip = "";
      nextCharacterTimer = Time.time + characterDelay * 150;
      TextAnimation();
  }


   /*  Before the change to a bios like stuff
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
    */
  #endregion
     
}
