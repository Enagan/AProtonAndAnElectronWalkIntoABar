using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(AudioListener))] 
public class CutsceneCamera : MonoBehaviour {

  private Camera _cutsceneCamera;
  private AudioListener _cutsceneAudio;

  private

  	// Use this for initialization
  void Start()
  {
    _cutsceneCamera = GetComponent<Camera>();
    _cutsceneAudio = GetComponent<AudioListener>();
    changeCameraState(false);
  }

  private void finishAnim()
  {
    BipolarConsole.AllLog("CutsceneEnded");
    //manager finishes animation and returns control to player

    ((CutsceneLocalHandler)transform.parent.GetComponent<CutsceneLocalHandler>()).switchCamera();

  }


  public void changeCameraState(bool newState)
  {
    _cutsceneAudio.enabled = newState;
    _cutsceneCamera.enabled = newState;
    
  }




}
