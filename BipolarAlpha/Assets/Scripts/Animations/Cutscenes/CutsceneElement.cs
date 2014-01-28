using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// CutsceneElement stores information necessary to link Animations to a cutscene
/// This is achieved by the IHasComplexState and the CutsceneManager, registration is handled automatically
/// All AnimationHandlers need to be a CutsceneElement before being added to a cutscene
/// One CutsceneElement per cutscene, but many animations
/// </summary>
[RequireComponent(typeof(AnimationChildHandler))]
public class CutsceneElement : MonoBehaviour, IHasComplexState
{

  #region private variables

  // Name of Cutscene/CutsceneHandler for this animation/element combination
  [SerializeField]
  string _cutsceneName = "";

  // Name of animation to be called by this element's animationHandler
  [SerializeField]
  List<string> _animationNames = new List<string>();
  

  // Delay for animation to start during cutscene
  [SerializeField]
  List<int> _delays = new List<int>();
  

  // Name of child if element is a roothandler and animation belongs to a childhandler
  [SerializeField]
  List<string> _childNames = new List<string>();


  #endregion

  #region start method

  void Start()
  {
    AnimationChildHandler animHandler = this.GetComponent<AnimationChildHandler>();
    
    // Find minimum common value
    int count = Mathf.Min(Mathf.Min(_animationNames.Count, _delays.Count), _childNames.Count);
    for (int i = 0; i < count; i++)
    {
      ServiceLocator.GetCutsceneManager().registerElement(_cutsceneName, animHandler, _animationNames[i], _delays[i], _childNames[i]);
    }
  }

  #endregion

  #region Complex State methods

  // Complex state write
  public ComplexState WriteComplexState()
  {
    // saves this instance cutsceneName
    CutsceneElementComplexState state = new CutsceneElementComplexState(this.gameObject);
    state.cutsceneName = _cutsceneName;
    state.animationNames = _animationNames;
    state.delays = _delays;
    state.optionalChilds = _childNames;

    return state;
  }

  public void LoadComplexState(ComplexState state)
  {
    if (!(state is CutsceneElementComplexState))
    {
      throw new BipolarExceptionComplexStateNotCompatibleWithScript(state.GetComplexStateName() + " is not compatible with CutsceneElement class");
    }

    CutsceneElementComplexState elementState = ((CutsceneElementComplexState)state);

    // loads this instance's fields

    _cutsceneName = elementState.cutsceneName;
    _animationNames = elementState.animationNames;
    _delays = elementState.delays;
    _childNames = elementState.optionalChilds;
  }

  public ComplexState UpdateComplexState(ComplexState state)
  {
    if (!(state is CutsceneElementComplexState))
    {
      throw new BipolarExceptionComplexStateNotCompatibleWithScript("Complex State " + state.GetComplexStateName() + " cannot be updated in CutsceneElement Class");
    }
    CutsceneElementComplexState specificState = (state as CutsceneElementComplexState);
    specificState.cutsceneName = _cutsceneName;
    specificState.animationNames = _animationNames;
    specificState.delays = _delays;
    specificState.optionalChilds = _childNames;

    return specificState;
  }


  #endregion
}
