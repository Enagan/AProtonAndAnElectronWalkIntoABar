using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {


    Camera _camera;

    PlayerController _originalPlayer = null;
    PlayerController _spawnedPlayer = null;

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
        if (_originalPlayer == null)
        {
          _originalPlayer = player;
          _spawnedPlayer = (PlayerController)Object.Instantiate(player);
          _originalPlayer.PlayerActivation(false);
          _spawnedPlayer.transform.position = transform.position;
          _spawnedPlayer.PlayerActivation(true);
        }
    }

    public void returnPlayer()
    {
        if(_originalPlayer != null)
        {

            _spawnedPlayer.PlayerActivation(false);
            _originalPlayer.PlayerActivation(true);
            _originalPlayer.transform.position = transform.position; // temporary
            Destroy(_spawnedPlayer.gameObject);
            _originalPlayer = null;
            _spawnedPlayer = null;
        }
    }

}
