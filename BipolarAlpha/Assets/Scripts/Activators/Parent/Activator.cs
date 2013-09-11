//Made by: Lousada
using UnityEngine;
using System.Collections;

/// <summary>
/// The Activator interface is an interface which will be used by game objects that have the need 
/// to be activated or deactivated. 
/// </summary>
public interface Activator
{

    /// <summary>
    /// Activates the object. 
    /// </summary>
    void Activate();

    /// <summary>
    /// Deactivates the object. 
    /// </summary>
    void Deactivate();
}