// by Ivo
using UnityEngine;
using System.Collections;

/// <summary>
/// Complex State of the CutsceneLocalHandler Class
/// </summary>
public class CutsceneLocalHandlerComplexState : ComplexState
{

  private string _cutsceneName;


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
    return "CutsceneLocalHandler";
  }

 /// <summary>
 /// Use GameObject Constructor to save you a lot of pain setting paths.
 /// This constructor is for seralization. HANDS OFF
 /// </summary>
  public CutsceneLocalHandlerComplexState() : base() { }

  public CutsceneLocalHandlerComplexState(GameObject complexStateSourceObject) : base(complexStateSourceObject) { }
}
