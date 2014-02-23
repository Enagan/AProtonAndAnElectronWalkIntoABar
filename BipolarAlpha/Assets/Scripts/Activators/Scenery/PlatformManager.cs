using UnityEngine;
using System.Collections;

public class PlatformManager : MonoBehaviour {

  public void activatePlatforms()
  {
    AnimationRootHandler anim = GetComponent<AnimationRootHandler>();
    if (anim)
    {
      anim.playChildAnimation("Platform2", "WeightPlatformStart");
      anim.playChildAnimation("Platform3", "WeightPlatformStart");
      anim.playChildAnimation("Platform4", "WeightPlatformStart");
    }
  }
}
