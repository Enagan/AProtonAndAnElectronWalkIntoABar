using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TutorialMessage { Generators, OppositeMagnets, HubRoom, StartGame}

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

    _messages[TutorialMessage.HubRoom] = "See those circuits? It appears that\n" +
                                         "the door ahead can be opened by \n" +
                                         "something in the balconies above \n";

    _messages[TutorialMessage.StartGame] = "> MagOS restored sucessfully\n" +
                             "> Situational Awareness Module Activated\n" +
                             "> Previous Directive Restored: \n" +
                             "      Pass Quality Assurance\n" +
                             "\n" +
                             "> Current Step: Exit Recycling Container\n" +
                             "\n" +
                             "> Positive Directional Magnet Driver Corrupted\n" +
                             "> ...\n" +
                             "> ...\n" +
                             "> ...\n" +
                             "> Positive Directional Magnet Driver Restored:\n" +
                             "> Advisor Subroutine:\n" +
                             "      Use left mouse button to activate\n" +
                             "      your directional magnet.\n" +
                             "      Point it at negative charged magnets (blue)\n" +
                             "      For rapid magnetic approximation\n" +
                             "\n(press \"z\" to dismiss command line)";


  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.tag == "Player")
    {
      ServiceLocator.GetEventHandlerSystem().SendTutorialMessageTriggerEvent(_messages[_messageToSend]);
    }
  }
}
