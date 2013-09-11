using UnityEngine;
using System.Collections;

/// <summary>
/// Circuit that only activates it´s children when it´s activated
/// </summary>
public class CircuitActivateActivator : Circuit {

  /// <summary>
  /// Activates the object and it´s children
  /// </summary>
  public override void Activate()
  {
    _state = true;
    for (int i = 0; i < this.gameObject.transform.childCount; i++)
    {
      Activator actv = (Activator) this.transform.GetChild(i).gameObject.GetComponent("Activator");
      if (actv is Activator)
      {
        actv.Activate();
      }
    }
  }

  /// <summary>
  /// Deactivates the object and it´s children
  /// </summary>
  public override void Deactivate()
  {
    _state = false;
    for (int i = 0; i < this.gameObject.transform.childCount; i++)
    {
      Activator actv = (Activator)this.transform.GetChild(i).gameObject.GetComponent("Activator");
      if (actv is Activator)
      {
        actv.Deactivate();
      }
    }
  }

  #region Circuit Methods

  /// <summary>
  /// Method used to infer circuit output by looking at input
  /// This method is overriden to infer using the logical operation AND
  /// <param name="inputsArray">Binary input for the circuit</param>
  /// </summary>
  protected override bool LogicOperation(bool[] inputsArray)
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
  public override string CircuitName()
  {
    return "Activate Activator";
  }
  #endregion
}
