// By: Engana
using UnityEngine;
using System.Collections;

/// <summary>
/// Complex State of the MagneticForce Class
/// </summary>
public class MagneticForceComplexState : ComplexState 
{
  private bool _isActivated;

  /// 0, 1 and 2, representing the cooresponding Enum position
  private int _magnetForce;

  /// 0 and 1 representing the cooresponding Enum position
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

 /// <summary>
 /// Use GameObject Constructor to save you a lot of pain setting paths.
 /// This constructor is for seralization. HANDS OFF
 /// </summary>
  public MagneticForceComplexState() : base() { }

  public MagneticForceComplexState(GameObject complexStateSourceObject) : base(complexStateSourceObject) { }
}
