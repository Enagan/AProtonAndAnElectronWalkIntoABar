#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Diagnostics;

public class ScriptBatch : MonoBehaviour
{
  [MenuItem("MyBuild/Windows Build With Postprocess")]
  public static void BuildGame()
  {
    // Get filename.
    string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");

    string[] levels = {"Main.unity"};

    // Build player.
    BuildPipeline.BuildPlayer(levels, path + "/BipolarAlpha.exe", BuildTarget.StandaloneWindows, BuildOptions.None);

    // Copy a file from the project folder to the build folder, alongside the built game.
    FileUtil.CopyFileOrDirectory("Assets/Resources/Levels", path + "Levels");

    // Run the game (Process class from System.Diagnostics).
    Process proc = new Process();
    proc.StartInfo.FileName = path + "BuiltGame.exe";
    proc.Start();
  }
}
#endif