using UnityEngine;

using System.Collections;

public class JackedInParticleSelector : MonoBehaviour {


  [SerializeField]
  float _selectTex;

	// Use this for initialization
	void Start () {
   Material _mat = renderer.material;
    Random rand = new Random();

    _mat.SetFloat("_selectTex", _selectTex);

    
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
