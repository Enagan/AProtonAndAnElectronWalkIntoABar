using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AnimationChildHandler))]
public class CutsceneElement : MonoBehaviour, IHasComplexState {

  [SerializeField]
  string _cutsceneName = "";

  [SerializeField]
  string _animationName = "";

  [SerializeField]
  int _delay = 0;

  [SerializeField]
  string _optionalChildName = "";

  void Start()
  {
    AnimationChildHandler animHandler = this.GetComponent<AnimationChildHandler>();
    ServiceLocator.GetCutsceneManager().registerElement(_cutsceneName, animHandler, _animationName, _delay, _optionalChildName);
  }

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
