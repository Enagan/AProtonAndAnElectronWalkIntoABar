using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

// PauseLabels generates all available buttons for the Pause Menu
public enum PauseLabels { Resume, Quit };


/// <summary>
/// HUDPausePanel generates the pause menu from PauseLabels enum
/// </summary>
public class HUDPausePanel : HUDPanel {

  #region HUD methods
  
  // Contains buttons in pause menu
  private Dictionary<PauseLabels, HUDPauseButton> _buttons = new Dictionary<PauseLabels,HUDPauseButton>(); 

 //Constructor
  public HUDPausePanel(int priority) : base(priority)
  {
   
    int indx =0;
   
    //set panel skin
    GUISkin mySkin = (GUISkin)Resources.Load("GUI/Skins/PausePanelSkin");

    //generate all buttons from enum
    int totalEnums = Enum.GetNames(typeof(PauseLabels)).Length;
    int verticalBlankSpace = Screen.height - (((totalEnums + 1) * HUDPauseButton.paddingSize) + (totalEnums * (int)mySkin.button.fixedHeight));

    foreach( PauseLabels val in Enum.GetValues(typeof(PauseLabels)))
    {
      HUDPauseButton temp = new HUDPauseButton(priority+1, indx, verticalBlankSpace/2,val);
      _buttons.Add(val,temp);
      addHUDObject(temp);
      indx++;
    }
  }
  #endregion
}
