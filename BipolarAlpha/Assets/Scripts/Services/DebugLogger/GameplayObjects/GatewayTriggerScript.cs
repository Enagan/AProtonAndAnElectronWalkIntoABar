//Made By: Engana
using UnityEngine;
using System.Collections;

/// <summary>
/// Gateway script, detects player passing through and keeps the connected room name
/// </summary>
public class GatewayTriggerScript : MonoBehaviour {

  //String that should be set up in the editor. Should be a valid room name
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

  private void OnTriggerEnter(Collider other)
  {
    if (other.tag == "Player")
    {

      ServiceLocator.GetAudioSystem().PlayMusic("kahvi315z_audio_cephlon-majlis_al_jinn");
      ServiceLocator.GetEventHandlerSystem().SendPlayerRoomChangeEvent(connectsTo);
    }
    else if (other.tag == "MovableObject")
    {
      ServiceLocator.GetEventHandlerSystem().SendObjectRoomChangeEvent(other.gameObject.transform.parent.name, transform.parent.name, other.gameObject);
    }
    else
    {
      //throw new BipolarExceptionUnexpectedObjectTraversingGateway("Object " + other.name + " with tag " + other.tag + " Tried to traverse " + this + " gateway");
    }

  }
	
}
