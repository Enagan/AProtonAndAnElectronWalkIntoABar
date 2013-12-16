using UnityEngine;
using System.Collections;

public class CreditTransition : MonoBehaviour {

    public void Start()
    {
      GUITexture logo = GetComponent<GUITexture>();

      logo.pixelInset = new Rect(Screen.width/2-1200/2,Screen.height/2-400/2,1200,400);
    }

	 public void doTransition( float time )
    {
		Application.LoadLevel("Credits");
    }
}
