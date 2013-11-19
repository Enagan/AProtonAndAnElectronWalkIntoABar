using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TutorialMessage { Generators, OppositeMagnets, HubRoom, StartGame}

public class TutorialMessageTrigger : MonoBehaviour {

  [SerializeField]
  private TutorialMessage _messageToSend;

  private string message = "";

  private void Start()
  {
    // Loads Tutorial Message on start
    message = ServiceLocator.GetStringRetrievalSystem().getStringWithKey("TutorialMessage."+_messageToSend);
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.tag == "Player")
    {
      ServiceLocator.GetEventHandlerSystem().SendTutorialMessageTriggerEvent(message);
    }
  }
}
