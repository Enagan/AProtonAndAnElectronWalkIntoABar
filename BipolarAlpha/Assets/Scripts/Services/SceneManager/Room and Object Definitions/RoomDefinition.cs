//Made By: Pedro Engana
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Xml.Serialization;

namespace SMSceneManagerSystem
{
  /// <summary>
  /// Room definition represents the DNA of how to build the room in the state it was previously saved.
  /// A Room is defined with a name and a set of object and gateway definitions that fully define the state of everything inside the room
  /// Gateways represent the borders of a room, and it's through them that we define how and where a room connects to other rooms
  /// </summary>
  public class RoomDefinition
  {
    private string _roomName;
    private List<RoomObjectDefinition> _objectsInRoom = new List<RoomObjectDefinition>();
    private List<RoomObjectGatewayDefinition> _gateways = new List<RoomObjectGatewayDefinition>();


    private Dictionary<int, List<Collider>> _colliders = new Dictionary<int, List<Collider>>();
    private List<Renderer> _renderers = new List<Renderer>();
    private int _maxDepth = 0;

    public RoomDefinition() { }

    public RoomDefinition(string name)
    {
      _roomName = name;
    }

    public string roomName
    {
      get
      {
        return _roomName;
      }
      set
      {
        _roomName = value;
      }
    }

    public List<RoomObjectDefinition> objectsInRoom
    {
      get
      {
        return _objectsInRoom;
      }
      set
      {
        _objectsInRoom = value;
      }
    }

    public List<RoomObjectGatewayDefinition> gateways
    {
      get
      {
        return _gateways;
      }
      set
      {
        _gateways = value;
      }
    }


    [XmlIgnore]
    public List<Renderer> renderers
    {
      get
      {
        return _renderers;
      }
      set
      {
        _renderers = value;
      }
    }

    /// <summary>
    /// All the mesh colliders in a room
    /// </summary>
    [XmlIgnore]
    public Dictionary<int, List<Collider>> colliders
    {
      get
      {
        return _colliders;
      }
    }

    /// <summary>
    /// Max Children depth with mesh colliders
    /// </summary>
    public int maxDepth
    {
      get
      {
        return _maxDepth;
      }
      set
      {
        _maxDepth = value;
      }
    }

    public void AddObjectDefinition(RoomObjectDefinition obj)
    {
      _objectsInRoom.Add(obj);
    }

    public void AddGatewayDefinition(RoomObjectGatewayDefinition gateway)
    {
      _gateways.Add(gateway);
    }

    /// <summary>
    /// Retrieves a gateway that connects to the given room, returns null in case such a gateway does not exist
    /// </summary>
    public RoomObjectGatewayDefinition GetDefinitionOfGatewayToRoom(RoomDefinition roomDef)
    {
      return GetDefinitionOfGatewayToRoom(roomDef.roomName);
    }

    public RoomObjectGatewayDefinition GetDefinitionOfGatewayToRoom(string destinationRoom)
    {
      return _gateways.Find(gatewayDefinition => gatewayDefinition.connectsToRoom == destinationRoom);
    }

    public bool Equals(RoomDefinition roomDef)
    {
      return roomName == roomDef.roomName;
    }
  }
}
