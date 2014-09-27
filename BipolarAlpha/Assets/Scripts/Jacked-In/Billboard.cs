using UnityEngine;
using System.Collections;
class Billboard : MonoBehaviour, IJackedInActivationListener
{
  public enum ACTIVATION_TYPE {PERMANENT, ACTIVATABLE, RANGE}

  private Camera _jackedInCamera;

  [SerializeField]
  // Boolean that determines whether billboard is active during jacked-in mode or not
  private bool _jackedInMode = false;
  // Boolean that helps check if player is in jacked-in mode or not
  private bool _isJackedIn = false;
  [SerializeField]
  private ACTIVATION_TYPE _activationType = ACTIVATION_TYPE.PERMANENT;

  public ACTIVATION_TYPE activationType
  {
    get
    {
      return _activationType;
    }
  }

  public void Awake()
  {
    if (_jackedInMode)
    {
      Deactivate();
      _jackedInCamera = null;
    }
    else
    {      
      _jackedInCamera = UnityEngine.Camera.main;
    }

    ServiceLocator.GetEventHandlerSystem().RegisterJackedInActivationListener(this);

    if (_activationType != ACTIVATION_TYPE.PERMANENT)
    {
      Deactivate();
    }
  }

  public void Update()
  {
    // Ensures billboard is not active in the wrong mode
    if ((_jackedInMode && !_isJackedIn) || (!_jackedInMode && _isJackedIn))
    {
      Deactivate();
      return;
    }
    if (_jackedInCamera != null)
    {
      this.transform.forward = -_jackedInCamera.transform.forward;
      this.transform.up = Vector3.up;
      this.transform.right = -_jackedInCamera.transform.right;

      transform.LookAt(_jackedInCamera.transform.position, transform.up);

      transform.Rotate(0.0f, 180.0f, 0.0f);
    }
  }

  public void Activate()
  {
    // Checks whether the desired mode of the billboard and the currently active mode match
    if ((_jackedInMode && _isJackedIn) || (!_jackedInMode && !_isJackedIn))
    {
      gameObject.SetActive(true);
    }
  }

  public void Deactivate()
  {
    gameObject.SetActive(false);
  }

  public void ListenJackedInActivation(Camera camera)
  {
    _isJackedIn = true;

    if (_jackedInMode)
    {
      _jackedInCamera = camera;
      if (_activationType == ACTIVATION_TYPE.PERMANENT)
      {
        Activate();
      }
    }
    else if (_activationType == ACTIVATION_TYPE.PERMANENT)
    {
        Deactivate();
    }
  }

  public void ListenJackedInDeactivation()
  {
    _isJackedIn = false;

    if (_jackedInMode)
    {
      if (_activationType == ACTIVATION_TYPE.PERMANENT)
      {
        Deactivate();
      }
      _jackedInCamera = null;
    }
    else if (_activationType == ACTIVATION_TYPE.PERMANENT)
    {
      Activate();
    }
  }
}