//Made by: Rui
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// </summary>
public class Railgun : MonoBehaviour, Activator
{
  #region Railgun Constants
  private const float FORCEMULTIPLIER = 50.0f;
  #endregion

  #region Railgun Variables
  [SerializeField]
  private bool _isActivated = false;
  #endregion

  #region Railgun Properties

  public bool isActivated
  {
    get { return _isActivated; }
  }
  #endregion

  public void Activate()
  {
    _isActivated = true;
  }

  public void Deactivate()
  {
    _isActivated = false;
  }

  public virtual void Start() { }

  private void OnTriggerEnter(Collider other)
  {
    if(_isActivated && other.tag == "Player")
    {
      other.rigidbody.AddForce( -1 * this.transform.right * FORCEMULTIPLIER, ForceMode.Force);
      ForceComponent.applyForce(other.gameObject, -1 * this.transform.right * FORCEMULTIPLIER, 5.0f);
    }
  }
}