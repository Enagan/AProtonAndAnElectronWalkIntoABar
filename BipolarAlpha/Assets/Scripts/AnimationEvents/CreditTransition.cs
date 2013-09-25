using UnityEngine;
using System.Collections;

public class CreditTransition : MonoBehaviour {

	 public void doTransition( float time )
    {
		Application.LoadLevel("Credits");
    }
}
