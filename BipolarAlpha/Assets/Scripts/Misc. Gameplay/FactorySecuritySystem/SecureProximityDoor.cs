using UnityEngine;
using System.Collections;

public class SecureProximityDoor : MonoBehaviour {

  public GameObject door = null;
  public SecurityLevels _minimumLevelNeeded = SecurityLevels.Level1;

  DummyDoor dummyDoor = null;

  bool _doorUnlocked = false;

  public void Start()
  {
    dummyDoor = door.GetComponent<DummyDoor>();
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
      Debug.Log("[SECURE DOOR] Door Unlocked!");
    }
    else
    {
      _doorUnlocked = false;
      Debug.Log("[SECURE DOOR] System Key invalid");
    }
  }

  public void lockDoor(SecurityLevels unlockerWithPermissions)
  {
    if (unlockerWithPermissions >= _minimumLevelNeeded)
    { 
      _doorUnlocked = false;
      Debug.Log("[SECURE DOOR] Door Locked!");
    }
  }
}
