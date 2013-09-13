// Made by: Lousada
using UnityEngine;
using System.Collections;

/// <summary>
/// Automatic doors that open when Activated and Close when Deactivated
/// </summary>
public class HexDoors : MonoBehaviour, Activator
{
  Animation _leftPart;
  Animation _rightPart;
  Animation _centerPart;

  public void Start()
  {
    _leftPart = (Animation)transform.FindChild("Left").GetComponent(typeof(Animation));
    _rightPart = (Animation)transform.FindChild("Right").GetComponent(typeof(Animation));
    _centerPart = (Animation)transform.FindChild("Center").GetComponent(typeof(Animation));

  }

  public void Activate()
  {
    _leftPart.Play("TopLeftOpen");
    _rightPart.Play("TopRightOpen");
    _centerPart.Play("TopCenterOpen");
  }

  public void Deactivate()
  {
    _leftPart.Play("TopLeftClose");
    _rightPart.Play("TopRightClose");
    _centerPart.Play("TopCenterClose");
  }
}
