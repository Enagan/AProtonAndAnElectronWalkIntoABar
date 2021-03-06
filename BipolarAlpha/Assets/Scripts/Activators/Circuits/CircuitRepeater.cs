﻿//MadeBy: Ivo
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Special subclass of Circuit
/// CircuitRepeater propagates output during an Input Events and according to frequency
/// 
/// Frequency can be edited using the _repeatFrequency variable
/// 
/// Each frequency cycle it calls the onFrequencyEvent method, 
/// The onFrequencyEvent method should be overriden by children who wish to add behaviour
/// 
/// WARNING: CircuitRepeater progates its first input only by directly updating its state
/// </summary>
public class CircuitRepeater : Circuit
{

  #region private fields
  // Frequency time in seconds to repeat sending its state
  [SerializeField]
  private float _repeatFrequency;
  #endregion

  private float _accumulatedTime =0;

  #region Circuit Methods

  /// <summary>
  /// Update will call state on output everytime it reaches frequency
  /// </summary>
 
  void Update()
  {
    _accumulatedTime += Time.deltaTime;
    if (_accumulatedTime >= _repeatFrequency)
    {
      OnFrequencyEvent();
      _accumulatedTime = 0.0f;
    
      // propagates state to output
      PropagateToOutputs();
    }
  }

  /// <summary>
  /// Method called when a frequency event happens
  /// Stub on CircuitRepeater, meant to be overriden by children
  /// </summary>
  protected virtual void OnFrequencyEvent()
  {}

  /// <summary>
  /// Method used to infer circuit output by looking at input
  /// CircuitRepeater propagates its input
  /// WARNING: CircuitRepeater progates its first input only
  /// <param name="inputsArray">Binary input for the circuit</param>
  /// </summary>
  protected override bool LogicOperation(bool[] inputsArray)
  {
    if (inputsArray.Length > 0)
    {
      return inputsArray[0];
    }
    else return false;
  }
  
  /// <summary>
  /// Method that returns each circuit Name, used for debug
  /// </summary>
  public override string CircuitName()
  {
    return "Repeater";
  }
  #endregion
}
