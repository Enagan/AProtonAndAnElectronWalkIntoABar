//MadeBy: Ivo
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CircuitPulser : CircuitRepeater
{

  #region Circuit Methods


  /// <summary>
  /// Special subclass of Circuit and CircuitRepeater
  /// CircuitPulser inverts its output each event or input cycle 
  /// 
  /// The onFrequencyEvent method is overriden to invert its state at each frequency cycle
  /// 
  /// The logicOperation method is overriden top invert its state at each InputEvent
  /// 
  /// WARNING: CircuitPulser disregards input and progagates inverse state
  /// </summary>
  protected override void OnFrequencyEvent()
  {

    _state = !_state;
  }
  /// <summary>
  /// Method used to infer circuit output by looking at input
  /// Circuit Repeater propagates its input
  /// WARNING: Disregards input and progagates inverse state
  /// <param name="inputsArray">Binary input for the circuit</param>
  /// </summary>
  protected override bool LogicOperation(bool[] inputsArray)
  {
    return !_state;
  }

  /// <summary>
  /// Method that returns each circuit Name, used for debug
  /// </summary>
  public override string CircuitName()
  {
    return "Pulser";
  }
  #endregion
}
