using System;
using UnityEngine;

public class MagnetDispenser : MonoBehaviour, Activator
{
  private bool _isActive = true;

  [SerializeField]
  private MagneticForce.Charge _charge = MagneticForce.Charge.NEGATIVE;

  #region Activator Interface functions
  void Activator.Activate()
  {
    _isActive = true;
  }

  void Activator.Deactivate()
  {
    _isActive = false;
  }
  #endregion

  #region Monobehaviour functions
  private void Update()
  {
    if(_isActive)
    {
      if (_charge == MagneticForce.Charge.NEGATIVE)
      {
        GameObject magnet = ServiceLocator.GetResourceSystem().InstanceOf("Prefabs/Activators/Magnets/MovableMagnet", this.transform.position + new Vector3(0, 2, 0));
      }
      else
      {
        GameObject magnet = ServiceLocator.GetResourceSystem().InstanceOf("Prefabs/Activators/Magnets/MovableMagnet", this.transform.position + new Vector3(0, 2, 0));
      }
      _isActive = false;
    }
  }
  #endregion
}