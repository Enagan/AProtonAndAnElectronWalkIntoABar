using UnityEngine;
using System.Collections;

public class DummyDoor : MonoBehaviour 
{

  Vector3 _originalPosition = Vector3.zero;
  public Vector3 _openedPosition = Vector3.zero;

  public void Start()
  {
    _originalPosition = transform.localPosition;
  }

  public void Activate()
  {
    transform.localPosition = _openedPosition;
  }

  public void Deactivate()
  {
    transform.localPosition = _originalPosition;
  }
}
