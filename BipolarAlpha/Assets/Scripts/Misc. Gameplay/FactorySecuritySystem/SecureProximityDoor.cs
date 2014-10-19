using UnityEngine;
using System.Collections;

public class SecureProximityDoor : MonoBehaviour {

  public GameObject door = null;
  public SecurityLevels _minimumLevelNeeded = SecurityLevels.Level1;
  public GameObject _lightParent = null;

  DummyDoor dummyDoor = null;

  bool _doorUnlocked = false;

  public void Start()
  {
    dummyDoor = door.GetComponent<DummyDoor>();
    refreshLights();
  }

	public void OnTriggerEnter(Collider other)
  {
    if (other.tag == "Player" && _doorUnlocked)
      dummyDoor.Activate();
  }

  public void OnTriggerExit(Collider other)
  {
    if (other.tag == "Player")
      dummyDoor.Deactivate();
  }

  public void unlockDoor(SecurityLevels unlockerWithPermissions)
  {
    if (unlockerWithPermissions >= _minimumLevelNeeded)
    {
      _doorUnlocked = true;
      SMConsole.Log("[SECURE DOOR] Door Unlocked!", "Debug", SMLogType.NORMAL);
    }
    else
    {
      _doorUnlocked = false;
      SMConsole.Log("[SECURE DOOR] Door Unlocked!", "Debug", SMLogType.NORMAL);
    }
    refreshLights();
  }

  public void lockDoor(SecurityLevels unlockerWithPermissions)
  {
    if (unlockerWithPermissions >= _minimumLevelNeeded)
    { 
      _doorUnlocked = false;
      SMConsole.Log("[SECURE DOOR] System Key invalid", "Debug", SMLogType.NORMAL);
    }
    refreshLights();
  }

  public void refreshLights()
  {
    float g = 52f / 255f;
   // float b = 174f / 255f;
    if (_lightParent == null)
    {
      Transform parentTransformLights = this.transform.parent.FindChild("MagnetLights");
      if (parentTransformLights != null)
      {
        _lightParent = parentTransformLights.gameObject;
      }
    }
    if (_lightParent != null)
    {
      foreach (Light light in _lightParent.GetComponentsInChildren<Light>())
      {
        if (_doorUnlocked)
        {
          light.color = new Color(0, g, 0);
        }
        else
        {
          light.color = Color.red;
        }
      }
    }
  }
}
