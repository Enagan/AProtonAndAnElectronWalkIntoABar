using UnityEngine;
using System.Collections;

public class JackedInRailMagnetControler : JackedInRemoteController {

  [SerializeField]
  private float _railSpeed = 10f;

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
    go.transform.position += go.transform.forward * _railSpeed * Time.deltaTime;
  }

  private void MoveBackwards(GameObject go)
  {
    go.transform.position -= go.transform.forward * _railSpeed * Time.deltaTime;
  }
}
