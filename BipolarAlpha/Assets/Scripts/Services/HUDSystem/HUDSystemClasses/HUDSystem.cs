using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// HUDSystem is the system tasked with rendering all GUI
/// </summary>
public class HUDSystem : MonoBehaviour, IPauseListener
{
	
  #region private variables
  // MainPanel that aggregates all other HUDPanels and HUDObjects
  private HUDMainPanel _HUDMain;
  
  // List with current paused monobehaviour objects if game is paused
  private List<MonoBehaviour> _pausedObjects = new List<MonoBehaviour>();

  //Bolean that indicates whether game is paused or not, any class dependent on pause should implement IPlayerAbilityPauseListener
  private bool _isGamePaused=false;
  #endregion
	
  #region MonoBehaviour methods
  void Start () 
  {
    // register self in ServiceLocator
    ServiceLocator.ProvideHUDSystem(this);
    
    // register self as pauseListener
    ServiceLocator.GetEventHandlerSystem().RegisterPauseListener(this);

    // Create Main HUD Objects
    _HUDMain = new HUDMainPanel();
  }
	//
  void OnGUI()
  {
    _HUDMain.DrawHUD(); // Calls HUDMainComposite DrawHUD, so it calls all HUDPanels and HUDObjects
  }
  #endregion

  #region Pause related methods

  // Called when an event to pause happens
  // Expected pause events include player pause ability
  public void pauseEvent()
  {
    //Send PauseEvent
    _isGamePaused = !_isGamePaused;
    ServiceLocator.GetEventHandlerSystem().SendPauseEvent(_isGamePaused);
    
    //Actual pause is done in ListenPlayerAbilityPauseGame 
  }

  // Ability needs to know if some other system paused/unpaused the game
  public void ListenPause(bool isPaused)
  {
    _isGamePaused = isPaused;
    if (_isGamePaused)
    {
      //Pausing everything code
      Time.timeScale = 0; //<- Engana hates this, here as a testament to "bad coding"

      // Get all Objects in scene
      object[] allObjects = Resources.FindObjectsOfTypeAll(typeof(MonoBehaviour));
      foreach (object thisObject in allObjects)
      {
        MonoBehaviour castObject = ((MonoBehaviour)thisObject);
        if (castObject != null && castObject!=this && castObject.transform.parent == null)
        {
          //Check if object is enabled only necessary on disabling
          if (castObject.enabled)
          {
            //Pause all Objects in scene
            castObject.enabled = false;
            _pausedObjects.Add(castObject);
          }
        }
      }
    }
    else
    {
      //Unpausing everything code
      Time.timeScale = 1; //<- Engana hates this aswell, its efficient though

      foreach (MonoBehaviour thisObject in _pausedObjects)
      {
        if(thisObject!=this)
          thisObject.enabled = true;
      }
      
      _pausedObjects.Clear();

    }
  }

  #endregion

}
