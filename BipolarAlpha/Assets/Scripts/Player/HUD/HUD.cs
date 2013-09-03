using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour, IPlayerRoomChangeListner
{
  [SerializeField]
  private Texture2D _crosshairNormal;
  [SerializeField]
  private Texture2D _crosshairHighlight;
  private GameObject _camera;
  private Rect _crosshairPosition;
  private int _raycastMask = ~(1 << 8); //Ignore objects in layer 8 (Magnetic Force)
  private float _textureWidth;
  private float _textureHeight;



  public void ListenPlayerRoomChange(string newRoomName)
  {
    this.gameObject.guiText.text = newRoomName;
  }

  private bool fireRaycast()
  {
    RaycastHit hit;
    if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit, Mathf.Infinity, _raycastMask))// && hit.collider.CompareTag("Magnet"))
    {
      BipolarConsole.LousadaLog(hit.collider.gameObject.name);
      return true;
    }
    return false;
  }



  // Use this for initialization
  void Start()
  {
    _camera = GameObject.Find("Camera");
    _textureWidth = _crosshairNormal.width;
    _textureHeight = _crosshairNormal.height;
    _crosshairPosition = new Rect(Screen.width - _textureWidth,
                             Screen.height - _textureHeight,
                             _textureWidth,
                             _textureHeight);
    this.gameObject.guiText.pixelOffset = new Vector2(0, Screen.height / 2 - Screen.height / 10);
  }

  // Update is called once per frame
  void Update()
  {

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
