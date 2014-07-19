using UnityEngine;
using System.Collections;

public class MoveWithPlayerLight : MonoBehaviour {

  [SerializeField]
  Transform _player;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
    Vector3 newPos = new Vector3(_player.position.x, transform.position.y, _player.position.z);
    transform.position = newPos;
	}
}
