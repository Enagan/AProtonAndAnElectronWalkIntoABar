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

  public void Start()
  {
    _leftPart = (Animation)transform.FindChild("Left").GetComponent(typeof(Animation));
    _rightPart = (Animation)transform.FindChild("Right").GetComponent(typeof(Animation));
    _centerPart = (Animation)transform.FindChild("Center").GetComponent(typeof(Animation));

  }

  public void Activate()
  {
    ServiceLocator.GetAudioSystem().PlayQuickSFX("OpenDoor",this.transform.position);
    _leftPart.CrossFade("TopLeftOpen");
    _rightPart.CrossFade("TopRightOpen");
    _centerPart.CrossFade("TopCenterOpen");
  }

  public void Deactivate()
  {
    ServiceLocator.GetAudioSystem().PlayQuickSFX("OpenDoor", this.transform.position);

    _leftPart.CrossFade("TopLeftClose");
    _rightPart.CrossFade("TopRightClose");
    _centerPart.CrossFade("TopCenterClose");
  }
}
