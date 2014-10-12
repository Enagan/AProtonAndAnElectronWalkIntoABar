using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class JackedInRemoteController : MonoBehaviour
{
  // List of intended activatable game objects
  [SerializeField]
  protected List<GameObject> _activatableObjects = new List<GameObject>();

  public virtual void Start()
  {
  }

  public void Hit(string key)
  {
    if (key == "Fire1")
    {
      Left();
    }
    if (key == "Fire2")
    {
      Right();
    }
  }

  public abstract void Left();
  
  public abstract void Right();
}