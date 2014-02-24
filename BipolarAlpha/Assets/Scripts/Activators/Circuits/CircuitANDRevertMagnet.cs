//MadeBy: Ivo
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Subclass of Circuit implements an AND logic circuit
/// </summary>
public class CircuitANDRevertMagnet : Circuit
{

  [SerializeField]
  MagneticForce targetMagneticForce;

  #region Circuit Methods


  public override void Activate()
  {
    base.Activate();
    if (targetMagneticForce)
    {
      targetMagneticForce.RevertCharge();
    }


  }

  public override void Deactivate()
  {
    base.Deactivate();
    if (targetMagneticForce && targetMagneticForce.isReverted)
    {
      targetMagneticForce.RevertCharge();
    }

  }
  /// <summary>
  /// Method used to infer circuit output by looking at input
  /// This method is overriden to infer using the logical operation AND
  /// <param name="inputsArray">Binary input for the circuit</param>
  /// </summary>
  protected override bool LogicOperation(bool[] inputsArray)
  {
    bool state = true;
    foreach (bool b in inputsArray)
    {
      state = state && b;
    }
    return state;
  }

  /// <summary>
  /// Method that returns each circuit Name, used for debug
  /// </summary>
  public override string CircuitName()
  {
    return "ANDRevertMagnet";
  }
  #endregion
}
