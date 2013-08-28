//MadeBy: Ivo
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Subclass of Circuit implements a NOT logic circuit
/// WARNING: This class only expects to recieve one input and disregards all others
/// </summary>
public class CircuitNOT : Circuit
{


  #region Circuit Methods

    /// <summary>
    /// Method used to infer circuit output by looking at input
    /// This method is overriden to infer using the logical operation NOT
    /// WARNING: This method only considers the first connected input
    /// <param name="inputsArray">Binary input for the circuit</param>
    /// </summary>
  protected override bool LogicOperation(bool[] inputsArray)
  {
    if (inputsArray.Length > 0)
    {
      return !inputsArray[0];
    }
    else
    {
      return false;
    }
  }

  /// <summary>
  /// Method that returns each circuit Name, used for debug
  /// </summary>
  public override string CircuitName()
  {
    return "NOT";
  }
  #endregion
}
