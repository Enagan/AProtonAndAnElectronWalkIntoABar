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

      foreach (GameObject obj in _activatableObjects)
      {
        obj.GetComponent<MagneticForce>().RevertCharge();
      }
      _lastTimer = Time.time;
    }
  }

  public override void Right()
  {
    Left();
  }



}
