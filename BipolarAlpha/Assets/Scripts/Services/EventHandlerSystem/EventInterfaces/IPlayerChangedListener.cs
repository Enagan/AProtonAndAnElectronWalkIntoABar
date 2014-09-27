using UnityEngine;
using System.Collections;

/// <summary>
/// Listener for pause game events
/// All classes that should use players that may change should implement this
/// </summary>
public interface IPlayerChangedListener 
{

   void ListenPlayerChange(PlayerController newPlayer);

}
