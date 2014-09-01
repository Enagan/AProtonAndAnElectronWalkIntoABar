using UnityEngine;
using System.Collections;

public class JackedInRotaryRamp : JackedInRemoteController
{
  enum Lock {top, bottom};

  [SerializeField]
  Lock _currentLock = Lock.bottom;

  [SerializeField]
  float _multiClickPreventionTimer = 1.0f;
  float _lastTimer = -10.0f;


  [SerializeField]
  float _rotationSpeed = 0.2f;


  //The top and the bottom magnets that define the limits of the rotation
  [SerializeField]
  GameObject _topMagnet = null;
  [SerializeField]
  GameObject _bottomMagnet = null;
  [SerializeField]
  GameObject _rampMagnet = null;

  Quaternion _from, _to;

  bool _onlyOnce = true;
  
  public void Start()
  {
    _from = Quaternion.identity;
    _to = Quaternion.identity;
    
    Left();
  }

    public void Update()
  {     
      foreach (GameObject obj in _activatableObjects)
      {
        if (_to != Quaternion.identity && obj.transform.rotation != _to)
        {
          obj.transform.rotation = Quaternion.Slerp(obj.transform.rotation, _to, Time.deltaTime * _rotationSpeed);   //does the actual rotation
        }
    }
  }
  public override void Left()
  {
    if (Time.time > _lastTimer + _multiClickPreventionTimer)   //To prevent multiply activation on the same mouse click
    {

      foreach (GameObject obj in _activatableObjects)
      {
        obj.GetComponentInChildren<MagneticForce>().RevertCharge();


        //Defines the final rotation without altering the object's rotation yet
        _from = obj.transform.rotation;
        obj.transform.Rotate(Vector3.up, GetAngle(obj), Space.Self);
        _to = obj.transform.rotation;
        obj.transform.rotation = _from;
      }
      _lastTimer = Time.time;
    }
  }

  public override void Right()
  {
    Left();
  }

  private float GetAngle(GameObject pivot) //simple trigonometry to figure out how many degrees to move
  {
    float hypotenuse;
    float opposite;
    float angle = 0.0f;

    if (_topMagnet == null || _bottomMagnet == null || _rampMagnet == null)
    {
      Debug.Log("ERROR: Put the damn magnets on the script!");
      return angle;
    }

    if (_currentLock == Lock.bottom)
    {
      _currentLock = Lock.top;
      hypotenuse = Vector3.Distance(pivot.transform.position, _bottomMagnet.transform.position);
      opposite = _bottomMagnet.transform.position.y - _rampMagnet.transform.position.y;
      angle = Mathf.Rad2Deg * Mathf.Asin(opposite / hypotenuse);
    }
    else
    {
      _currentLock = Lock.bottom;
     hypotenuse = Vector3.Distance(pivot.transform.position, _topMagnet.transform.position);
     opposite = _topMagnet.transform.position.y - _rampMagnet.transform.position.y;
     angle = Mathf.Rad2Deg * Mathf.Asin(opposite / hypotenuse);

    }

    if (_onlyOnce)
    {
      _onlyOnce = false;
      return angle;
    }
    return angle > 0 ? angle - 2.0f : angle + 2.0f;   // overshooting fix

  }
}
