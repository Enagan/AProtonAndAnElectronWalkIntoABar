//Made By: Engana
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Enum designed to be used only inside the system to ease in archiving and registring listners
/// </summary>
enum BipolarEvent { PlayerRoomChange }

/// <summary>
/// System designed to handle events specific to bipolar, in situations where a lot of different
/// classes will have to observe certain happenings ingame.
/// </summary>
public class BipolarEventHandlerSystem
{
  Dictionary<BipolarEvent, List<object>> _listners = new Dictionary<BipolarEvent, List<object>>();

  #region Register Listners

  /// <summary>
  /// Registers a new listner for Player Room Change Events
  /// </summary>
  public void RegisterPlayerRoomChangeListner(IPlayerRoomChangeListner listner)
  {
    RegisterEventListner(BipolarEvent.PlayerRoomChange, listner);
  }

  /// <summary>
  /// Master Register event function, receives the cooresponding enum, and listner
  /// archiving them for future sending of events
  /// </summary>
  private void RegisterEventListner(BipolarEvent eventType, object listner)
  {
    if (!_listners.ContainsKey(eventType))
    {
      _listners.Add(eventType, new List<object>());
    }
    _listners[eventType].Add(listner);
  }
  #endregion

  #region Send Events
  /// <summary>
  /// Sends a Player Room Change Event, should receive the new room's name
  /// </summary>
  public void SendPlayerRoomChangeEvent(string newRoomName)
  {
    BipolarConsole.EnganaLog("BroadcastingRoomChange");
    foreach (IPlayerRoomChangeListner listner in _listners[BipolarEvent.PlayerRoomChange])
    {
      listner.ListenPlayerRoomChange(newRoomName);
    }
  }
  #endregion

}
