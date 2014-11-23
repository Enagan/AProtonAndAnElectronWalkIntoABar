//---------------------------------------------
// Bipolar
// Written by: Pedro Engana
//---------------------------------------------

using UnityEngine;
using System.Collections;

namespace SMSceneManagerSystem
{
  /// <summary>
  /// A room gateway definition is a special roomObject which represents a connection to another room within the scene manager system.
  /// A gateway must have the name of the room its pointing too, and on the other room, a similar gateway must exist poiting to the original room.
  /// When two rooms are validly connected, the Room Factory can use the relative position of the gateways, in their respective rooms, to position
  /// them adjacent to each other in the world.
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

    public RoomObjectGatewayDefinition() : base() { }

    public RoomObjectGatewayDefinition(string roomConnectedTo, string prefabPath,
      Vector3 position, Vector3 scale, Vector3 eulerAngles)
      : base(prefabPath, position, scale, eulerAngles)
    {
      _connectsToRoom = roomConnectedTo;
    }

  }

}
