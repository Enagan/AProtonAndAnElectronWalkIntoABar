using UnityEngine;
using System.Collections;

/// <summary>
/// This class implements the animation and activator behavior for the Lift in elevatorDisposal room
/// </summary>
public class LiftActivator : MonoBehaviour, Activator
{

  #region ElevatorState enum

  // Possible Elevator states
  enum ElevatorState
  {
    Initial, // In its initial state elevator is stuck midway
    Up, // Elevator reached top floor
    Down, // Elevator reached bottom floor
    Descending, // Elevator is descending, can or not be still
    Ascending // Elevator is ascending, can or not be still
  };

  #endregion

  #region private variables

  // Variable saving elevator state
  private ElevatorState _state = ElevatorState.Initial;

  // AnimationRootHandler for quick accessing animations
  private AnimationRootHandler _animHandler;

  #endregion

  #region monobehavior methods

  void Start () {
    _animHandler = GetComponent<AnimationRootHandler>();
	}

  #endregion

  #region Activator methods

  public void Activate() // Activated by generator should go down if possible and resume if already descending
  {
    if (_animHandler == null)
      _animHandler = GetComponent<AnimationRootHandler>();

    if (_animHandler != null)
    {
      switch (_state) 
      {
       case ElevatorState.Initial: // In Initial state go down
          _animHandler.playAnimation("Lift down");
          _animHandler.playChildAnimation("LiftLights", "LiftLightsSwirl");
          _state = ElevatorState.Descending;
          break;
       case ElevatorState.Descending: // Was descending, resume descent
         Animation anim= _animHandler.getAnimation();
         anim.enabled = true;
         break;
        case ElevatorState.Down: // Do nothing
        default:
          break;
       // TODO other states
      }
    }
  }

  public void Deactivate() // Deactivated by generator should stop
  {
    if (_animHandler != null)
    {
      switch (_state)
      {
        case ElevatorState.Initial: 
        case ElevatorState.Down:
        default:
          break; // does nothing

        case ElevatorState.Descending: // Stop animation
          Animation anim = _animHandler.getAnimation(); 
          if (anim.IsPlaying("Lift down"))
          {
            anim.enabled = false;  //will stop playing but won't jump back to begnning
          }
          _animHandler.playChildAnimation("LiftLights", "LightLightsStop");
          break;

        // TODO other states
      }
    }
  }

  #endregion

  #region animation functions

  // Called when "Lift down" animation clip ends
  public void turnOffLights()
  {
    _animHandler.playChildAnimation("LiftLights", "LightLightsStop");
  }

  #endregion
}
