using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BillboardController : JackedInRemoteController
{
  // List of Billboards that can be activated through this
  [SerializeField]
  private List<Billboard> _billboards = new List<Billboard>();

  public void Start()
  {
    // Deactivates all associated billboards at the beginning
    Right();
  }

  public override void Left()
  {
    // Activates all associated billboards
    foreach(Billboard bb in _billboards)
    {
      if (bb.activationType == Billboard.ACTIVATION_TYPE.ACTIVATABLE)
      {
        bb.Activate();
      }
    }
  }

  public override void Right()
  {
    // Deactivates all associated billboards
    foreach (Billboard bb in _billboards)
    {
      if (bb.activationType == Billboard.ACTIVATION_TYPE.ACTIVATABLE)
      {
        bb.Deactivate();
      }
    }
  }
}