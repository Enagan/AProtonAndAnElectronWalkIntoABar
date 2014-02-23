using UnityEngine;
using System.Collections;

public class HexaDoorActivator : MonoBehaviour, Activator {

  AnimationChildHandler _handler;
  bool isOpen = false;
  // Use this for initialization
  void Start() {
    _handler = GetComponent<AnimationChildHandler>();
	}

  public void Activate()
  {
    if (_handler)
    {
      Animation anim = _handler.getAnimation();
      anim["HexDoor2Open"].speed = 1;
      if (!anim.IsPlaying("HexDoor2Open"))
      {
        anim["HexDoor2Open"].time = 0;
        _handler.playAnimation("HexDoor2Open");
      }
      isOpen = true;
    }
  }

  public void Deactivate()
  {

    if (_handler && isOpen)
    {
      Animation anim = _handler.getAnimation();
      anim["HexDoor2Open"].speed = -1;
      if (!anim.IsPlaying("HexDoor2Open"))
      {
        anim["HexDoor2Open"].time = anim["HexDoor2Open"].length;
        anim.Play("HexDoor2Open");
      }

      
      
      isOpen = false;
    }
    
  }
}
