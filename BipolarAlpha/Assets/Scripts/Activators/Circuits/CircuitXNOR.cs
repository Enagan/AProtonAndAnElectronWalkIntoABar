//MadeBy: Ivo
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Subclass of Circuit implements a XNOR logic circuit
/// WARNING: For multiple arguments, XOR is defined to be true if an even number of its arguments are true, and false otherwise. 
/// </summary>
public class CircuitXNOR : Circuit
{


  #region Circuit Methods

    /// <summary>
    /// Method used to infer circuit output by looking at input
    /// This method is overriden to infer using the logical operation XOR
    /// WARNING: For multiple arguments, XOR is defined to be true if an even number of its arguments are true, and false otherwise. 
    /// <param name="inputsArray">Binary input for the circuit</param>
    /// </summary>
    protected override bool LogicOperation(bool[] inputsArray)
    {
      int trueCount = 0;
      foreach (bool b in inputsArray)
      {
        if (b)
          trueCount++;
      }
      return (trueCount % 2) == 0;
    }

    /// <summary>
    /// Method that returns each circuit Name, used for debug
    /// </summary>
    public override string CircuitName()
    {
      return "XNOR";
    }
   #endregion
}
