using UnityEngine;
using System.Collections;

public class WeightPlatform : MonoBehaviour, Activator, IHasComplexState
{

  // Resting height where platform will stop and return to when not holding player
  //[SerializeField] 
//  private float _restingHeightFraction;

  private Rigidbody _body;

   [SerializeField]
  private float _restingHeight;

  // Floor of the room in question
  [SerializeField]
  private float _floorHeight = -4;

  // Acceleration of platform
  [SerializeField]
  private float _acceleration = 0.005f;
  
  // Height where platform starts, initialized in Start()
  [SerializeField]
  //private float _relativeStartHeight;
  private float _startHeight;

  [SerializeField]
  // Current speed of the platform, will change position of next update
  private float _speed = 0;

  [SerializeField]
  private bool _hasPlayer;

  [SerializeField]
  private bool _debugHasplayer;

  private Rigidbody _rigidBody;

  private bool enabled;
  #region monobehavior methods
  // Use this for initialization
	void Start () {

    enabled = false;
    _rigidBody = GetComponent<Rigidbody>();
    
    _hasPlayer = false;
    _speed = 0;

    //_startHeight = _floorHeight + _relativeStartHeight;

//    _restingHeight = _floorHeight + (_startHeight - _floorHeight) * _restingHeightFraction;

    
	}

  
  private void OnTriggerEnter (Collider other) {
		if(other.tag == "Player" && !_hasPlayer)
    {
      this.Activate();
      Debug.Log(gameObject.name);

    }
	}
  
  
  private void OnTriggerStay(Collider other)
  {
    if (other.tag == "Player" && !_hasPlayer)
    {
      this.Activate();
    }
  }

  

  private void OnTriggerExit (Collider other) {
    if (other.tag == "Player" && _hasPlayer)
    {
      this.Deactivate();
     
    }
	}

  bool isColliding;
	/// Update is called once per frame
	void Update () {

    /*

    
    if (_debugHasplayer && !_hasPlayer)
    {
      Activate();

    }
    else if (_hasPlayer && !_debugHasplayer)
    {
      Deactivate();
    }
    */

    if (!enabled)
      return;

    if (_hasPlayer && transform.position.y > _floorHeight) // has player goes down
    {
      _speed = _speed - 3* _acceleration;  // with player goes down faster
      float newPos = transform.position.y + _speed * Time.deltaTime;  // newPos = curPos + deltaX, deltaX = _speed*deltaTime
      float diffVec;
      if (newPos < _floorHeight) // Beyond floor?
      {
        diffVec = _floorHeight - transform.position.y;
        _speed = 0;
      }
      else
        diffVec = newPos - transform.position.y; // not there yet
      //transform.Translate(0, diffVec, 0);
      //transform.position = new Vector3(transform.position.x,transform.position.y + diffVec,transform.position.z);
      _rigidBody.MovePosition(_rigidBody.position + new Vector3(0, diffVec, 0));
    }
    else if(!_hasPlayer && transform.position.y != _restingHeight) // doesnt have player and not there yet
    {
      float pos = transform.position.y;
      float dir = Mathf.Sign( _startHeight - _restingHeight);
      float botPos;
      float topPos;

      bool normalCase = true;
      if (_startHeight < _restingHeight)
      {
        topPos = _startHeight;
        botPos = _restingHeight;
        normalCase = false;
      }
      else
      {
        topPos = _startHeight;
        botPos = _restingHeight;
        normalCase = true;
      }

      float topMid = botPos + (topPos - botPos) / 2;
      float botMid = _floorHeight + (botPos - _floorHeight) / 2;

      bool nearingEnd = false;

      if (normalCase && ((pos > botMid && pos < botPos) || (pos > botPos && pos < topMid)))
      {
        dir = -dir;
        nearingEnd = true;

      }

      float oriSpeed = _speed;

      if (nearingEnd || !normalCase)
        _speed = _speed - dir * _acceleration/2;
      else
        _speed = _speed - dir * _acceleration;
      float newPos = transform.position.y + _speed * Time.deltaTime;
      float diffVec;
     
      bool stopping = false;
      if (nearingEnd && ( -10*_acceleration + _restingHeight <= newPos && newPos <= 10* _acceleration + _restingHeight)
        || (! normalCase && newPos > _restingHeight))
        stopping = true;
           
      if(stopping)
      {
        diffVec = _restingHeight - transform.position.y; // final translation
        _speed = 0;
      }
      else
        diffVec = newPos - transform.position.y; // not there yet
      //transform.position = new Vector3(transform.position.x, transform.position.y + diffVec, transform.position.z);
      _rigidBody.MovePosition(_rigidBody.position+new Vector3(0, diffVec, 0));
    }
	}
  #endregion

  #region Activator methods

  public void Activate() 
  {
    if (!_hasPlayer)
    {
       Debug.Log("Found Player");
        _hasPlayer = true;
        _speed = 0;
    }
  }


  public void Deactivate()
  {
    if (_hasPlayer)
    {
      
      Debug.Log("Lost Player");
      _hasPlayer = false;
      _startHeight = transform.position.y;
    }
    _speed = 0;
  }

  #endregion



  #region Complex State methods

  // Complex state write
  public ComplexState WriteComplexState()
  {
    // saves this instance cutsceneName
    WeightPlatformComplexState state = new WeightPlatformComplexState(this.gameObject);
    state.restingHeight = _restingHeight;
    state.acceleration = _acceleration;
    state.floorHeight = _floorHeight;
    state.startHeight = _startHeight;
    return state;
  }

  public void LoadComplexState(ComplexState state)
  {
    if (!(state is WeightPlatformComplexState))
    {
      throw new BipolarExceptionComplexStateNotCompatibleWithScript(state.GetComplexStateName() + " is not compatible with WeightPlatform class");
    }

    WeightPlatformComplexState handlerState = ((WeightPlatformComplexState)state);

    // loads this instance cutsceneName
    _restingHeight = handlerState.restingHeight;
    _acceleration = handlerState.acceleration;
    _floorHeight = handlerState.floorHeight;
    _startHeight = handlerState.startHeight;

  }

  public ComplexState UpdateComplexState(ComplexState state)
  {
    if (!(state is WeightPlatformComplexState))
    {
      throw new BipolarExceptionComplexStateNotCompatibleWithScript("Complex State " + state.GetComplexStateName() + " cannot be updated in WeightPlatform Class");
    }
    WeightPlatformComplexState specificState = (state as WeightPlatformComplexState);
    specificState.restingHeight = _restingHeight;
    specificState.acceleration = _acceleration;
    specificState.floorHeight = _floorHeight;
    specificState.startHeight = _startHeight;
    return specificState;
  }


  #endregion

  public void AnimationDescendEvent()
  {
    enabled = true;
  }
}
