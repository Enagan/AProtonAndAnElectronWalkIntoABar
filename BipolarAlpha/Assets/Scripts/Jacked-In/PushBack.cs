using UnityEngine;
using System.Collections;

public class PushBack: MonoBehaviour{

  [SerializeField]
  private float _pushForce = 2000.0f;

  private GameObject player;

  public void OnTriggerExit(Collider col){
    if (col.gameObject.tag == "Player")
    {
      player = col.gameObject; 
      col.gameObject.rigidbody.AddForce((this.transform.position - col.transform.position).normalized * _pushForce, ForceMode.Impulse);
      Invoke("StopMovement", 0.2f);

    }
  }

  private void StopMovement()
  {
    player.rigidbody.velocity = Vector3.zero;

  }

  
}
