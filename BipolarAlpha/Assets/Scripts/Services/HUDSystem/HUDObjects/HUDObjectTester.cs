//Made By: Ivo
using UnityEngine;
using System.Collections;

/// <summary>
/// HUD object tester. A HUD test class to test all sorts of HUD tests needed in a HUD test class
/// </summary>
public class HUDObjectTester : HUDObject {
  
  [SerializeField]
  private Texture _testTexture = Resources.Load("Materials/ShaderTextures/NoiseTexture") as Texture;

  public HUDObjectTester() : base(0) { }
  public HUDObjectTester(int priority) : base(priority) { }

  public void setTex(Texture tex) { _testTexture = tex; }

  public override void DrawHUD()
  {
    if (_testTexture != null)
      GUI.DrawTexture(new Rect(25,25,100,100) ,_testTexture);
  }
}
