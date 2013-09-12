using System;
using UnityEngine;
using UnityEditor;

class LightActivator : MonoBehaviour, Activator
{
  private float _lightIntensity;
  private Light _associatedLight;

  void Start()
  {
    _associatedLight = this.GetComponent<Light>();
    _lightIntensity = _associatedLight.intensity;
    Deactivate();
  }

  public void Activate()
  {
    _associatedLight.intensity = _lightIntensity;
  }

  public void Deactivate()
  {
    _associatedLight.intensity = 0;
  }
}
