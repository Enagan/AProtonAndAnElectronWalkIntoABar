using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SMSceneManagerSystem
{
  /// <summary>
  /// A world state definition holds the entire information needed to dynamically build the world
  /// We have a list of Rooms definitions which define all the different areas of the world, as well as the starting room, refrencing the origin room
  /// </summary>
  public class WorldStateDefinition
  {
    private string _startingRoom;

    private List<RoomDefinition> _roomsDefinedInState;

    public string startingRoom
    {
      get
      {
        return _startingRoom;
      }
      set
      {
        _startingRoom = value;
      }
    }

    public List<RoomDefinition> roomsDefinedInState
    {
      get
      {
        return _roomsDefinedInState;
      }
      set
      {
        _roomsDefinedInState = value;
      }
    }
    public WorldStateDefinition()
    {
      _startingRoom = null;
      _roomsDefinedInState = new List<RoomDefinition>();
    }

    public WorldStateDefinition(List<RoomDefinition> rooms, string startingRoom)
    {
      _startingRoom = startingRoom;
      _roomsDefinedInState = rooms;
    }

  }
}
