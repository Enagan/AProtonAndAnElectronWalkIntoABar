using UnityEngine;
using System.Collections;

/// <summary>
/// CutsceneElement stores information necessary to link Animations to a cutscene
/// This is achieved by the IHasComplexState and the CutsceneManager, registration is handled automatically
/// All AnimationHandlers need to be a CutsceneElement before being added to a cutscene
/// One CutsceneElement per animation
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
  string _animationName = "";

  // Delay for animation to start during cutscene
  [SerializeField]
  int _delay = 0;

  // Name of child if element is a roothandler and animation belongs to a childhandler
  [SerializeField]
  string _optionalChildName = "";

  #endregion

  #region start method

  void Start()
  {
    AnimationChildHandler animHandler = this.GetComponent<AnimationChildHandler>();
    ServiceLocator.GetCutsceneManager().registerElement(_cutsceneName, animHandler, _animationName, _delay, _optionalChildName);
  }

  #endregion

  #region Complex State methods

  // Complex state write
  public ComplexState WriteComplexState()
  {
    // saves this instance cutsceneName
    CutsceneElementComplexState state = new CutsceneElementComplexState(this.gameObject);
    state.cutsceneName = _cutsceneName;
    state.animationName = _animationName;
    state.delay = _delay;
    state.optionalChild = _optionalChildName;

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
    _animationName = elementState.animationName;
    _delay = elementState.delay;
    _optionalChildName = elementState.optionalChild;
  }

  public ComplexState UpdateComplexState(ComplexState state)
  {
    if (!(state is CutsceneElementComplexState))
    {
      throw new BipolarExceptionComplexStateNotCompatibleWithScript("Complex State " + state.GetComplexStateName() + " cannot be updated in CutsceneElement Class");
    }
    CutsceneElementComplexState specificState = (state as CutsceneElementComplexState);
    specificState.cutsceneName = _cutsceneName;
    specificState.animationName = _animationName;
    specificState.delay = _delay;
    specificState.optionalChild = _optionalChildName;

    return specificState;
  }


  #endregion
}
