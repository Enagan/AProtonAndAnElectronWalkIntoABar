// By: Engana

using UnityEngine;
using System.Collections;

/// <summary>
/// Interface that declares a specific scripts needs to save a complexState when its definition is serialized
/// </summary>
public interface IHasComplexState {
  /// <summary>
  /// Creates a Complex State from the ground up, stating every variable worth saving
  /// </summary>
	ComplexState WriteComplexState();

  /// <summary>
  /// Loads into the scritp the ComplexState saved variables given by "state"
  /// </summary>
  void LoadComplexState(ComplexState state);

  /// <summary>
  /// Updates a ComplexState definition with the current runtime values, without having to create a new ComplexState instance
  /// </summary>
  /// <param name="state"></param>
  /// <returns></returns>
  ComplexState UpdateComplexState(ComplexState state);
}
