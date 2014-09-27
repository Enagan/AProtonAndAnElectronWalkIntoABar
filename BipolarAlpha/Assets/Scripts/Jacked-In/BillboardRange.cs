using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BillboardRange : MonoBehaviour
{
  // List of Billboards that can be activated through this
  [SerializeField]
  private List<Billboard> _billboards = new List<Billboard>();

  public void Start()
  {
    // Deactivates all associated billboards at the beginning
    Deactivate();
  }

  private void Activate()
  {
    // Activates all associated billboards
    foreach (Billboard bb in _billboards)
    {
      if (bb.activationType == Billboard.ACTIVATION_TYPE.RANGE)
      {
        bb.Activate();
      }
    }
  }

  private void Deactivate()
  {
    // Activates all associated billboards
    foreach (Billboard bb in _billboards)
    {
      if (bb.activationType == Billboard.ACTIVATION_TYPE.RANGE)
      {
        bb.Deactivate();
      }
    }
  }

  public void OnTriggerStay(Collider col)
  {
    if (col.tag == "Player")
    {
      Activate();
    }
  }

  public void OnTriggerExit(Collider col)
  {
    if (col.tag == "Player")
    {
      Deactivate();
    }
  }
}