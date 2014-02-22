//Made By: Ivo
using UnityEngine;
using System.Collections;

/// <summary>
/// HUDPlayerPanel is a HUDPanel to aggregate the player interface HUDObjects
/// </summary>
public class HUDPlayerPanel : HUDPanel {

  #region HUD methods
  
  // Instantiates the player interface in the constructor
  public HUDPlayerPanel(int priority) : base(priority)
  {
    HUDCrosshair cross = new HUDCrosshair(3);
    addHUDObject(cross);
		
    HUDAdvisorRoutine advisor = new HUDAdvisorRoutine(2);
    addHUDObject(advisor);

    HUDBootSeq bootUpSeq = new HUDBootSeq(2);
    addHUDObject(bootUpSeq);
    

  }
  #endregion
	
}
