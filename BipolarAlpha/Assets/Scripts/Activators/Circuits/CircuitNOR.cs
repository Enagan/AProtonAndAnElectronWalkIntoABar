//MadeBy: Ivo
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Subclass of Circuit implements a NOR logic circuit
/// </summary>
public class CircuitNOR : Circuit
{


  #region Circuit Methods

    /// <summary>
    /// Method used to infer circuit output by looking at input
    /// This method is overriden to infer using the logical operation NOR
    /// <param name="inputsArray">Binary input for the circuit</param>
    /// </summary>
  protected override bool LogicOperation(bool[] inputsArray)
    {
      bool state = false;
      foreach (bool b in inputsArray)
      {
        state = state || b;
      }
      return !state;
    }

  /// <summary>
  /// Method that returns each circuit Name, used for debug
  /// </summary>
  public override string CircuitName()
  {
    return "NOR";
  }
   #endregion
}
