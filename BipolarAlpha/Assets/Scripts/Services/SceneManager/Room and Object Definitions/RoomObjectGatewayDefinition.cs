//Made By: Engana
using UnityEngine;
using System.Collections;

/// <summary>
/// Represents a gateway in a room
/// </summary>
public class RoomObjectGatewayDefinition : RoomObjectDefinition 
{
  private string _roomConnected;

  /// <summary>
  /// Name of the room this gateway connects to
  /// </summary>
  public string connectedToRoom
  {
    get
    {
      return _roomConnected;
    }
    set
    {
      _roomConnected = value;
    }
  }

  // Needed for Serialization to work
  public RoomObjectGatewayDefinition() : base() { } 

  public RoomObjectGatewayDefinition(string roomConnectedTo, string prefabPath,
    Vector3 position, Vector3 scale, Vector3 eulerAngles)
    : base(prefabPath, position, scale, eulerAngles)
  {
    _roomConnected = roomConnectedTo;
  }
	
}
