using UnityEngine;
using System.Collections;

public class BarredDoorSound : MonoBehaviour {

  void playLock()
  {
    ServiceLocator.GetAudioSystem().PlayQuickSFX("lock open",transform.position,0.5f);
  }


  void playDoorSound()
  {
    ServiceLocator.GetAudioSystem().PlayQuickSFX("Door",transform.position,1);
  }


}
