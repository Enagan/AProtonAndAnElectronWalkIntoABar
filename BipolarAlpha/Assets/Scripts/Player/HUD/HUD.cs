//Made by: Lousada

using UnityEngine;
using System.Collections;

/// <summary>
/// Class used to show the head-up display
/// </summary>
public class HUD : MonoBehaviour, IPlayerRoomChangeListner
{
  [SerializeField]
  private Texture2D _crosshairNormal;   //texture for the standard crosshair
  [SerializeField]
  private Texture2D _crosshairHighlight; // texture used when a player is pointing to a magnet
  private GameObject _camera;
  private Rect _crosshairPosition;
  private int _raycastMask = ~(1 << 8); //Ignore objects in layer 8 (Magnetic Force)
  private float _textureWidth;
  private float _textureHeight;



  public void ListenPlayerRoomChange(string newRoomName)
  {
    //Do Something with this
  }

  /// <summary>
  /// Fires a ray to determine if a player is pointing at a magnet or not
  /// </summary>
  private bool fireRaycast()
  {
    RaycastHit hit;
    if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit, Mathf.Infinity, _raycastMask) && hit.collider.CompareTag("Magnet"))
    {
      return hit.collider.gameObject.GetComponentInChildren<MagneticForce>().isActivated;
    }
    return false;
  }



  // Use this for initialization
  void Start()
  {
    _camera = GameObject.Find("Camera");
    _textureWidth = _crosshairNormal.width;
    _textureHeight = _crosshairNormal.height;
    //positions the crosshair texture on the center of the screen
    _crosshairPosition = new Rect(Screen.width - _textureWidth,  
                             Screen.height - _textureHeight,
                             _textureWidth,
                             _textureHeight);
  }

  void Update()
  {
    //positions the crosshair texture on the center of the screen
    _crosshairPosition.Set((Screen.width - _textureWidth / 4) / 2,       
                           (Screen.height - _textureHeight / 4) / 2,
                           _textureWidth / 4,
                           _textureHeight / 4);
  }

  void OnGUI()
  {
    if (fireRaycast())
    {
      GUI.DrawTexture(_crosshairPosition, _crosshairHighlight, ScaleMode.ScaleToFit);
    }
    else
    {
      GUI.DrawTexture(_crosshairPosition, _crosshairNormal, ScaleMode.ScaleToFit);
    }
  }
}
