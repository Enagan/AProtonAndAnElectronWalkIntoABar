//Made By: Ivo
/// <summary>
/// HUD main panel. Instantiated and called from HUDSystem, aggregates all HUDObjects and HUDPanels
/// </summary>
using UnityEngine;
using System.Collections;

public class HUDMainPanel : HUDPanel {

  public HUDMainPanel() : base(0) // Main Class has priority 0
  {
    addHUDObject(new HUDPlayerPanel(_priority+1));
  }
}
