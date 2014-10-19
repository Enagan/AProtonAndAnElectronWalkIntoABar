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
  private ArrayList _disposableHUDObjects;
  #endregion
	
  #region overriden methods from HUDObject
  public override void DrawHUD()
  {
    if (_HUDObjects != null)
    {
      //Draws HUDObjects according to sorted ArrayList
      foreach (HUDObject obj in _HUDObjects)
      {
        if (obj != null)
          obj.DrawHUD();
      }
    }

    // Check if needs disposal
    if (_disposableHUDObjects!=null)
    {
      bool disposed = false;
      //if contains none doesn't do disposal
      foreach(HUDObject obj in _disposableHUDObjects)
      {
        disposed = true;
        if (_HUDObjects != null && _HUDObjects.Contains(obj))
        {
          _HUDObjects.Remove(obj);
          _HUDObjects.Sort(new HUDObjectComparator());
        }
      }
      if(disposed)
        _disposableHUDObjects.Clear();
    }
  }
  #endregion
	
  #region HUDPanel constructor and aggretating methods
  protected HUDPanel(int priority) : base(priority) 
  {
    _HUDObjects = new ArrayList();
    _disposableHUDObjects = new ArrayList();
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
      // Add for later disposal
      _disposableHUDObjects.Add(obj);
    }
  }

  public void dumpHUDObjects()
  {
    int i = 0;
    SMConsole.Log("Dumping HUDPanel", "HUD", SMLogType.NORMAL);
  
    foreach (HUDObject obj in _HUDObjects)
    {
      if (obj != null)
      {
          SMConsole.Log(i + ":" + obj, "HUD", SMLogType.NORMAL);
  
      }
    }
  }
  #endregion
}
