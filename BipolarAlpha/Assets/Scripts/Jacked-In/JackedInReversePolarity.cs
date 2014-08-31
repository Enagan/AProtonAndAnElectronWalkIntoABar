using UnityEngine;
using System.Collections;

public class JackedInReversePolarity : JackedInRemoteController
{

  [SerializeField]
  float _multiClickPreventionTimer = 1.0f;

  float _lastTimer = 0.0f;

  public override void Left()
  {
    if (Time.time > _lastTimer + _multiClickPreventionTimer)
    {
      this.transform.parent.GetComponentInChildren<MagneticForce>().RevertCharge();
      _lastTimer = Time.time;
    }
  }

  public override void Right()
  {
    if (Time.time > _lastTimer + _multiClickPreventionTimer)
    {
      this.transform.parent.GetComponentInChildren<MagneticForce>().RevertCharge();
      _lastTimer = Time.time;
    }
  }



}
