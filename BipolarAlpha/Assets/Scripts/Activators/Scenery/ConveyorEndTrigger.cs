using UnityEngine;
using System.Collections;

public class ConveyorEndTrigger : MonoBehaviour {

  void OnTriggerEnter(Collider other)
  {
    this.gameObject.transform.parent.gameObject.GetComponent<ConveyorSystem>().CollisionWithEndPoint(other.gameObject);
  }

}
