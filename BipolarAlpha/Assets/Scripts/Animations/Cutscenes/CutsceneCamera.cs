using UnityEngine;
using System.Collections;

/// <summary>
/// CutsceneCamera is a Cutscene's camera, should be in a CutsceneLocalHandler's Hiearchy
/// Takes care of camera duty during an animation
/// IMPORTANT!!! All cutscene's should end by calling finishAnim() during the cutsceneCamera's animation as an Animation Event
/// Animation Events are added in the Animation Pane by right clicking above the key's line
/// </summary>
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(AudioListener))] 
public class CutsceneCamera : MonoBehaviour
{

  #region private variables

  // The actual camera entity
  private Camera _cutsceneCamera;

  // The camera's audio entity that can conflict with the player's
  private AudioListener _cutsceneAudio;

  #endregion

  #region monobehavior methods

  void Start()
  {
    // Get camera components
    _cutsceneCamera = GetComponent<Camera>();
    _cutsceneAudio = GetComponent<AudioListener>();

    // disable camera on start
    changeCameraState(false);
  }

  #endregion

  #region finishAnim() method <--- VERY IMPORTANT FOR ENDING CUTSCENES

  // This method should be called, as an Animation Event, by the camera's animation when the cutscene ends
  // Animation Events are added in the Animation Pane by right clicking above the key's line
  private void finishAnim()
  {
    BipolarConsole.AllLog("CutsceneEnded");
    //manager finishes animation and returns control to player

    ((CutsceneLocalHandler)transform.parent.GetComponent<CutsceneLocalHandler>()).switchCamera();

  }

  #endregion

  #region methods for swapping (enabling/disabling) camera

  // changes the state of the camera's components
  public void changeCameraState(bool newState)
  {
    _cutsceneAudio.enabled = newState;
    _cutsceneCamera.enabled = newState;

  }

  #endregion


}
