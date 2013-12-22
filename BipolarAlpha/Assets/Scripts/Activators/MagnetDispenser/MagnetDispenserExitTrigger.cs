using UnityEngine;
using System.Collections;

public class MagnetDispenserExitTrigger : MonoBehaviour {

  private MagnetDispenser _spawner;

  public void Start()
  {
   _spawner = this.transform.parent.GetComponentInChildren<MagnetDispenser>();
  }

  public void OnTriggerExit(Collider other)
  {
    if(_spawner.IsLastCreated(other.gameObject))
    {
      this.transform.parent.GetComponentInChildren<MagnetDispenserGenerator>().Reset();

      this.transform.parent.GetComponentInChildren<DispenserActivator>().Deactivate();
    }
  }

	
}
