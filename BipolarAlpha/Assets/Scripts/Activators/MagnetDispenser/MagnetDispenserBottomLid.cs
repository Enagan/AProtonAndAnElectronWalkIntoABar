// Made by: Lousada
using UnityEngine;
using System.Collections;

/// <summary>
/// Automatic lids that open when Activated and Close when Deactivated
/// </summary>
public class MagnetDispenserBottomLid : MonoBehaviour, Activator
{
  Animation _bottomLid;

  public void Start()
  {
    _bottomLid = (Animation)transform.GetComponent(typeof(Animation));
  }

  public void Activate()
  {
    _bottomLid.CrossFade("BottomLidOpen");
  }

  public void Deactivate()
  {
    _bottomLid.CrossFade("BottomLidClose");
  }
}
