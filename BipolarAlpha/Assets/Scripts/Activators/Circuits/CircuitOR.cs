using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Subclass of Circuit implements an OR logic circuit
/// </summary>
public class CircuitOR : Circuit
{


  #region Circuit Methods

    /// <summary>
    /// Method used to infer circuit output by looking at input
    /// This method is overriden to infer using the logical operation OR
    /// <param name="inputsArray">Binary input for the circuit</param>
    /// </summary>
    protected override bool logicOperation(bool[] inputsArray)
    {
      bool state = false;
      foreach (bool b in inputsArray)
      {
        state = state || b;
      }
      return state;
    }

    /// <summary>
    /// Method that returns each circuit Name, used for debug
    /// </summary>
    public override string circuitName()
    {
      return "OR";
    }
   #endregion
}
