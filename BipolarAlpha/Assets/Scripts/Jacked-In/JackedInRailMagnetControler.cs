using UnityEngine;
using System.Collections;

public class JackedInRailMagnetControler : JackedInRemoteController {

  private const float RAIL_SPEED = 10f;

  public override void Left()
  {
    foreach (GameObject go in _activatableObjects)
    {
      MoveFoward(go);
    }

  }

  public override void Right()
  {
    foreach (GameObject go in _activatableObjects)
    {
      MoveBackwards(go);
    }
  }

  private void MoveFoward(GameObject go)
  {
    go.transform.position += go.transform.forward * RAIL_SPEED * Time.deltaTime;
  }

  private void MoveBackwards(GameObject go)
  {
   go.transform.position -= go.transform.forward * RAIL_SPEED * Time.deltaTime;
  }
}
