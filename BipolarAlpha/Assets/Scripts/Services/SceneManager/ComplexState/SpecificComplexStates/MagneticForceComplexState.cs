﻿//---------------------------------------------
// Bipolar
// Written by: Pedro Engana
//---------------------------------------------

using UnityEngine;
using System.Collections;
using SMSceneManagerSystem;

/// <summary>
/// Complex State for the MagneticForce Class, which represents the magnetic force of a magnet
/// </summary>
public class MagneticForceComplexState : ComplexStateDefinition 
{
  private bool _isActivated;

  /// 0, 1 and 2, representing the cooresponding Enum position in the magnetic force class
  private int _magnetForce;

  /// 0 and 1 representing the cooresponding Enum position in the magnetic force class
  private int _magnetCharge;


  public bool isActive
  {
    get
    {
      return _isActivated;
    }
    set
    {
      _isActivated = value;
    }
  }

  public int magnetForce
  {
    get
    {
      return _magnetForce;
    }
    set
    {
      _magnetForce = value;
    }
  }

  public int magnetCharge
  {
    get
    {
      return _magnetCharge;
    }
    set
    {
      _magnetCharge = value;
    }
  }

  public override string GetComplexStateName()
  {
      return "MagneticForce";
  }

  public MagneticForceComplexState() : base() { }

  public MagneticForceComplexState(GameObject complexStateSourceObject) : base(complexStateSourceObject) { }
}
