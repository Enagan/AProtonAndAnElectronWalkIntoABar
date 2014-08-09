using UnityEngine;
using System.Collections;

public class PushBack: MonoBehaviour{

  [SerializeField]
  private float _pushForce = 2000.0f;

  public void OnTriggerExit(Collider col){
    Debug.Log("Exit");
    if (col.gameObject.tag == "Player")
    {
      col.gameObject.rigidbody.AddForce((this.transform.position - col.transform.position).normalized * _pushForce, ForceMode.VelocityChange);        
    }
  }

  
}
