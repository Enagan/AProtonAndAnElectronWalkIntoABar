using UnityEngine;
using System.Collections;

public class DoorOpenSounds : MonoBehaviour {

  void lockOpen()
  {
    ServiceLocator.GetAudioSystem().PlayQuickSFX("lock open",transform.position,0.5f);
  }

  void doorOpen()
  {
    ServiceLocator.GetAudioSystem().PlayQuickSFX("Door",transform.position,1);
  }
}
