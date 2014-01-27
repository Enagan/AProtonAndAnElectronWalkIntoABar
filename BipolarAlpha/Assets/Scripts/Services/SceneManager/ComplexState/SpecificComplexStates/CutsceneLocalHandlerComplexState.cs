// by Ivo
using UnityEngine;
using System.Collections;

/// <summary>
/// Complex State of the CutsceneLocalHandler Class
/// </summary>
public class CutsceneLocalHandlerComplexState : ComplexState
{
  #region private fields
  
  // Name of Cutscene
  private string _cutsceneName;

  // Radius for Sphere Collision
  private float _collisionRadius;

  #endregion

  #region property getters/setters

  public string cutsceneName
  {
    get
    {
      return _cutsceneName;
    }
    set
    {
      _cutsceneName = value;
    }
  }

  public float collisionRadius
  {
    get
    {
      return _collisionRadius;
    }
    set
    {
      _collisionRadius = value;
    }
  }
  #endregion

  #region complex state methods

  public override string GetComplexStateName()
  {
    return "CutsceneLocalHandler";
  }

 /// <summary>
 /// Use GameObject Constructor to save you a lot of pain setting paths.
 /// This constructor is for seralization. HANDS OFF
 /// </summary>
  public CutsceneLocalHandlerComplexState() : base() { }

  public CutsceneLocalHandlerComplexState(GameObject complexStateSourceObject) : base(complexStateSourceObject) { }

  #endregion
}
