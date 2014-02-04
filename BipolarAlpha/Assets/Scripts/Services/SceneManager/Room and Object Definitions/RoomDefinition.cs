//Made By: Engana
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Xml.Serialization;

/// <summary>
/// Room definition represents a room.
/// A Room is defined with a name and a set of objects and gateways
/// </summary>
public class RoomDefinition
{
  private string _roomName;

  private bool _constructionFinished = false;

  private List<RoomObjectDefinition> _objectsInRoom = new List<RoomObjectDefinition>();

  private Dictionary<int, List<MeshCollider>> _meshColliders = new Dictionary<int, List<MeshCollider>>();
  private int _maxDepth = 0;

  // Gateways are treated like special objects because they are in charge of transitioning between rooms
  // Gateways possess the name of the room they link to
  private List<RoomObjectGatewayDefinition> _gateways = new List<RoomObjectGatewayDefinition>();

  
  /// <summary>
  /// The name of the room
  /// </summary>
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

  /// <summary>
  /// List with all the objects in the room
  /// </summary>
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

  /// <summary>
  /// List with all gateways in the room
  /// </summary>
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

  /// <summary>
  /// All the mesh colliders in a room
  /// </summary>
  [XmlIgnore]
  public Dictionary<int, List<MeshCollider>> meshColliders
  {
    get
    {
      return _meshColliders;
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

  /// <summary>
  /// Is the construction done
  /// </summary>
  public bool constructionFinished
  {
    get
    {
      return _constructionFinished;
    }
    set
    {
      _constructionFinished = value;
    }
  }

  // Needed for Serialization to work
  public RoomDefinition() { }

  public RoomDefinition(string name) 
  { 
    _roomName = name; 
  }

  public void AddObject(RoomObjectDefinition obj)
  {
    _objectsInRoom.Add(obj);
  }

  public void AddGateway(RoomObjectGatewayDefinition gateway)
  {
    _gateways.Add(gateway);
  }

  /// <summary>
  /// Retrieves a gateway that connects to the given room
  /// returns null in case such a gateway does not exist
  /// </summary>
  public RoomObjectGatewayDefinition GetGatewayTo(string destinationRoom)
  {
    foreach (RoomObjectGatewayDefinition gate in _gateways)
    {
      if (gate.connectedToRoom == destinationRoom)
      {
        return gate;
      }
    }
    return null;
  }

  /// <summary>
  /// Retrieves a gateway that connects to the given room
  /// returns null in case such a gateway does not exist
  /// </summary>
  public RoomObjectGatewayDefinition GetGatewayTo(RoomDefinition roomDef)
  {
    return GetGatewayTo(roomDef.roomName);
  }

  /// <summary>
  /// Compares this room with the given room
  /// </summary>
  public bool Equals(RoomDefinition roomDef)
  {
    return roomName == roomDef.roomName;
  }
  
}
