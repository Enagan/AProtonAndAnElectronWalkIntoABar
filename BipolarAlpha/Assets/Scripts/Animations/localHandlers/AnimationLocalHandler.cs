using UnityEngine;
using System.Collections;

/// <summary>
/// AnimationLocalHandler is the root class of a set of animations
/// Treats player collision and activates cutscene animations depicted in its CutsceneScript
/// It recieves CutsceneElements to be animated by the CutsceneManager via CutsceneTokens
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(CutsceneScript))]
public class AnimationLocalHandler : MonoBehaviour, IHasComplexState
{

  #region variables

  // Name of this particular cutscene, variable is stored in complex state
  [SerializeField]
  protected string _cutsceneName = "";

  // Radius of Sphere Collider that treates player collision, stored in complex state
  [SerializeField]
  protected float _collisionRadius = 0.5f;

  // The cutscene's "script" handles all animation tasks
  // Created by recieving registered CutsceneElements via CutsceneManager
  protected CutsceneScript _script;

  // Player reference stored upon collision for enabling/disabling
  protected PlayerController _player;

  #endregion

  #region properties getters/setters

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

  #region  monobehavior methods
  protected void Start()
  {
    _script = GetComponent<CutsceneScript>();

    this.GetComponent<SphereCollider>().radius = _collisionRadius; // set radius from serialize/complexState

    ServiceLocator.GetCutsceneManager().registerCutsceneHandler(this); // register self in cutsceneManager
  }

  #endregion

  #region trigger methods

  // checks if player IS inside handler's collider
  void OnTriggerStay(Collider other)
  {
    OnTriggerEnter(other);
  }

  // checks if player collided with handler's collider and calls animation
  void OnTriggerEnter(Collider other)
  {
    triggerMethod(other);
  }

  virtual public void triggerMethod(Collider other)
  {
      if (other.tag == "Player") // check if collided entity is player
      {
        BipolarConsole.AllLog("Player in cutscene");
        this.GetComponent<Collider>().enabled = false; // stop future collisions


        // get player components 
        _player = other.GetComponent<PlayerController>();

        if (_player != null)
          _script.startAnimation();
      }
  }

  #endregion


  #region Register CutsceneTokens in script methods

  public void addElement(CutsceneToken token)
  {
    _script.addAnimation(token);
  }

  #endregion

  #region Complex State methods

  // Complex state write
  public ComplexState WriteComplexState()
  {
    // saves this instance cutsceneName
    AnimationLocalHandlerComplexState state = new AnimationLocalHandlerComplexState(this.gameObject);
    state.cutsceneName = _cutsceneName;
    state.collisionRadius = _collisionRadius;

    return state;
  }

  public void LoadComplexState(ComplexState state)
  {
    if (!(state is AnimationLocalHandlerComplexState))
    {
      throw new BipolarExceptionComplexStateNotCompatibleWithScript(state.GetComplexStateName() + " is not compatible with CutsceneLocalHandler class");
    }

    AnimationLocalHandlerComplexState handlerState = ((AnimationLocalHandlerComplexState)state);

    // loads this instance cutsceneName
    _cutsceneName = handlerState.cutsceneName;
    _collisionRadius = handlerState.collisionRadius;


  }

  public ComplexState UpdateComplexState(ComplexState state)
  {
    if (!(state is AnimationLocalHandlerComplexState))
    {
      throw new BipolarExceptionComplexStateNotCompatibleWithScript("Complex State " + state.GetComplexStateName() + " cannot be updated in CutsceneLocalHandler Class");
    }
    AnimationLocalHandlerComplexState specificState = (state as AnimationLocalHandlerComplexState);
    specificState.cutsceneName = _cutsceneName;
    specificState.collisionRadius = _collisionRadius;

    return specificState;
  }


  #endregion
}
