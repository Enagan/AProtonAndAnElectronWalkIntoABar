using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {

  void playLock()
  {
    ServiceLocator.GetAudioSystem().PlayQuickSFX("lock open", transform.position, 1);
  }


  void playDoorSound()
  {
    ServiceLocator.GetAudioSystem().PlayQuickSFX("Door", transform.position, 1);
  }

}
