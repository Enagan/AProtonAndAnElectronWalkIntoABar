using UnityEngine;
using System;

class ForceComponent : MonoBehaviour
{
  #region Variables
  private Vector3 _force = Vector3.zero;
  private float _time = 0.0f;
  #endregion

  private void Start()
  {
    if(this.rigidbody == null)
    {

        SMConsole.Log("ForceComponent not associated with a rigid body.", "Debug", SMLogType.ERROR);
    }
  }

  private void Update()
  {
    if(_time <= 0.0f)
    {
      Destroy(this);
    }
    this.rigidbody.AddForce(_force*_time, ForceMode.Impulse);
    _time -= Time.deltaTime;
  }

  public void applyForce(Vector3 force, float duration)
  {
    _time = duration;
    _force = force;
  }

  public static void applyForce(GameObject target, Vector3 force, float duration) 
  {
    ForceComponent fc = target.AddComponent<ForceComponent>();
    fc.applyForce(force, duration);
  }
}
