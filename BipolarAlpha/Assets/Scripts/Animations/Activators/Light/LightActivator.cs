using System;
using UnityEngine;
//using UnityEditor;

class LightActivator : MonoBehaviour, Activator
{
  #region private fields
  // Stores the associated light object
  private Light _associatedLight;

  // Stores the associated light's intensity
  private float _lightIntensity;
  #endregion

  void Start()
  {
    _associatedLight = this.GetComponent<Light>();
    _lightIntensity = _associatedLight.intensity;
    Deactivate();
  }

  #region Activator interface methods
  /// <summary>
  /// Activates the light (Turns the associated light on)
  /// Gives to the associated light intensity its (stored) value to turn it on
  /// </summary>
  public void Activate()
  {
    _associatedLight.intensity = _lightIntensity;
  }

  /// <summary>
  /// Deactivates the light (Turns the associated light off)
  /// Makes the associated light intensity equal 0(zero) to turn it off
  /// </summary>
  public void Deactivate()
  {
    _associatedLight.intensity = 0;
  }
  #endregion
}