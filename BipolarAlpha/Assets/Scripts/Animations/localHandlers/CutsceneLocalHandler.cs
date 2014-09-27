using UnityEngine;
using System.Collections;

/// <summary>
/// CutsceneLocalHandler is the root class of a particular cutscene
/// Treats player collision and activates cutscene animations depicted in its CutsceneScript
/// It recieves CutsceneElements to be animated by the CutsceneManager via CutsceneTokens
/// </summary>
public class CutsceneLocalHandler : AnimationLocalHandler
{

  #region variables



  // This handler's cutscene camera, used instead of player camera during animation
  private CutsceneCamera _camera;

  // Player camera and audio
  // These references are required for reactivation
  private Camera _playerCam;
  private AudioListener _playerAudio;

  // Whether game is inPlayerCam or cutsceneCam
  private bool _inPlayerCam = true;

  #endregion

  #region properties getters/setters

  #endregion

  #region  monobehavior methods
  protected override void Start()
  {
    base.Start();
    _camera = GetComponentInChildren<CutsceneCamera>();
  }

  #endregion

  #region trigger methods

  override public void triggerMethod(Collider other)
  {
    
    if (_camera != null) // in case of player initializing inside of a collision will call this before initializing kids
    {

      if (other.tag == "Player") // check if collided entity is player
      {
        Debug.Log("Player in cutscene");
        this.GetComponent<Collider>().enabled = false; // stop future collisions


        // get player components 
        _player = other.GetComponent<PlayerController>();

        if (_player != null)
        {
          _playerCam = _player.getPlayerCamera();
          _playerAudio = _player.getPlayerAudioListener();

          // start animation
          if (_playerCam != null && _playerAudio != null)
            switchCamera();
          _script.startAnimation();
        }
      }
    }
  }

  #endregion

  #region animation helpers

  // Alternates between cutsceneCamera and playerCamera each call
  public void switchCamera()
  {
    if (_inPlayerCam)
    {
      // Required to avoid explosions

      _playerCam.enabled = false;//gameObject.SetActive(false);
      _playerAudio.enabled = false;//gameObject.SetActive(false);
      _camera.changeCameraState(true);

      _player.enabled = false;
      _inPlayerCam = false;

    }
    else
    {
      // Required to avoid explosions

      _playerCam.enabled = true;
      _playerAudio.enabled = true;
      _camera.changeCameraState(false);
      

      _player.enabled = true;
      _inPlayerCam = true;
    }
  }

  # endregion
}
