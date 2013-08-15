//Made By: Engana
using UnityEngine;
using System.Collections;

/// <summary>
/// Gateway script, detects player passing through and keeps the connected room name
/// </summary>
public class GatewayTriggerScript : MonoBehaviour {

  [SerializeField]
  private string _connectsTo;

  /// <summary>
  /// Returns the name of the room this gate is connected to
  /// </summary>
  public string connectsTo
  {
    get
    {
      return _connectsTo;
    }
    set
    {
      _connectsTo = value;
    }
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.tag == "Player")
    {
      ServiceLocator.GetEventHandlerSystem().SendPlayerRoomChangeEvent(transform.parent.name);
    }
  }
	
}
