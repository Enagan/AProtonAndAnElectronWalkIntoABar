using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {


    Camera _camera;

	// Use this for initialization
	void Start ()
    {
        // For debug
        GetComponent<MeshRenderer>().renderer.enabled = false;

       _camera = GetComponentInChildren<Camera>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void spawnPlayer(PlayerController player)
    {
        //player.GetComponentInChildren<Camera>().transform.LookAt(_camera.transform.forward);
        PlayerController player2 = (PlayerController)Object.Instantiate(player);
        player.PlayerActivation(false);
        player2.transform.position = transform.position;
        player2.PlayerActivation(true);
     //   player2.transform.rotation = transform.rotation;
        //player2.transform.rotation = transform.rotation;
    }

}
