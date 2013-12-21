using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This is a class to help call animations on circuit structures that open/close
/// Examples: Door, Dispenser
/// </summary>
public abstract class OpenCloseAnimationHelperCircuit : Circuit
{
  #region variables
  //List containing all ChildAnimations that open/close
  private List<AnimationChildHandler> _childAnimators = new List<AnimationChildHandler>();

  //RootAnimationHandler for searching children
  protected AnimationRootHandler _handler = null;

  #endregion

  #region OpenCLoseAnimation helper children methods

  // Method to be overriden by children
  // Children need initialize their own openclose children by calling addChild(string anim)
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

  #region circuit methods
  //Override of activate opens all children
  public override void Activate()
  {
    _state = true;

    if (_handler == null)
      initializeHandler();

    //Call child animations
    foreach (AnimationChildHandler anim in _childAnimators)
    {
      anim.getAnimation().CrossFade(anim.childName + "Open");
    }
  }

  //Override of deactivate closes all children
  public override void Deactivate()
  {
    _state = false;

    //Call child animations
    foreach (AnimationChildHandler anim in _childAnimators)
    {
      anim.getAnimation().CrossFade(anim.childName + "Close");
    }
  }

  /// <summary>
  /// Stub doesn't implement logic
  /// </summary>
  protected override bool LogicOperation(bool[] inputsArray)
  {
    if (inputsArray.Length > 0)
      return inputsArray[0];
    else
      return false;
  }

  #endregion
}
