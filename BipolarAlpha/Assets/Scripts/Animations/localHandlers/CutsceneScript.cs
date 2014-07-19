using UnityEngine;
using System;
using System.Collections.Generic;

#region Cutscene Token
/// <summary>
/// Cutscene Token represents a CutsceneElement and the required variables to run it in a cutscene
/// A structure useful for transporting cutscene information in collections, across intances and for complexStates
/// Implements IComparable to allow being sorted by delay
/// </summary>
public struct CutsceneToken : IComparable
{
  #region token variables

  // Name of animation to be run in cutscene
  public string animationName;

  // Name of AnimationChildHandler in CutsceneElement
  public AnimationChildHandler handler;

  // Delay into start of this handler's animation
  public int delay;

  // Respective child to be animated, if available
  public string childName;

  #endregion

  #region token constructors

  // A constructor without childName
  public CutsceneToken(AnimationChildHandler root, string animName, int animDelay)
    : this()
  {
    this.handler = root;
    this.animationName = animName;
    this.delay = animDelay;
    this.childName = null;
  }

  // A constructor with childName
  public CutsceneToken(AnimationChildHandler root, string animName, int animDelay, string child)
    : this()
  {
    this.handler = root;
    this.animationName = animName;
    this.delay = animDelay;
    this.childName = child;

  }

  #endregion

  #region IComparable methods

  // Method for IComparable interface, allows tokens to be ordered according to crescent order of delat
  public int CompareTo(object item)
  {
    if(item.GetType() != this.GetType())
      throw new ArgumentException();

    CutsceneToken token = (CutsceneToken)item;

    return this.delay - token.delay;
  }
  #endregion

  #region Animation methods

  // Runs animation stored in token
  public void runAnimation()
  {
    if (childName == null || childName == "") // if doesnt have child
      handler.playAnimation(animationName); // play handler animation
    else // if has a child casts to root
      ((AnimationRootHandler)handler).playChildAnimation(childName, animationName); // play child animation
  }

  #endregion
}
#endregion

#region CutsceneScript

/// <summary>
/// CutsceneScript represents a script, storing information for animating a collection of CutsceneElements
/// All cutsceneScripts should be automatically joined by a CutsceneLocalHandler that calls its animation when needed
/// </summary>
public class CutsceneScript : MonoBehaviour
{

  #region private variables

  // Time since animation started
  private float elapsedAnimTime = 0;

  // Whether animation has started or not
  private bool inAnimation = false;

  // List containing CutsceneTokens, each allows for the animation of a single cutsceneElement
  List<CutsceneToken> tokens = new List<CutsceneToken>();

  #endregion

  #region methods for adding tokens

  // Creats and adds a new cutscene token without a childName
  public void addAnimation(AnimationChildHandler anim, string animation, int delay)
  {
    CutsceneToken token = new CutsceneToken(anim, animation, delay);
    token.childName = null;
    tokens.Add(token);
    tokens.Sort();
  }

  // Creats and adds a new cutscene token with a childName
  public void addAnimation(AnimationChildHandler anim, string animation, int delay, string childName)
  {
    CutsceneToken token = new CutsceneToken(anim, animation, delay,childName);
    tokens.Add(token);
    tokens.Sort();
  }

  // Adds an existing cutscene token
  public void addAnimation(CutsceneToken token)
  {
    tokens.Add(token);
    tokens.Sort();
  }

  #endregion 

  #region Animation methods


  // Starts the animation for this cutscene
  public void startAnimation()
  {
    inAnimation = true;

  }

  void Update()
  {
    if (inAnimation) // During animation
    {
      elapsedAnimTime += Time.deltaTime; // increase time 

      int toRemove = -1; // for ordered removal from token list

      // Run animations until delay of first token > elapsedAnimTime
      foreach (CutsceneToken token in tokens)
      {
        if (token.delay > elapsedAnimTime)
          break; // All other tokens have bigger delay
        else
        {
          toRemove++; // increase tokens to be removed
          token.runAnimation(); // run animation on this token
        }
      }

      // Remove already animated tokens
      if (toRemove > -1)
      {
          tokens.RemoveRange(0,toRemove+1);
      }

    }
  }

  #endregion

  #region debug methods

  // Debug method for debugging tokens
  void dumpTokens()
  {
    foreach (CutsceneToken token in tokens)
    {
      string str = "Handler " + token.handler.name + " registered for animation " + token.animationName + " after " + token.delay ;
      if(token.childName != null)
        str += " childName " + token.childName;
      Debug.Log( str);
    }
  }

  #endregion
}
#endregion