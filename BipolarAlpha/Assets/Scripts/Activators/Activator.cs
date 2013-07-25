using UnityEngine;
using System.Collections;

/// <summary>
/// The Activator class is an abstract class which will be used by game objects that have the need 
/// to be activated or deactivated. 
/// </summary>
public abstract class Activator{

    /// <summary>
    /// Activates the object. 
    /// </summary>
    public abstract void Activate();

    /// <summary>
    /// Deactivates the object. 
    /// </summary>
    public abstract void Deactivate();
}