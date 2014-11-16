//Made By: Engana
using UnityEngine;
using System.Collections;

/// <summary>
/// Represents a gateway in a room
/// </summary>
public class RoomObjectGatewayDefinition : RoomObjectDefinition 
{
  private string _connectsToRoom;

  /// <summary>
  /// Name of the room this gateway connects to
  /// </summary>
  public string connectsToRoom
  {
    get
    {
      return _connectsToRoom;
    }
    set
    {
      _connectsToRoom = value;
    }
  }

  // Needed for Serialization to work
  public RoomObjectGatewayDefinition() : base() { } 

  public RoomObjectGatewayDefinition(string roomConnectedTo, string prefabPath,
    Vector3 position, Vector3 scale, Vector3 eulerAngles)
    : base(prefabPath, position, scale, eulerAngles)
  {
    _connectsToRoom = roomConnectedTo;
  }
	
}
