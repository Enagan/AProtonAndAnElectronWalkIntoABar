using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CutsceneManager {
  
  private Dictionary<string, CutsceneLocalHandler> registeredCutscenes = new Dictionary<string, CutsceneLocalHandler>();
  private Dictionary<string, ArrayList> unregisteredElements = new Dictionary<string, ArrayList>();

  public void registerCutsceneHandler(CutsceneLocalHandler handler)
  {

    string name = handler.cutsceneName;
    registeredCutscenes.Add(name, handler); // Add local cutscenehandler

    if (unregisteredElements.ContainsKey(name)) // if elements were added before localhandler had a chance to register
    {
      
      ArrayList arr = unregisteredElements[name]; // copy all elements into array
      foreach (CutsceneToken token in arr)
      {
        handler.addElement(token);
      }
      unregisteredElements.Remove(name); // removed unnecessary link
    }
  }

  public void registerElement(string cutsceneName, AnimationChildHandler element, string animationName, int delay, string optChild)
  {
    
    CutsceneToken token = new CutsceneToken(element, animationName, delay, optChild); // create Cutscene token

    if (registeredCutscenes.ContainsKey(cutsceneName)) // if localhandler is registered add token
    {
      registeredCutscenes[cutsceneName].addElement(token);
    }
    else // save token for when handler registers
    {
      ArrayList arr;
      if (unregisteredElements.ContainsKey(cutsceneName))
        arr = unregisteredElements[cutsceneName];
      else
      {
        arr = new ArrayList();
        unregisteredElements[cutsceneName] = arr;
      }
      arr.Add(token);
    }
    
  }
}
