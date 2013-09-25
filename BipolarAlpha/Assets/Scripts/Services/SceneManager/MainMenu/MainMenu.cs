using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Return))
    {
      GameObject.Find("Main Camera").animation.Play();
      Invoke("ChangeScene", 1.5f);
    }
  }

 void ChangeScene() {
   Application.LoadLevel("Main");
     }
}

