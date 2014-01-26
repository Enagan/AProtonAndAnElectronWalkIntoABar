using UnityEngine;
using System.Collections;

/// <summary>
///  This class is meant to be inherited by objects that when collided with the player (for now at least) 
///  Award him with an abillity.
///  Requires a Collision Volume with onTrigger enabled
/// </summary>
public abstract class AwardPlayerAbility : MonoBehaviour {

	// When collided with player adds him an ability using the awardAbility method
 void OnTriggerEnter(Collider other)
 {
    BipolarConsole.AllLog("Collided");
     if(other.tag == "Player")
     {
       BipolarConsole.AllLog("Collided with player");
       PlayerController player = other.GetComponent<PlayerController>();
       awardAbility(player);
       gameObject.SetActive(false);
     }
  }

  // meant to be implemented by children classes awards the player a specific abiluty
  public abstract void awardAbility(PlayerController player);
}
