using UnityEngine;
using System.Collections;

public class Console : MonoBehaviour {

  public GameObject _realPlayer;
  public GameObject _jackedInSpawnPoint = null;
  private GameObject _jackedInPlayer = null;
  private float _playerMass;

  [SerializeField]
  private float waitingTime = 2.0f;

  /// <summary>
  /// Activates jacked in mode, the player is deactivated and a JackedInPlayer prefab is created
  /// </summary>
  public void ActivateJackedIn() {
    _realPlayer = GameObject.Find("Player");
    _realPlayer.rigidbody.velocity = Vector3.zero;
    PlayerActivation(false);
    PlayInAnimation();
    this.transform.Find("Boundary").gameObject.SetActive(true);
    _jackedInPlayer = ServiceLocator.GetResourceSystem().InstanceOf("Prefabs/JackedIn/JackedInPlayer", _jackedInSpawnPoint != null ? 
                                                                                                              _jackedInSpawnPoint.transform.position :
                                                                                                              this.transform.position);
    _jackedInPlayer.GetComponent<JackedInPlayer>().MotherConsole = this;
    _jackedInPlayer.transform.forward = -1.0f * _realPlayer.transform.forward;

    ServiceLocator.GetEventHandlerSystem().SendJackedInActivationEvent(_jackedInPlayer.GetComponentInChildren<Camera>());
  }


  /// <summary>
  /// Enables or Disables all the components in the player that can't co-exist with the JackedInPlayer
  /// </summary>
  /// <param name="state"></param>
  private void PlayerActivation(bool state)
  {
    _realPlayer.GetComponent<PlayerController>().enabled = state;

    foreach (Camera cam in _realPlayer.GetComponentsInChildren<Camera>())
    {
      cam.enabled = state;
    }

    foreach (AudioListener audio in _realPlayer.GetComponentsInChildren<AudioListener>())
    {
      audio.enabled = state;
    }


    /// this mass managemente is so the Player won't get pushed around by the JackedInPlayer
    if (!state)
    {
      _playerMass = _realPlayer.GetComponent<Rigidbody>().mass;
      _realPlayer.GetComponent<Rigidbody>().mass = 1000.0f;
    }
    else
    { 
      _realPlayer.GetComponent<Rigidbody>().mass = _playerMass;
  }
  }

  /// <summary>
  /// Deletes the instanced JackedInPlayer and restores the player to an active state
  /// </summary>
  public void DeleteSpawn()
  {
    PlayOutAnimation();
    PlayerActivation(true);
    GameObject.Destroy(_jackedInPlayer);
    _jackedInPlayer = null;
    this.transform.Find("Boundary").gameObject.SetActive(false);

    ServiceLocator.GetEventHandlerSystem().SendJackedInDeactivationEvent();
  }



  //Animations or effects of player switching go here

   private void PlayInAnimation(){}
   private void PlayOutAnimation(){}

}
