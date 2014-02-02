using System;
using UnityEngine;

public class MagnetDispenser : MonoBehaviour, Activator
{
  [SerializeField]
  private MagneticForce.Charge _charge = MagneticForce.Charge.NEGATIVE;
  private GameObject _lastCreatedMagnet;

  #region Activator Interface functions

  public void Activate()
  {
    if (_charge == MagneticForce.Charge.NEGATIVE)
    {
      _lastCreatedMagnet = ServiceLocator.GetResourceSystem().InstanceOf("Prefabs/Activators/Magnets/MovableNegativeMagnet", this.transform.position);
    }
    else
    {
      _lastCreatedMagnet = ServiceLocator.GetResourceSystem().InstanceOf("Prefabs/Activators/Magnets/MovablePositiveMagnet", this.transform.position);
    }
  }

  public void Deactivate()
  {
    
  }
  #endregion

  /// <summary>
  /// Compares the given object reference to the last spawned magnet
  /// Used primarily by the ExitTrigger of the magnet dispenser
  /// </summary>

  public bool IsLastCreated(GameObject obj)
  {
    return obj == _lastCreatedMagnet;
  }
}