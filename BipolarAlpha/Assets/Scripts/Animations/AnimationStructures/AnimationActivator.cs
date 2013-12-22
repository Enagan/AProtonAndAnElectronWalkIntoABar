using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class AnimationActivator : MonoBehaviour, Activator 
{

  #region variables
  //List containing all ChildAnimations that open/close
  private List<AnimationChildHandler> _childAnimators = new List<AnimationChildHandler>();

  //RootAnimationHandler for searching children
  protected AnimationRootHandler _handler = null;

  #endregion

  #region OnAwake Methods

  public virtual void Start()
  {
    initializeHandler();
  }

  // Initializes openclose children by calling addChild(string anim)
  public abstract void initializeHandler();

  //Adds a child with open/close animation
  public void addChild(string anim)
  {
    //Initialize handler first time
    if (_handler == null)
      _handler = transform.GetComponent<AnimationRootHandler>();

    //Add child
    _childAnimators.Add(_handler.getChildHandler(anim));
  }

  #endregion

  #region Activator Methods
  //Override of activate opens all children
  public virtual void Activate()
  {
    if (_handler == null)
      initializeHandler();

    //Call child animations
    foreach (AnimationChildHandler anim in _childAnimators)
    {
      anim.getAnimation().CrossFade(anim.childName + "Open");
    }
  }

  //Override of deactivate closes all children
  public virtual void Deactivate()
  {
    //Call child animations
    foreach (AnimationChildHandler anim in _childAnimators)
    {
      anim.getAnimation().CrossFade(anim.childName + "Close");
    }
  }

  #endregion
}
