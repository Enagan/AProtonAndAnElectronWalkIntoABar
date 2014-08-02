using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Listner for Player Room Change Events in any room and dims light after he leaves the room
/// 
/// </summary>
public class DimLightOnPlayerChangeRoom : MonoBehaviour, IPlayerRoomChangeListner {

  // Room Name that contains light
  [SerializeField]
  private List<string> _RoomNames;

  private Light _light = null;

  [SerializeField]
  private float _timeToDim = 2.0f;

  private float _diffIntensity = 0.0f;
  private float _startIntensity;

  void Start()
  {
    _light = GetComponent<Light>();
    if(_light != null)
      _startIntensity = _light.intensity;
    _light.intensity = 0.0f;

    ServiceLocator.GetEventHandlerSystem().RegisterPlayerRoomChangeListner(this);

  }

  public void ListenPlayerRoomChange(string newRoomName)
  {
    if (_light == null)
      return;

    bool found = false;

    foreach (string roomName in _RoomNames)
    {
      found = found || roomName == newRoomName;
    }

    if (found)
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
    if (_diffIntensity > 0.0f)
    {
      if (_light.intensity == 0.0f)

      _light.intensity += _diffIntensity * delta;
      if(_light.intensity >= _startIntensity)
      {
        _light.intensity = _startIntensity;
        _diffIntensity = 0;
      }
    }
    else if (_diffIntensity < 0.0f)
    {
      if (_light.intensity == 0.0f)
      _light.intensity += _diffIntensity * delta;
      if (_light.intensity <= 0)
      {
        _light.intensity = 0;
        _diffIntensity = 0;
      }
    }
  }


}
