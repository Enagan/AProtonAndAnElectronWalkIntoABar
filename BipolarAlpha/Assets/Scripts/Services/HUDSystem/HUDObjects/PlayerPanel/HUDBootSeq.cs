using UnityEngine;
using System.Collections;

public class HUDBootSeq : HUDObject{

    #region private variables
    // Text style elements
    private GUIStyle BIOSStyle;
    Texture2D textureBlack;

    // Current and stored tooltips
    private string displayText = "";
    private string fullText = "MAGBOT9000 Control System v4.85.6.1 [Feb 24 2045] \n\n\n\nBooting...\n\n\n\nConsulting WebTimeServer for time update...[Connection failed] \n\nUnable to determine current year\n\n\n\nMAGBOT9000 was not shut down properly. Initiating full systems check.\n\n\n\nHUD System...[OK]\n\nHand Magnet System...[OK]\n\nArm Control System...[OK]\n\nLocomotor System...[OK]\n\n\n\nAll Systems OK\n\n\n\nActivating HUD";
    private Rect texturePosition;

    private bool onBootSeq;
    private float characterDelay = 0.005f;

    private float nextCharacterTimer;
    private Rect textArea;

    #endregion

	  #region HUD methods
	
  // Constructor
    public HUDBootSeq(int priority) : base(priority)
  {
      textureBlack = new Texture2D(1, 1);
      textureBlack.SetPixel(0, 0, Color.black);
      textureBlack.Apply();

      texturePosition = new Rect(0, 0, Screen.width , Screen.height);

      textArea = new Rect(Screen.width / 80, Screen.height / 50, Screen.width - Screen.width / 20, Screen.height - Screen.height / 20);

      onBootSeq = true;
      nextCharacterTimer = Time.time + characterDelay;

      // Set text style

      GUISkin mySkin = (GUISkin)Resources.Load("GUI/Skins/BootSequence");
      BIOSStyle = mySkin.label;

  }
	
  public override void DrawHUD()
  {
      if (onBootSeq)
      {
          TextAnimation();

          GUI.DrawTexture(texturePosition, textureBlack);

          GUI.Label(textArea, displayText, BIOSStyle);

      }

  }
	
  #endregion

  #region Auxiliary Functions

  void TextAnimation()
  {
      if (fullText.Length > 0)
      {

          if (Time.time > nextCharacterTimer)
          {
              if (fullText.StartsWith("\n\n") || fullText.StartsWith(".["))
              {
                  nextCharacterTimer = Time.time + characterDelay * 100;

              }
              else
              {
                  nextCharacterTimer = Time.time + characterDelay;
              }
              displayText += fullText.Substring(0,1);
              fullText = fullText.Remove(0,1);
          }
      }
      else
      {
          onBootSeq = false;
          Application.LoadLevel("Main");

      }
  }
    
  #endregion

}
