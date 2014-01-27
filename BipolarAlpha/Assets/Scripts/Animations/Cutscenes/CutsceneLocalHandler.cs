using UnityEngine;
using System.Collections;

/// <summary>
/// CutsceneLocalHandler is the root class of a particular cutscene
/// Treats player collision and activates cutscene animations depicted in its CutsceneScript
/// It recieves CutsceneElements to be animated by the CutsceneManager via CutsceneTokens
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(CutsceneScript))]
public class CutsceneLocalHandler : MonoBehaviour, IHasComplexState
{

  #region private variables

  // Name of this particular cutscene, variable is stored in complex state
  [SerializeField]
  private string _cutsceneName ="";

  // Radius of Sphere Collider that treates player collision, stored in complex state
  [SerializeField]
  private float _collisionRadius = 0.5f;

  // This handler's cutscene camera, used instead of player camera during animation
  private CutsceneCamera _camera;

  // The cutscene's "script" handles all animation tasks
  // Created by recieving registered CutsceneElements via CutsceneManager
  private CutsceneScript _script;

  // Player reference stored upon collision for enabling/disabling
  private PlayerController _player;

  // Player camera and audio
  // These references are required for reactivation
  private Camera _playerCam;
  private AudioListener _playerAudio;

  // Whether game is inPlayerCam or cutsceneCam
  private bool _inPlayerCam = true;

  #endregion

  #region properties getters/setters

  public string cutsceneName
  {
    get
    {
      return _cutsceneName;
    }

    set
    {
      _cutsceneName = value;
    }

  }

  #endregion

  #region  monobehavior methods
  void Start()
  {
    _camera = GetComponentInChildren<CutsceneCamera>();
    _script = GetComponent<CutsceneScript>();

    
    this.GetComponent<SphereCollider>().radius = _collisionRadius; // set radius from serialize/complexState

    ServiceLocator.GetCutsceneManager().registerCutsceneHandler(this); // register self in cutsceneManager
  }

  #endregion

  #region trigger methods

  // checks if player IS inside handler's collider
  void OnTriggerStay(Collider other)
  {
    OnTriggerEnter(other);
  }

  // checks if player collided with handler's collider and calls animation
  void OnTriggerEnter (Collider other)
  {
    if (_camera != null) // in case of player initializing inside of a collision will call this before initializing kids
    {
      
      if (other.tag == "Player") // check if collided entity is player
      {
        BipolarConsole.AllLog("Player in cutscene");
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

  #region Register CutsceneTokens in script methods

  public void addElement(CutsceneToken token)
  {
    _script.addAnimation(token);
  }

  #endregion

  #region Complex State methods

  // Complex state write
  public ComplexState WriteComplexState()
  {
    // saves this instance cutsceneName
    CutsceneLocalHandlerComplexState state = new CutsceneLocalHandlerComplexState(this.gameObject);
    state.cutsceneName = _cutsceneName;
    state.collisionRadius = _collisionRadius;

    return state;
  }

  public void LoadComplexState(ComplexState state)
  {
    if (!(state is CutsceneLocalHandlerComplexState))
    {
      throw new BipolarExceptionComplexStateNotCompatibleWithScript(state.GetComplexStateName() + " is not compatible with CutsceneLocalHandler class");
    }

    CutsceneLocalHandlerComplexState handlerState = ((CutsceneLocalHandlerComplexState)state);
    
    // loads this instance cutsceneName
    _cutsceneName = handlerState.cutsceneName;
    _collisionRadius = handlerState.collisionRadius;

    
  }

  public ComplexState UpdateComplexState(ComplexState state)
  {
    if (!(state is CutsceneLocalHandlerComplexState))
    {
      throw new BipolarExceptionComplexStateNotCompatibleWithScript("Complex State " + state.GetComplexStateName() + " cannot be updated in CutsceneLocalHandler Class");
    }
    CutsceneLocalHandlerComplexState specificState = (state as CutsceneLocalHandlerComplexState);
    specificState.cutsceneName = _cutsceneName;
    specificState.collisionRadius = _collisionRadius;

    return specificState;
  }


  #endregion
}
