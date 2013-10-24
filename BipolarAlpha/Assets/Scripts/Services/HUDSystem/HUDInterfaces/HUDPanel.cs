using UnityEngine;
using System.Collections;

/// <summary>
/// HUDPanel is an interface HUDObject that aggregates other HUDObjects
/// Any aggregated HUDObject's DrawHUD call is called automatically
/// Notable HUDPanels should include menus, playerInterface, etc
/// </summary>
public abstract class HUDPanel : HUDObject {

  #region private variables
	
  //Aggregated HUDObjects array	
  private ArrayList _HUDObjects;
  #endregion
	
  #region overriden methods from HUDObject
  public override void DrawHUD()
  {
    if (_HUDObjects != null)
    {
      //TODO make foreach ordered by priority
      foreach (HUDObject obj in _HUDObjects)
      {
        if (obj != null)
          obj.DrawHUD();
      }
    }
  }
  #endregion
	
  #region HUDPanel constructor and aggretating methods
  protected HUDPanel(int priority) : base(priority) 
  {
    _HUDObjects = new ArrayList();
  }

  public void addHUDObject(HUDObject obj)
  {
    //TODO Make ordered array
    if (_HUDObjects != null && obj!=null)
    {
      _HUDObjects.Add(obj);
      _HUDObjects.Sort(new HUDObjectComparator());
    }
  }

  public void removeHUDObject(HUDObject obj)
  {
    if (_HUDObjects != null && _HUDObjects.Contains(obj))
    {
      _HUDObjects.Remove(obj);
      _HUDObjects.Sort(new HUDObjectComparator());
    }
  }

  public void dumpHUDObjects()
  {
    int i = 0;
    BipolarConsole.AllLog("Dumping HUDPanel");
    foreach (HUDObject obj in _HUDObjects)
    {
      if (obj != null)
      {
        BipolarConsole.AllLog(i+":"+obj);
      }
    }
  }
  #endregion
}
