using UnityEngine;
using System.Collections;

public class EnderScript : MonoBehaviour {

	// Use this for initialization
  void OnTriggerEnter(Collider collider)
  {
    if(collider.tag.Equals("Player"))
    {
      Application.LoadLevel("CreditsFade");
    }

  }
}
