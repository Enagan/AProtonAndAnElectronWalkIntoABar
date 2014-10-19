using UnityEngine;
using System.Collections;

/// <summary>
///  This class is meant to be inherited by objects that when collided with the player (for now at least) 
///  Award him with an abillity.
///  Requires a Collision Volume with onTrigger enabled
/// </summary>
public abstract class AwardPlayerAbility : MonoBehaviour {

  // If should deactivate self on pickup
  protected bool _destroyOnPickup = true;

	// When collided with player adds him an ability using the awardAbility method
 void OnTriggerEnter(Collider other)
 {
     if(other.tag == "Player")
     {
         SMConsole.Log("[AWARD PLAYER ABILITY] Collided with player", "Ability", SMLogType.NORMAL);

       PlayerController player = other.GetComponent<PlayerController>();
       awardAbility(player);
       if(_destroyOnPickup)
        gameObject.SetActive(false);
     }
  }

  // meant to be implemented by children classes awards the player a specific abiluty
  public abstract void awardAbility(PlayerController player);
}
