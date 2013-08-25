//Made By: Engana
using UnityEngine;
using System.Collections;

/// <summary>
/// Listner Interface for Object Room Change Events
/// </summary>
public interface IObjectRoomChangeListner 
{
  void ListenObjectRoomChange(string prevRoomName, string newRoomName, GameObject objectChangedRoom);
}
