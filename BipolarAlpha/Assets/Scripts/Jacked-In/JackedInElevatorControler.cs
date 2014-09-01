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
      foreach (GameObject go in _activatableObjects)
      {
        Check();
        Move(go);
      }
    }
  }

  private void Move(GameObject go)
  {
    go.transform.position += go.transform.up * _elevatorSpeed * Time.deltaTime;
  }

  private void Check()
  {
    if (this.transform.position.y <= _elevatorBottom.transform.position.y)
    {
      _activeMode = ElevatorMode.AT_BOTTOM;
      this.transform.position = new Vector3(this.transform.position.x, _elevatorBottom.transform.position.y, this.transform.position.z);
    }
    else if (this.transform.position.y >= _elevatorTop.transform.position.y)
    {
      _activeMode = ElevatorMode.AT_TOP;
      this.transform.position = new Vector3(this.transform.position.x, _elevatorTop.transform.position.y, this.transform.position.z);
    }
    else
    {
      foreach (GameObject floor in _elevatorFloors)
      {
        if (this.transform.position.y == floor.transform.position.y)
        {
          _activeMode = ElevatorMode.STOP;
        }
      }
    }
  }
}
