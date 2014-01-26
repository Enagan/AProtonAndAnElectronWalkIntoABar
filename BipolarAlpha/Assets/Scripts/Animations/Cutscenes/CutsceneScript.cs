using UnityEngine;
using System;
using System.Collections.Generic;



public struct CutsceneToken : IComparable

{

  public string animationName;

  public AnimationChildHandler handler;

  public int delay;

  public string childName;

  public CutsceneToken(AnimationChildHandler root, string animName, int animDelay)
    : this()
  {
    this.handler = root;
    this.animationName = animName;
    this.delay = animDelay;
    this.childName = null;
  }

  public CutsceneToken(AnimationChildHandler root, string animName, int animDelay, string child)
    : this()
  {
    this.handler = root;
    this.animationName = animName;
    this.delay = animDelay;
    this.childName = child;

  }

  public int CompareTo(object item)
  {
    if(item.GetType() != this.GetType())
      throw new ArgumentException();

    CutsceneToken token = (CutsceneToken)item;

    return this.delay - token.delay;
  }
}

public class CutsceneScript : MonoBehaviour {


  private float elapsedAnimTime = 0;

  private bool inAnimation = false;

  List<CutsceneToken> tokens = new List<CutsceneToken>();

  public void startAnimation()
  {
    inAnimation = true;

  }

  public void addAnimation(AnimationChildHandler anim, string animation, int delay)
  {
    CutsceneToken token = new CutsceneToken(anim, animation, delay);
    token.childName = null;
    tokens.Add(token);
    tokens.Sort();
  }

  public void addAnimation(AnimationChildHandler anim, string animation, int delay, string childName)
  {
    CutsceneToken token = new CutsceneToken(anim, animation, delay,childName);
    tokens.Add(token);
    tokens.Sort();
  }

  public void addAnimation(CutsceneToken token)
  {
    tokens.Add(token);
    tokens.Sort();
  }


  // Update is called once per frame
  void Update()
  {
    if (inAnimation)
    {
      elapsedAnimTime += Time.deltaTime;

      int toRemove = -1;

      // Run animations until delay of first token > elapsedAnimTime
      foreach (CutsceneToken token in tokens)
      {
        if (token.delay > elapsedAnimTime)
          break; // All other tokens have bigger delay
        else
        {
          toRemove++;
          if(token.childName!=null)
            BipolarConsole.AllLog(token.handler.name + " - "+token.childName);
          if (token.childName == null ||  token.childName == "")
            token.handler.playAnimation(token.animationName); // play handler animation
          else
            ((AnimationRootHandler)token.handler).playChildAnimation(token.childName, token.animationName); // play child animation
        }
      }

      // Remove already animated tokens
      if (toRemove > -1)
      {
          tokens.RemoveRange(0,toRemove+1);
      }

    }
  }



  void dumpTokens()
  {
    foreach (CutsceneToken token in tokens)
    {
      BipolarConsole.AllLog("Handler " + token.handler.name + " registered for animation " + token.animationName + " after " + token.delay +"s"  );
    }
  }
}
