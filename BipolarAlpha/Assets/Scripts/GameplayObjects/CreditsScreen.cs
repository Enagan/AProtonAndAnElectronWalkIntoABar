using UnityEngine;
using System.Collections;

public class CreditsScreen : MonoBehaviour {

  public void Start()
  {
    GUITexture logo = this.transform.FindChild("SM Logo").GetComponent<GUITexture>();

    logo.pixelInset = new Rect((Screen.width/4)/2 -290/2, Screen.height / 2 - 290 / 2 - 55, 290, 290);
  }

	// Update is called once per frame
	void Update () 
  {
    if (Input.GetKeyUp(KeyCode.Escape))
    {
      Application.LoadLevel("MainMenu");
    }
	}
}
