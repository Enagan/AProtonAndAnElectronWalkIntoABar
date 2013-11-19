//Made By: Ivo
using UnityEngine;
using System.Collections;

public class ScanShaderAuxScript : MonoBehaviour
{
	private Material _ScanMat;
		
  void Start()
  {

	_ScanMat = Resources.Load("Materials/ScanMaterial") as Material;

	Material[] rendMats = new Material[renderer.materials.Length+1];
	for(int i = 0; i < renderer.materials.Length; i++)
			rendMats[i]=renderer.materials[i];
	rendMats[renderer.materials.Length] = _ScanMat;
	renderer.materials = rendMats;
  }
}
