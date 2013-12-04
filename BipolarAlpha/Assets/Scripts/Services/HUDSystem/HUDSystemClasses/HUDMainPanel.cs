//Made By: Ivo
/// <summary>
/// HUD main panel. Instantiated and called from HUDSystem, aggregates all HUDObjects and HUDPanels
/// </summary>
using UnityEngine;
using System.Collections;

public class HUDMainPanel : HUDPanel, IPauseListener {

  private HUDPlayerPanel _playerPanel= new HUDPlayerPanel(1);
  private HUDPausePanel _pausePanel= new HUDPausePanel(2);


  public HUDMainPanel() : base(0) // Main Class has priority 0
  {
    ServiceLocator.GetEventHandlerSystem().RegisterPauseListener(this);
    addHUDObject(_playerPanel);
  }


  public void ListenPause(bool isPaused)
  {
    if (isPaused)
    {
      addHUDObject(_pausePanel);
    }
    else
    {
      removeHUDObject(_pausePanel);
    }
  }
}
