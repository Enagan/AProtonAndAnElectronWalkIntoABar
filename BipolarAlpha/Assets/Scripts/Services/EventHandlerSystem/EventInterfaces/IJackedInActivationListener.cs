using UnityEngine;
using System.Collections;

/// <summary>
/// Listener for Jacked-In mode activation events
/// All classes that should act on pause should implement this interface
/// </summary>
public interface IJackedInActivationListener
{
  void ListenJackedInActivation(Camera camera);

  void ListenJackedInDeactivation();
}
