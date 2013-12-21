using UnityEngine;
using System.Collections;

public class MagnetDispenserGenerator : MonoBehaviour
{

  #region  private fields

  // Angular Velocity Magnitude Threshold value
  [SerializeField]
  private float _activationThreshold = 5.0f;
  [SerializeField]
  private bool _canBeActivated = true;


  #endregion

  /// <summary>
  /// Update will verify if rotation threshold was achieved
  /// </summary>
  public void Update()
  {
    if (this.transform.rigidbody.angularVelocity.magnitude > _activationThreshold && _canBeActivated)
    {
      _canBeActivated = false;
      this.transform.parent.transform.parent.GetComponentInChildren<DispenserActivator>().Activate();
    }
  }

  public void Reset() 
  {
    this.transform.rigidbody.angularVelocity = Vector3.zero;
    _canBeActivated = true;
}
}