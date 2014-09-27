//Made By: Ivo, Lousada
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// HUDPauseButton generates an instance of a button for the PauseMenu but contains code for all possible outcomes due to refractoring
/// </summary>
public class HUDPauseButton : HUDObject {


  #region private variables
  // Text style elements

  //labels for available buttons, to add more buttons add label here and enum in HUDPausePanel
  #pragma warning disable 414
  private static string[] labels = { "Resume Game", "Quit" };

  // Style for buttons
  private GUIStyle _textStyle;
  private PauseLabels _buttonType;

  //Helpers for placement
  private int _buttonHeight;
  private static int _paddingSize= 5;
  
  public static int paddingSize
  {
    get { return _paddingSize; }

  }


  #endregion

  #region HUD methods
	
  
  // Constructor
  public HUDPauseButton(int priority, int posIndx, int startPos, PauseLabels label)
    : base(priority)
  {
    _buttonType = label;

    GUISkin mySkin = (GUISkin)Resources.Load("GUI/Skins/PausePanelSkin");
    _textStyle = mySkin.button;

    _buttonHeight = startPos + posIndx *(_paddingSize+ (int)(_textStyle.fixedHeight));

  }
	
  // Draw of button
  public override void DrawHUD()
  {
    //Check if being clicked
    if (GUI.Button(new Rect(Screen.width / 2 - _textStyle.fixedWidth/2, _buttonHeight, _textStyle.fixedWidth, _textStyle.fixedHeight), GetUIButtonString(_buttonType), _textStyle))
    {
      // Check kind of transition
      switch (_buttonType)
      {
        case PauseLabels.Resume:  //Resume Button was pressed
          ServiceLocator.GetEventHandlerSystem().SendPauseEvent(false);
          break;
        case PauseLabels.Quit: // Quit Button was pressed
          Application.LoadLevel(0); // Loads main menu
          break;
      }
    }
  }
	
  #endregion

  #region Pause Button Methods

  public string GetUIButtonString(PauseLabels label)
  {
    switch (label)
    {
      case PauseLabels.Resume: 
        return "Resume Game";
      
      case PauseLabels.Quit: 
        return "Quit to Title";
      default:
        return "BAD_LABEL";
    }
  }

  #endregion

}
