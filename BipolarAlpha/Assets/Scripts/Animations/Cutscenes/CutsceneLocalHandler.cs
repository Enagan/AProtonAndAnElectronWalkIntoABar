using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(CutsceneScript))]
public class CutsceneLocalHandler : MonoBehaviour, IHasComplexState {

  [SerializeField]
  private string _cutsceneName ="";

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

  private CutsceneCamera _camera;

  private CutsceneScript _script;

  private PlayerController _player;

  // These references are required for reactivation
  private Camera _playerCam;
  private AudioListener _playerAudio;

  private bool _inPlayerCam = true;

  void Start()
  {
    _camera = GetComponentInChildren<CutsceneCamera>();
    _script = GetComponent<CutsceneScript>();

    ServiceLocator.GetCutsceneManager().registerCutsceneHandler(this); // register self in cutsceneManager
  }

  void OnTriggerStay(Collider other)
  {
    OnTriggerEnter(other);
  }

  void OnTriggerEnter (Collider other)
  {
    if (_camera != null) // in case of player initializing inside of a collision will call this before initializing kids
    {
      BipolarConsole.AllLog("Collison");
      if (other.tag == "Player")
      {
        BipolarConsole.AllLog("Player in cutscene");
        this.GetComponent<Collider>().enabled = false;

        _player = other.GetComponent<PlayerController>();

        if (_player != null)
        {
          _playerCam = _player.getPlayerCamera();
          _playerAudio = _player.getPlayerAudioListener();
          if (_playerCam != null && _playerAudio != null)
            switchCamera();
          //_camera.GetComponent<AnimationRootHandler>().playAnimation("CutsceneTest");
          _script.startAnimation();
        }
      }
    }

  } 

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

  public void addElement(CutsceneToken token)
  {
    _script.addAnimation(token);
  }

  #region Complex State methods

  // Complex state write
  public ComplexState WriteComplexState()
  {
    // saves this instance cutsceneName
    CutsceneLocalHandlerComplexState state = new CutsceneLocalHandlerComplexState(this.gameObject);
    state.cutsceneName = _cutsceneName;

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

    
  }

  public ComplexState UpdateComplexState(ComplexState state)
  {
    if (!(state is CutsceneLocalHandlerComplexState))
    {
      throw new BipolarExceptionComplexStateNotCompatibleWithScript("Complex State " + state.GetComplexStateName() + " cannot be updated in CutsceneLocalHandler Class");
    }
    CutsceneLocalHandlerComplexState specificState = (state as CutsceneLocalHandlerComplexState);
    specificState.cutsceneName = _cutsceneName;

    return specificState;
  }


  #endregion
}
