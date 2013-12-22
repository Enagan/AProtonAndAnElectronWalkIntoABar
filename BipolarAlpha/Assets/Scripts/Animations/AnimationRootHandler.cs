//Made by. Ivo Capelo
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// #1. Quick and dirty AnimationGuide
//1. Add AnimationRootHandler to nodes where scripts are meant to call animation
//2. Add AnimationChildHandler to nodes that should be animated but don't require scripts (ensure there is a RootHandler in a parent to call the child's animation)
//3. Add Animation Component to all RootHandlesr and ChildHandlers
//4. Add clips to the Animations array in Animation
//5. Call animations using methods in #2

//#2 Methods for acessing Animations
// Calling animation on this Object
//playAnimation(string clipName);

// Calling animation on Child Object
//playChildAnimation(string childName, string clipName);

// Getting Animation Components
// Animation Components can be altered and have additional Play calls such as CrossFade or Blend
//getAnimation() - For this object
//getChildAnimation(string childName) - For child

/// <summary>
/// AnimationRootHandler is an AnimationChildHandler tasked with aggregating animations in object hiearchy to facilitate calling animations
/// The class is meant to be a parent node and should be called from scripts for object animation
/// Requires the Animation component in the object
/// </summary>
[RequireComponent(typeof(Animation))]
public class AnimationRootHandler : AnimationChildHandler
{

  #region private variables

  //Symbol used to split keys between child name and clip name
  private static char _splitSymbol = ':';

  //Dictionary for quick acess to children with certain animations
  //Keys can be obtained with getAnimChildrenKey(string clipName, string childName) or getAnimChildrenKey(string clipName, AnimationChildHandler child)
  private Dictionary<string, AnimationChildHandler> _animChildren = new Dictionary<string,AnimationChildHandler>();

  #endregion

  #region Monobehavior methods

  // Use this for initialization
	void Awake () {
    //Initialize _animChildren with all animations in hiearchy below
    initializeAnimations();
	}

  #endregion

  #region Startup register AnimationChildHandler methods

  private void initializeAnimations()
  {
    //Get self, all children, and children of children, with animation handlers
    AnimationChildHandler[] array = this.GetComponentsInChildren<AnimationChildHandler>();
    foreach(AnimationChildHandler child in array)
    {
     // Register Children and Self
     registerAnimationsInChildHandler(child);
    }
  }

  // Registers all animations from an AnimationChildHAndler in _animChildren
  private void registerAnimationsInChildHandler(AnimationChildHandler handler)
  {
    if(handler.animationClips!=null && handler.animationClips.Count> 0)
      foreach (string animName in handler.animationClips.Keys)
      {
        //Register pairing of animation and handler for easy acquisition and call of animation
        string pairingID = getAnimChildrenKey(animName,handler);
        _animChildren.Add(pairingID,handler);
      }
  }

  #endregion

  #region Key Construction static methods

  // Returns key for acessing _animChildren
  public static string getAnimChildrenKey(string anim, AnimationChildHandler child)
  {
    // Key is formed from "ChildName _splitSymbol anim"
    // Child "LeftMagnet"'s animation "Turn" will have the key "LeftMagnet:Turn", assuming ':' as splitSymbol
    return child.childName+_splitSymbol+anim;
  }

  // Returns key for acessing _animChildren
  public static string getAnimChildrenKey(string anim, string childName)
  {
    // Key is formed from "ChildName _splitSymbol anim"
    // Child "LeftMagnet"'s animation "Turn" will have the key "LeftMagnet:Turn", assuming ':' as splitSymbol
    return childName+_splitSymbol+anim;
  }

  // Returns the clipName from a key
  public static string getAnimNameFromAnimChildrenKey(string key)
  {
    string[] split = key.Split(new char[] {_splitSymbol});
    if(split.Length !=2)
    {
      throw new BipolarExceptionInvalidRegisteredChild("Invalid data for key: " + key);
    }
    else
    {
      //returns animation name
      return split[1];
    }
  }

  // Returns the childName from a key
  public static string getChildNameFromAnimChildrenKey(string key)
  {
    string[] split = key.Split(new char[] {_splitSymbol});
    if (split.Length != 2)
    {
      throw new BipolarExceptionInvalidRegisteredChild("Invalid data for key: " + key);
    }
    else
    {
      //returns animation name
      return split[0];
    }
  }

  #endregion

  #region Child animation accessing methods

  // Plays clip in child
  public void playChildAnimation(string childName,string clipName)
  {
    string key = getAnimChildrenKey(clipName, childName);
    AnimationChildHandler child = _animChildren[key];
    child.playAnimation(clipName);
  }

  // Returns Animation component in child
  public Animation getChildAnimation(string childName)
  {

    foreach (string key in _animChildren.Keys)
    {
      if(getChildNameFromAnimChildrenKey(key).CompareTo(childName) == 0)
      {
        return _animChildren[key].getAnimation();
      }
    }

    throw new BipolarExceptionInvalidRegisteredChild("Not able to Find AnimationChildHandler with name:"+childName);
  }

  public AnimationChildHandler getChildHandler(string childName)
  {
      
    foreach (string key in _animChildren.Keys)
    {
      
      if (getChildNameFromAnimChildrenKey(key).CompareTo(childName) == 0)
      {
        
        return _animChildren[key];
      }
    }

    throw new BipolarExceptionInvalidRegisteredChild("Not able to Find AnimationChildHandler with name:" + childName);
  }
  #endregion

  #region Debug methods

  //Dumps into console all animations in RootHandler and respective children
  private void dumpAnimations()
  {
    BipolarConsole.AllLog("<Dumping Animations in ROOT:" + childName);
    foreach (string key in _animChildren.Keys)
    {
      BipolarConsole.AllLog(key);
    }
    BipolarConsole.AllLog("Dump end>");
  }

  #endregion
}
