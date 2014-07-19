//Made by Ivo.C
using UnityEngine;
using System.Collections;

public class CheckPointSystem : ICheckPointRegisterListener
{

  #region private variables
  
  // Current active checkpoint
  private CheckPoint _currentCheckpoint;

  #endregion
  
  #region Constructor and setters/getters

  // Sets the current checkpoint
  // Should be used to set checkpoint from save
  public void setCurrentCheckPoint(CheckPoint checkpoint)
  {
    _currentCheckpoint = checkpoint;
  }

  public CheckPointSystem()
  {
    //Register self on EventSystem
    ServiceLocator.GetEventHandlerSystem().RegisterCheckPointListener(this);
  }
  #endregion

  #region ICheckPointListener methods

  // Implementation of ICheckPointRegisterListener called when a checkpoint is interacted
  public void ListenCheckPointChange(CheckPoint checkpoint)  
  {
    //Disable old checkpoint
    if(_currentCheckpoint !=null)
      _currentCheckpoint.isActive = false;

    //Set new checkpoint
    _currentCheckpoint = checkpoint;
  }

 #endregion

 #region checkPoint system public methods

  //Respawns player at currently saved checkpoint
  public void respawnPlayer(PlayerController player)
  {
   
    player.transform.position = _currentCheckpoint.savedPlayerPosition;
    player.transform.eulerAngles = _currentCheckpoint.savedPlayerRotation;
    //Debug.Log("Respawning at "+ player.transform.position + " - with rot "+ player.transform.eulerAngles  );
  }

 #endregion
}
