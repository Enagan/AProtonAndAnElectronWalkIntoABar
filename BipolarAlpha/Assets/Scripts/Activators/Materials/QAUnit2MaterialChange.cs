using UnityEngine;
using System.Collections;

public class QAUnit2MaterialChange : MonoBehaviour, Activator
{

  MeshRenderer _renderer;
  [SerializeField]
  Material _blue;
  [SerializeField]
  Material _red;
  public void Start()
  {
    Deactivate();
  }
  public void Activate()
  {
    Material[] newMats = renderer.materials;
    newMats[1] = _blue;
    renderer.materials = newMats;
  }

  public void Deactivate()
  {
    Material[] newMats = renderer.materials;
    newMats[1] = _red;
    renderer.materials = newMats;
    

  }
}
