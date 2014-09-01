using UnityEngine;
using System.Collections;

public class JackedInElevatorControler : JackedInRemoteController {

  public enum ElevatorMode {STOP, MOVE, AT_BOTTOM, AT_TOP};

  [SerializeField]
  private float _elevatorSpeed = 5.0f;

  private ElevatorMode _activeMode = ElevatorMode.STOP;

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
        Move(go);
      }
    }
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.name == "ElevatorStopFloor")
    {
      _activeMode = ElevatorMode.STOP;
    }
    else if (other.gameObject.name == "ElevatorTop")
    {
      _activeMode = ElevatorMode.AT_TOP;
    }
    else if (other.gameObject.name == "ElevatorBottom")
    {
      _activeMode = ElevatorMode.AT_BOTTOM;
    }
  }

  private void Move(GameObject go)
  {
    go.transform.position += go.transform.up * _elevatorSpeed * Time.deltaTime;
  }
}
