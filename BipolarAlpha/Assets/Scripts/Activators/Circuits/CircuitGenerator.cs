//MadeBy: Ivo
using UnityEngine;
using System.Collections;

// Class that represents a generator's output circuit
public class CircuitGenerator : Circuit {

  #region  private fields

  // Angular Velocity Magnitude Threshold value
  [SerializeField]
  private float _activationThreshold = 5.0f;

  #endregion

  #region Monobehavior Methods

  /// <summary>
  /// Update will verify if rotation threshold was achieved
  /// </summary>
  public void Update()
  {
    if (this.transform.rigidbody.angularVelocity.magnitude < _activationThreshold)
    {
      if (_state)  //Check to only change once
      {
        // Circuit is disabled
        Deactivate();
      }
    }
    else
    {
      if (!_state)  //Check to only change once
      {
        // Circuit is enabled
        Activate();
      }
    }
  }

  #endregion

  #region Circuit Methods

  /// <summary>
  /// Method used to infer circuit output by looking at input
  /// This method is overriden to implement Circuit's interface
  /// </summary>
  protected override bool LogicOperation(bool[] inputsArray)
  {
      return _state;
  }

  /// <summary>
  /// Method that returns each circuit Name, used for debug
  /// </summary>
  public override string CircuitName()
  {
    return "Generator";
  }
  #endregion
}
