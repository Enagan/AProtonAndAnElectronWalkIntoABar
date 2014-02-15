using UnityEngine;
using System.Collections;



public class InitialCutsceneRootCaller : MonoBehaviour {

  [SerializeField]
  private int _botCount;

  private AnimationRootHandler _roomHandler;
  private AnimationRootHandler _handler;
  private Animation _anim;
  
  void Start()
  {
    _handler = GetComponent<AnimationRootHandler>();
    _anim = _handler.getAnimation();
    _roomHandler = transform.parent.GetComponent<AnimationRootHandler>();
    
  }

  public void moveBots()
  {
    
    
    for (int i = 0; i <= _botCount; i++)
    {
      string robot;
      if (i == 0)
        robot = "PlayerHolder";
      else
        robot = "Rob" + i;
        BipolarConsole.AllLog("Moving" + robot);
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

  public void rejectBot()
  {

    _roomHandler.playChildAnimation("GarbageHatch", "GarbageHatch");
    _roomHandler.playChildAnimation("BrokenBot", "InitialCutsceneDropRobot");
    _roomHandler.playChildAnimation("PlayerHolder", "InitialCameraLookAtBotAnim");
    
  }

  public void rejectPlayer()
  {
    _roomHandler.playChildAnimation("GarbageHatch", "GarbageHatch");
    _roomHandler.playChildAnimation("PlayerHolder", "InitialCutsceneDropCamera");
    _roomHandler.playChildAnimation("Lights1", "InitialCutsceneRejectBot");
    _roomHandler.playChildAnimation("Lights2", "InitialCutsceneRejectBot");
  }

}
