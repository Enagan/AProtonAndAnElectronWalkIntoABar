using UnityEngine;
using System.Collections;



public class InitialCutsceneRootCaller : MonoBehaviour {

  [SerializeField]
  private int _botCount;

  private AnimationRootHandler _roomHandler;
  private AnimationRootHandler _handler;
 
  #pragma warning disable 414
  private Animation _anim;
  
  void Start()
  {
    _handler = GetComponent<AnimationRootHandler>();
    _anim = _handler.getAnimation();
    _roomHandler = transform.parent.GetComponent<AnimationRootHandler>();
    
  }

  public void moveBots()
  {
    ServiceLocator.GetAudioSystem().PlayQuickSFX("rolling_meta_conveyor_belt", new Vector3(-29, 28, 17), 1f);
    
    for (int i = 0; i <= _botCount; i++)
    {
      string robot;
      if (i == 0)
        robot = "PlayerHolder";
      else
        robot = "Rob" + i;
        Debug.Log("Moving" + robot);
      if(i != 0)
        if(Random.Range(0,100)<50)
          _handler.playChildAnimation(robot, "InitialCutsceneNormalHolders");
        else
          _handler.playChildAnimation(robot, "InitialCutsceneNormalHolders2");
      else
        _handler.playChildAnimation(robot, "InitialCutscenePlayerHolder");

    }
    //_handler.playChildAnimation("PlayerHolder","InitialCutsceneNormalholders2");
  }
  public void acceptBot()
  {
    
    _roomHandler.playChildAnimation("Scanner", "InitialScanner");
    _roomHandler.playChildAnimation("Lights1", "InitialCutsceneAcceptBot");
    _roomHandler.playChildAnimation("Lights2", "InitialCutsceneAcceptBot");
  }

  public void acceptBotSound()
  {
    ServiceLocator.GetAudioSystem().PlayQuickSFX("htranchant", new Vector3(-29, 28, 17), 1f);
  }
  public void rejectBot()
  {
   
    _roomHandler.playChildAnimation("GarbageHatch", "GarbageHatch");
    _roomHandler.playChildAnimation("BrokenBot", "InitialCutsceneDropRobot");
    _roomHandler.playChildAnimation("PlayerHolder", "InitialCameraLookAtBotAnim");
    
  }

  public void rejectPlayer()
  {
    ServiceLocator.GetAudioSystem().PlayQuickSFX("military_alarm_noise", new Vector3(-29, 28, 17), 1f);
    _roomHandler.playChildAnimation("GarbageHatch", "GarbageHatch");
    _roomHandler.playChildAnimation("PlayerHolder", "InitialCutsceneDropCamera");
    _roomHandler.playChildAnimation("Lights1", "InitialCutsceneRejectBot");
    _roomHandler.playChildAnimation("Lights2", "InitialCutsceneRejectBot");
  }

}
