//Made by: Lousada

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class used to show the head-up display
/// </summary>
public class HUD : MonoBehaviour, IPlayerRoomChangeListner, ITutorialMessageTriggerListener
{
  [SerializeField]
  private Texture2D _crosshairNormal;   //texture for the standard crosshair
  [SerializeField]
  private Texture2D _crosshairHighlight; // texture used when a player is pointing to a magnet

  private GUIStyle _commandLineStyle;
  [SerializeField]
  private Font _commandLineFont;

  private GameObject _camera;
  private Rect _crosshairPosition;
  private int _raycastMask = ~(1 << 8); //Ignore objects in layer 8 (Magnetic Force)
  private float _textureWidth;
  private float _textureHeight;


  private string _currentlyDisplayedTip = null;
  private List<string> _alreadyPresentedTips = new List<string>();



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

  /// <summary>
  /// Fires a ray to determine if a player is pointing at a magnet or not
  /// </summary>
  private bool fireRaycast()
  {
    RaycastHit hit;
    if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit, Mathf.Infinity, _raycastMask) && hit.collider.CompareTag("Magnet"))
    {
      return hit.collider.gameObject.GetComponentInChildren<MagneticForce>().isActivated;
    }
    return false;
  }



  // Use this for initialization
  void Start()
  {
    _camera = GameObject.Find("Camera");
    _textureWidth = _crosshairNormal.width;
    _textureHeight = _crosshairNormal.height;
    //positions the crosshair texture on the center of the screen
    _crosshairPosition = new Rect(Screen.width - _textureWidth,  
                             Screen.height - _textureHeight,
                             _textureWidth,
                             _textureHeight);

    ServiceLocator.GetEventHandlerSystem().RegisterPlayerRoomChangeListner(this);
    ServiceLocator.GetEventHandlerSystem().RegisterTutorialMessageTriggerListener(this);

    //Starting command line message
    _currentlyDisplayedTip = "> MagOS restored sucessfully\n" +
                             "> Situational Awareness Module Activated\n" +
                             "> Previous Directive Restored: \n" +
                             "      Pass Quality Assurance\n" +
                             "\n" +
                             "> Current Step: Exit Recycling Container\n" +
                             "\n" +
                             "> Positive Directional Magnet Driver Corrupted\n" +
                             "> ...\n" +
                             "> ...\n" +
                             "> ...\n" +
                             "> Positive Directional Magnet Driver Restored:\n" +
                             "> Advisor Subroutine:\n" +
                             "      Use left mouse button to activate\n" +
                             "      your directional magnet.\n" +
                             "      Point it at negative charged magnets (blue)\n" +
                             "      For rapid magnetic approximation\n" +
                             "\n(press \"z\" to dismiss command line)";

    _commandLineStyle = new GUIStyle();
    _commandLineStyle.font = _commandLineFont;
    _commandLineStyle.normal.textColor = Color.green;
    _commandLineStyle.border = new RectOffset(10, 10, 10, 10);
  }

  void Update()
  {
    //positions the crosshair texture on the center of the screen
    _crosshairPosition.Set((Screen.width - _textureWidth / 4) / 2,       
                           (Screen.height - _textureHeight / 4) / 2,
                           _textureWidth / 4,
                           _textureHeight / 4);

    //Dismisses current message
    if (Input.GetButtonDown("DismissUIMessage"))
    {
      _currentlyDisplayedTip = null;
    }
  }

  void OnGUI()
  {
    if (fireRaycast())
    {
      GUI.DrawTexture(_crosshairPosition, _crosshairHighlight, ScaleMode.ScaleToFit);
    }
    else
    {
      GUI.DrawTexture(_crosshairPosition, _crosshairNormal, ScaleMode.ScaleToFit);
    }

    if (_currentlyDisplayedTip != null)
    {
      float height = 25 + 25*(_currentlyDisplayedTip.Length / 40);
      GUI.TextField(new Rect(10, Screen.height - 10 - height, 300, height), _currentlyDisplayedTip/*, _commandLineStyle*/);
    }
  }
}
