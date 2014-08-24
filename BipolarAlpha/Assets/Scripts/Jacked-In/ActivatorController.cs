using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActivatorController : JackedInRemoteController
{
  // List of Activators located in the activatable game objects
  private List<Activator> _activatables = new List<Activator>();

  public void Start()
  {
    List<Activator> temp;
    // Visits each intended activatable game object
    foreach (GameObject obj in _activatableObjects)
    {
      // Gets all existing Activators in the game object hierarchy and adds them to the list
      temp = BPUtil.GetComponentsInHierarchy<Activator>(obj.transform);
      foreach(Activator t in temp)
      {
        _activatables.Add(t);
      }
    }
  }

  public override void Left()
  {
    foreach(Activator t in _activatables){
      t.Activate();
    }
  }

  public override void Right()
  {
    foreach (Activator t in _activatables)
    {
      t.Activate();
    }
  }
}