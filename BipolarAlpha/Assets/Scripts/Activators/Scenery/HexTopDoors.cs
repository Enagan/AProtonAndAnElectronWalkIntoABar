// Made by: Lousada
using UnityEngine;
using System.Collections;

/// <summary>
/// Automatic doors that open when Activated and Close when Deactivated
/// </summary>
public class HexTopDoors : MonoBehaviour, Activator
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
    ServiceLocator.GetAudioSystem().PlayQuickSFX("OpenDoor",this.transform.position);
    _leftPart.CrossFade("TopLeftOpen");
    _rightPart.CrossFade("TopRightOpen");
    _centerPart.CrossFade("TopCenterOpen");
  }

  public void Deactivate()
  {
    if (!open)
    {
      return;
    }
    open = false;
    ServiceLocator.GetAudioSystem().PlayQuickSFX("CloseDoor", this.transform.position);

    _leftPart.CrossFade("TopLeftClose");
    _rightPart.CrossFade("TopRightClose");
    _centerPart.CrossFade("TopCenterClose");
  }
}
