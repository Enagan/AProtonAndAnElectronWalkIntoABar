using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Circuit that activates its children when it´s activated
/// Circuit that deactivates its children when it´s deactivated
/// </summary>
public class CircuitActivateActivator : Circuit
{
  private void Start()
  {
    if (_state)
    {
      TurnOnLights();
    }
    else
    {
      TurnOffLights();
    }
  }

  #region Activator Methods
  /// <summary>
  /// Activates the object and its children
  /// </summary>
  public override void Activate()
  {
    // Calls the components' activate method
    _state = true;

    // Fetches all Activators in the hierarchy of this object
    List<Activator> actvs = BipolarUtilityFunctions.GetComponentsInHierarchy<Activator>(this.gameObject.transform);

    foreach (Activator act in actvs) {

      // Confirms the current Activator is an same instance of CircuitActivateActivator and
      // activates the Activator in the hierarchy

      // NOTE: It might not be desired to activate circuits
      // (just remove "ActivateActivator" from the 'if' bellow to produce that effect)
      if (!(act is CircuitActivateActivator))
      {
        act.Activate();
      }
    }

    TurnOnLights();
  }

  /// <summary>
  /// Deactivates the object and its children
  /// </summary>
  public override void Deactivate()
  {
    // Calls the components' deactivate method
    _state = false;

    // Fetches all Activators in the hierarchy of this object
    List<Activator> actvs = BipolarUtilityFunctions.GetComponentsInHierarchy<Activator>(this.gameObject.transform);

    foreach (Activator act in actvs)
    {
      // Confirms the current Activator is not an instance of CircuitActivateActivator and
      // deactivates the Activator in the hierarchy

      // NOTE: It might not be desired to deactivate circuits 
      // (just remove "ActivateActivator" from the 'if' bellow to produce that effect)
      if (!(act is CircuitActivateActivator))
      {
        act.Deactivate();
      }
    }

    TurnOffLights();
  }
  #endregion

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
