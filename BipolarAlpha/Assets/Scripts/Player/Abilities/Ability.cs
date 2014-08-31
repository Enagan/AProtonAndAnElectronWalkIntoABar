//Made By: Engana
using UnityEngine;
using System.Collections;

/// <summary>
/// Interface defining an Ability
/// An ability can be used, receiving a reference to the caller entity, to allow for state changing
/// </summary>
public interface Ability{

  /// <summary>
  /// Continuosly Triggers the abilities' effect
  /// </summary>
  void Use(string key = null);
	
  /// <summary>
  /// The key that activates the ability has been pressed down
  /// </summary>
  void KeyDown(string key = null);

  /// <summary>
  /// The key that activates the ability has been lifted up
  /// </summary>
  void KeyUp(string key = null);

}
