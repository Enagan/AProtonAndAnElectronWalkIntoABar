// By: Pedro Engana

using UnityEngine;
using System.Collections;

namespace SMSceneManagerSystem
{
  /// <summary>
  /// Interface that declares that a specific class needs to be able to save a complex state definition representing its variables
  /// </summary>
  public interface IHasComplexState
  {
    /// <summary>
    /// Needs to return a subclass of a ComplexState, which encapsulates the desired variables that must the saved for the object in question
    /// </summary>
    ComplexStateDefinition WriteComplexStateDefinition();

    /// <summary>
    /// Loads a complex state into the object, reading it's properties and applying them to the gameobject itself
    /// </summary>
    void LoadComplexStateDefinition(ComplexStateDefinition state);

    /// <summary>
    /// Updates the complex state definition properties with the up to date values in the gameobject
    /// </summary>
    ComplexStateDefinition UpdateComplexStateDefinition(ComplexStateDefinition state);
  }
}
