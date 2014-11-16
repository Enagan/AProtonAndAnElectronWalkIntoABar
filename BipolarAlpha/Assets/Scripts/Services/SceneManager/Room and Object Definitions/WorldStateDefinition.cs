using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
