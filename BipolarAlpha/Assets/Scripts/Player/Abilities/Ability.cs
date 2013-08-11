//Made By: Engana
using UnityEngine;
using System.Collections;

/// <summary>
/// Interface defining an Ability
/// An ability can be used, receiving a reference to the caller entity, to allow for state changing
/// </summary>
public interface Ability{

  /// <summary>
  /// Triggers the abilities' effect on it's caller
  /// </summary>
  void Use(PlayerController caller);
	
}
