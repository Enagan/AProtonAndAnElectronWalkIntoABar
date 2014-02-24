using UnityEngine;
using System.Collections;

public class InitialCutsceneTransition : MonoBehaviour {

  public void transitionScene()
  {
    Application.LoadLevel("BootSequence");
  }
}
