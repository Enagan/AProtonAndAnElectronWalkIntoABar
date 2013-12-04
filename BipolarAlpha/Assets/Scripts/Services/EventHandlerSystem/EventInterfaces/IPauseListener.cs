using UnityEngine;
using System.Collections;

/// <summary>
/// Listener for pause game events
/// All classes that should act on pause should implement this interface
/// </summary>
public interface IPauseListener
{
  void ListenPause(bool isPaused);
}
