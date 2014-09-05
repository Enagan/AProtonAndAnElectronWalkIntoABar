using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JackedInElevatorControler : JackedInRemoteController {

  public enum ElevatorMode {STOP, MOVE, AT_BOTTOM, AT_TOP};

  [SerializeField]
  private float _elevatorSpeed = 5.0f;

  private ElevatorMode _activeMode = ElevatorMode.STOP;

  [SerializeField]
  private GameObject _elevatorBottom;
  [SerializeField]
  private GameObject _elevatorTop;
  [SerializeField]
  private List<GameObject> _elevatorFloors = new List<GameObject>();

  public override void Left()
  {
    if (_activeMode != ElevatorMode.AT_TOP)
    {
      _activeMode = ElevatorMode.MOVE;
      if (_elevatorSpeed < 0.0f)
      {
        _elevatorSpeed *= -1.0f;
      }
    }
  }

  public override void Right()
  {
    if (_activeMode != ElevatorMode.AT_BOTTOM)
    {
      _activeMode = ElevatorMode.MOVE;
      if (_elevatorSpeed > 0.0f)
      {
        _elevatorSpeed *= -1.0f;
      }
    }
  }

  private void Update()
  {
    if (_activeMode == ElevatorMode.MOVE)
    {
      foreach (GameObject obj in _activatableObjects)
      {
        Move(obj);
        Check(obj);
      }
    }
  }

  private void Move(GameObject obj)
  {
    obj.transform.Translate(obj.transform.up * _elevatorSpeed * Time.deltaTime);
  }

  private void Check(GameObject obj)
  {
    if (obj.transform.position.y <= _elevatorBottom.transform.position.y)
    {
      _activeMode = ElevatorMode.AT_BOTTOM;
      this.transform.position = new Vector3(this.transform.position.x, _elevatorBottom.transform.position.y, this.transform.position.z);
    }
    else if (obj.transform.position.y >= _elevatorTop.transform.position.y)
    {
      _activeMode = ElevatorMode.AT_TOP;
      this.transform.position = new Vector3(this.transform.position.x, _elevatorTop.transform.position.y, this.transform.position.z);
    }
    else
    {
      foreach (GameObject floor in _elevatorFloors)
      {
        if (obj.transform.position.y == floor.transform.position.y)
        {
          _activeMode = ElevatorMode.STOP;
        }
      }
    }
  }
}
