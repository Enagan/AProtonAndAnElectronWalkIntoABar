// Made by: Lousada
using UnityEngine;
using System.Collections;

/// <summary>
/// Automatic doors that open when Activated and Close when Deactivated
/// </summary>
public class HexBottomDoors : MonoBehaviour, Activator
{
  Animation _leftPart;
  Animation _rightPart;
  Animation _centerPart;
  bool open;

  public void Start()
  {
    open = false;
    _leftPart = (Animation)transform.FindChild("Left").GetComponent(typeof(Animation));
    _rightPart = (Animation)transform.FindChild("Right").GetComponent(typeof(Animation));
    _centerPart = (Animation)transform.FindChild("Center").GetComponent(typeof(Animation));

  }

  public void Activate()
  {
    if (open)
    {
      return;
    }
    open = true;
    _leftPart.CrossFade("BottomLeftOpen");
    _rightPart.CrossFade("BottomRightOpen");
    _centerPart.CrossFade("BottomCenterOpen");
  }

  public void Deactivate()
  {
    if (!open)
    {
      return;
    }
    open = false;
    _leftPart.CrossFade("BottomLeftClose");
    _rightPart.CrossFade("BottomRightClose");
    _centerPart.CrossFade("BottomCenterClose");
  }
}
