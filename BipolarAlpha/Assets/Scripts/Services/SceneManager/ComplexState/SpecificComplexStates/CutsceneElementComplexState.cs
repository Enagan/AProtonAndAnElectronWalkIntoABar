// by Ivo
using UnityEngine;
using System.Collections;

/// <summary>
/// Complex State of the CutsceneElement Class
/// </summary>
public class CutsceneElementComplexState : ComplexState
{

  private string _animationName;

  private int _delay;

  /// <summary>
  /// In case element is a root handler and animation is for child
  /// </summary>
  private string _optionalChild;

 // handler/cutscene name
 private string _cutsceneName;

 public string animationName
 {
   get
   {
     return _animationName;
   }
   set
   {
     _animationName = value;
   }
 }

 public int delay
 {
   get
   {
     return _delay;
   }
   set
   {
     _delay = value;
   }
 }

 public string optionalChild
 {
   get
   {
     return _optionalChild;
   }
   set
   {
     _optionalChild = value;
   }
 }

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

  public override string GetComplexStateName()
  {
    return "CutsceneElement";
  }

 /// <summary>
 /// Use GameObject Constructor to save you a lot of pain setting paths.
 /// This constructor is for seralization. HANDS OFF
 /// </summary>
  public CutsceneElementComplexState() : base() { }

  public CutsceneElementComplexState(GameObject complexStateSourceObject) : base(complexStateSourceObject) { }
}
