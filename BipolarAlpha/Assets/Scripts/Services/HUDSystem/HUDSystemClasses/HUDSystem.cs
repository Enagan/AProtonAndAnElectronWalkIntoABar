using UnityEngine;
using System.Collections;

/// <summary>
/// HUDSystem is the system tasked with rendering all GUI
/// </summary>
public class HUDSystem : MonoBehaviour {
	
  #region private variables
  // MainPanel that aggregates all other HUDPanels and HUDObjects
  private HUDMainPanel _HUDMain;
  #endregion
	
  #region MonoBehaviour methods
  void Start () 
  {
    // register self in ServiceLocator
    ServiceLocator.ProvideHUDSystem(this);

    _HUDMain = new HUDMainPanel();
  }
	//
  void OnGUI()
  {
    _HUDMain.DrawHUD(); // Calls HUDMainComposite DrawHUD, so it calls all HUDPanels and HUDObjects
  }
  #endregion
}
