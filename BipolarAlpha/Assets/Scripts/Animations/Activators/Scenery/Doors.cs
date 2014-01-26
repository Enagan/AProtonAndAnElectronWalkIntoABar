// Made by: Lousada
using UnityEngine;
using System.Collections;

/// <summary>
/// Automatic doors that open when Activated and Close when Deactivated
/// </summary>
public class Doors : MonoBehaviour, Activator 
{
  Animation _leftPart;
  Animation _rightPart;

  public void Start()
  {
    _leftPart = (Animation)transform.FindChild("left").GetComponent(typeof(Animation));
    _rightPart = (Animation)transform.FindChild("right").GetComponent(typeof(Animation));

  }

  public void Activate()
  {
    _leftPart.Play("LeftDoorOpen");
    _rightPart.Play("RightDoorOpen");
  }
  
  public void Deactivate()
  {
    _leftPart.Play("LeftDoorClose");
    _rightPart.Play("RightDoorClose");
  }
}
