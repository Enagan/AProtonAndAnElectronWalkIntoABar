using UnityEngine;
using System.Collections;

/// <summary>
/// Listner for Player Room Change Events in any room and dims light after he leaves the room
/// 
/// </summary>
public class DimLightOnPlayerChangeRoom : MonoBehaviour, IPlayerRoomChangeListner {

  // Room Name that contains light
  [SerializeField]
  private string _RoomName;

  private Light _light = null;

  [SerializeField]
  private const float _timeToDim = 5.0f;

  private float _diffIntensity = 0.0f;
  private float _startIntensity;

  void Start()
  {
    _light = GetComponent<Light>();
    if(_light != null)
      _startIntensity = _light.intensity;
    _light.intensity = 0.0f;
  }

  public void ListenPlayerRoomChange(string newRoomName)
  {
    if (_light == null)
      return;

    if (newRoomName == _RoomName)
    {
      // should undim
      _diffIntensity = _startIntensity / _timeToDim;
    }
    else
    {
      // should dim
      _diffIntensity = -_startIntensity / _timeToDim;
    }

  }

  void Update()
  {
    float delta = Time.deltaTime;
    if (_diffIntensity < 0.0f)
    {
      if (_light.intensity == 0.0f)
        BipolarConsole.AllLog("UnDimming");

      _light.intensity += _diffIntensity * delta;
      if(_light.intensity >= _startIntensity)
      {
        _light.intensity = _startIntensity;
        _diffIntensity = 0;
      }
    }
    else if (_diffIntensity > 0.0f)
    {
      if (_light.intensity == 0.0f)
        BipolarConsole.AllLog("Dimming");
      _light.intensity += _diffIntensity * delta;
      if (_light.intensity <= _startIntensity)
      {
        _light.intensity = _startIntensity;
        _diffIntensity = 0;
      }
    }
  }


}
