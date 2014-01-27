using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Cutscene Manager handles registering localCutscene handlers and cutscene elements
/// Necessary to ensure instancing different handlers and elements by complex states and then linking them
/// </summary>
public class CutsceneManager
{

  #region private variables

  // Dictionary with available cutscene handlers
  private Dictionary<string, CutsceneLocalHandler> registeredCutscenes = new Dictionary<string, CutsceneLocalHandler>();
  
  // Dictionary with cutsceneElements who asked to be registered before their cutscene handler was made available
  private Dictionary<string, ArrayList> unregisteredElements = new Dictionary<string, ArrayList>();

  #endregion

  #region Handler and Element registering methods

  // Registers a cutscene handler and registers on it cutscene elements if available
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

  // Links a cutsceneElement to its corresponding cutscenehandler if available
  // If handler isn't available adds it to an unregistered pool, linking once it becomes available
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
  #endregion
}
