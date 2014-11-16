// By: Pedro Engana
using UnityEngine;
using System.Collections;
using SMSceneManagerSystem;

/// <summary>
/// Complex State of the MagneticForce Class
/// </summary>
public class WeightPlatformComplexState : ComplexStateDefinition 
{
  // Resting height for platform
  private float _restingHeight;

  // Acceleration of platform
  private float _acceleration;

  // Height of the room's floor in actual scene
  private float _floorHeight;

  private float _startHeight;

  public float startHeight
  {
    get
    {
      return _startHeight;
    }
    set
    {
      _startHeight = value;
    }
  }


  public float floorHeight
  {
    get
    {
      return _floorHeight;
    }
    set
    {
      _floorHeight = value;
    }
  }

  public float restingHeight
  {
    get
    {
      return _restingHeight;
    }
    set
    {
      _restingHeight = value;
    }
  }

  public float acceleration
  {
    get
    {
      return _acceleration;
    }
    set
    {
      _acceleration = value;
    }
  }


  public override string GetComplexStateName()
  {
      return "WeightPlatform";
  }

 /// <summary>
 /// Use GameObject Constructor to save you a lot of pain setting paths.
 /// This constructor is for seralization. HANDS OFF
 /// </summary>
  public WeightPlatformComplexState() : base() { }

  public WeightPlatformComplexState(GameObject complexStateSourceObject) : base(complexStateSourceObject) { }
}
