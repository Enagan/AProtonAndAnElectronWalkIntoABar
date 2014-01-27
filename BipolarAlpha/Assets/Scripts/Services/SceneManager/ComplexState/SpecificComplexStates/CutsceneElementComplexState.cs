// by Ivo
using UnityEngine;
using System.Collections;

/// <summary>
/// Complex State of the CutsceneElement Class
/// </summary>
public class CutsceneElementComplexState : ComplexState
{

  #region private variables

  //Name of animation for cutscene in element's animation
  private string _animationName;

  // Delay for start of animation
  private int _delay;

  // In case element is a rootAnimationHandler and animation is meant for its child
  private string _optionalChild;

 // handler/cutscene name
 private string _cutsceneName;


  #endregion

 #region property getters/setters

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

 #endregion

  #region complex state required methods

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

  #endregion

}
