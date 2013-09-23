using UnityEngine;
using System.Collections;
using System.Collections.Generic;

enum TutorialMessage { Generators, OppositeMagnets }

public class TutorialMessageTrigger : MonoBehaviour {

  [SerializeField]
  private TutorialMessage _messageToSend;

  private Dictionary<TutorialMessage, string> _messages = new Dictionary<TutorialMessage, string>();

  private void Start()
  {
    _messages[TutorialMessage.Generators] = "When Magnetic Force is applied \n" +
                                            "to the rotatable piece, this device \n" +
                                            "will generate an electric current \n";

    _messages[TutorialMessage.OppositeMagnets] = "Equally charged magnets \n" +
                                            "when activated \n" +
                                            "strongly repel each other \n";
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.tag == "Player")
    {
      ServiceLocator.GetEventHandlerSystem().SendTutorialMessageTriggerEvent(_messages[_messageToSend]);
    }
  }
}
