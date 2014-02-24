using System;
using UnityEngine;
//using UnityEditor;

class LightUp : MonoBehaviour, Activator
{
  #region private fields
  // Stores the associated light object
  private Light[] _associatedLight;

  // Stores the associated light's intensity
  private float _lightIntensity;
  
  private bool isActivated;
  #endregion


  void Start()
  {
    _associatedLight = this.GetComponentsInChildren<Light>();
    _lightIntensity = _associatedLight[0].intensity;
    TurnOff();
  }

  public void TurnOn()
  {
    if(isActivated)
    {
      foreach (Light light in _associatedLight)
      {
        light.intensity = _lightIntensity;
      }
    }
  }

  public void TurnOff()
  {
    foreach (Light light in _associatedLight)
      {
        light.intensity = 0;
      }

    isActivated=false;
  }

  private void OnTriggerEnter(Collider other)
  {
    TurnOn();
  }

  private void OnTriggerExit(Collider other)
  {
    TurnOff();
  }

  void Activator.Activate()
  {

      
    isActivated = true;
  }

  void Activator.Deactivate()
  {

    isActivated = false;
      
  }
}