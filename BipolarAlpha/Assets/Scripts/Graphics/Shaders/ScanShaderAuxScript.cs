//Made By: Ivo
using UnityEngine;
using System.Collections;

public class ScanShaderAuxScript : MonoBehaviour, IPlayerAbilityScanListener
{
  //Scan Material wish Shader
	private Material _ScanMat;
  
  //Lists of Materials in object
  private Material[] _oriMats; // original materials for restoring
  private Material[] _scanMats; // materials plus _ScanMat

  void Start()
  {

	  _ScanMat = Resources.Load("Materials/ScanMaterial") as Material;

    _oriMats = renderer.materials;

    _scanMats = new Material[renderer.materials.Length + 1];
	  for(int i = 0; i < renderer.materials.Length; i++)
      _scanMats[i] = renderer.materials[i];
    _scanMats[renderer.materials.Length] = _ScanMat;

    ServiceLocator.GetEventHandlerSystem().RegisterPlayerAbilityScanListener(this);
  }

  public void ListenPlayerScan(bool isScanning)
  {
    BipolarConsole.AllLog("SCANNING");
    if (isScanning)
      renderer.material = _ScanMat;
    else
      renderer.materials = _oriMats;
  }
}
