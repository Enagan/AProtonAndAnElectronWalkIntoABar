using System;
using UnityEngine;

public class MagnetDispenser : MonoBehaviour, Activator
{
  [SerializeField]
  private MagneticForce.Charge _charge = MagneticForce.Charge.NEGATIVE;

  #region Activator Interface functions
  void Activator.Activate()
  {
    if (_charge == MagneticForce.Charge.NEGATIVE)
    {
      ServiceLocator.GetResourceSystem().InstanceOf("Prefabs/Activators/Magnets/MovableMagnet", this.transform.position);
    }
    else
    {
      ServiceLocator.GetResourceSystem().InstanceOf("Prefabs/Activators/Magnets/MovableMagnet", this.transform.position);
    }
  }

  void Activator.Deactivate()
  {
    
  }
  #endregion
}