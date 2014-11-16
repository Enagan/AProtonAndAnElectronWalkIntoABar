// by Ivo Capelo
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SMSceneManagerSystem;

/// <summary>
/// Complex State of the CutsceneElement Class
/// </summary>
public class CutsceneElementComplexState : ComplexStateDefinition
{

  #region private variables

  //Name of animation for cutscene in element's animation
  private List<string> _animationNames;

  // Delay for start of animation
  private List<int> _delays;

  // In case element is a rootAnimationHandler and animation is meant for its child
  private List<string> _optionalChilds;

 // handler/cutscene name
  private string _cutsceneName;


  #endregion

 #region property getters/setters

  public List<string> animationNames
 {
   get
   {
     return _animationNames;
   }
   set
   {
     _animationNames = value;
   }
 }

 public List<int> delays
 {
   get
   {
     return _delays;
   }
   set
   {
     _delays = value;
   }
 }

 public List<string> optionalChilds
 {
   get
   {
     return _optionalChilds;
   }
   set
   {
     _optionalChilds = value;
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
