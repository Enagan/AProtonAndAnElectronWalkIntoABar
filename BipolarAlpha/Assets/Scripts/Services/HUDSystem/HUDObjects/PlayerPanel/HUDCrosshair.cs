//Made By: Ivo, Lousada
using UnityEngine;
using System.Collections;
/// <summary>
/// HUDCrosshair handles rendering the player crosshair in the middle of the screen 
/// </summary>
public class HUDCrosshair : HUDObject {
  
  #region private variables
	
  // Crosshair textures, loaded on creation
  private Texture2D _crosshairNormal = Resources.Load("GUI/Crosshairs/Deactivated") as Texture2D;   //texture for the standard crosshair
  private Texture2D _crosshairHighlight = Resources.Load("GUI/Crosshairs/ActivatedBoth") as Texture2D; // texture used when a player is pointing to a magnet
	
  //Texture dimensions
  private float _textureWidth;
  private float _textureHeight;

  //Raycast mask to gnore objects in layer 8 (Magnetic Force) and layer 15 (Magnetic Blockers)
  private int _raycastMask = ~((1 << 8) | ( 1 << 15) | (1 << 17)); 
	
  //Camera attached to the player	
  private GameObject _camera;
  
  //Cursor position for screen resizing
  private Rect _crosshairPosition;

  #endregion
	
  #region Crosshair public methods
  
  // Constructor	
  public HUDCrosshair(int priority) : base(priority)  {
    
    _camera = GameObject.Find("Camera"); // Warning, main camera needs this name or create a service to localize cameras
    
    //positions the crosshair texture on the center of the screen		
	_textureWidth = _crosshairNormal.width;
    _textureHeight = _crosshairNormal.height;
    _crosshairPosition = new Rect(Screen.width - _textureWidth,
                             Screen.height - _textureHeight,
                             _textureWidth,
                             _textureHeight);
  }
  /// <summary>
  /// Overriden DrawHUD method, chooses correct crosshair texture and places it in the center of the screen
  /// </summary>
  public override void DrawHUD()
  {
    _crosshairPosition.Set((Screen.width - _textureWidth / 4) / 2,
                       (Screen.height - _textureHeight / 4) / 2,
                       _textureWidth / 4,
                       _textureHeight / 4);

   // Selects crosshair depending on whether looking or not at a magnet, alternatively use the Event system to determine
   if (fireRaycast())
    {
      GUI.DrawTexture(_crosshairPosition, _crosshairHighlight, ScaleMode.ScaleToFit);
    }
    else
    {
      GUI.DrawTexture(_crosshairPosition, _crosshairNormal, ScaleMode.ScaleToFit);
    }
  }
	
  #endregion
  
  #region Auxiliary private methods
  /// <summary>
  /// Fires a ray to determine if a player is pointing at a magnet or not
  /// </summary>
  private bool fireRaycast()
  {
    RaycastHit hit;
    if (_camera!=null && _camera.gameObject.active)
    {
      if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit, Mathf.Infinity, _raycastMask) && hit.collider.CompareTag("Magnet"))
      {
        return hit.collider.gameObject.GetComponentInChildren<MagneticForce>().isActivated;
      }
    }
    return false;

  }
  #endregion
}
