using UnityEngine;
using System.Collections;

public class BillboardLookAtPlayer : MonoBehaviour {
	
	[SerializeField]
	Camera cam;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(cam!=null)
		{
			transform.LookAt(cam.transform.position,Vector3.up);
		}

			
	}
}
