#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System.IO;

/// <summary>
/// Post build script that should be run to ensure that all files are correctly placed
/// </summary>
public class ScriptBatch : MonoBehaviour
{
  [MenuItem("MyBuild/Bipolar Custom Build")]
  public static void CopyBuild()
  {
    
    // Asks user for the generated release pathname
    string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
    if (path.Equals(""))
    {
      return;
    }
    UnityEngine.Debug.Log(Application.dataPath);
    //Deletes any existing instances OffMeshLink the FileShare to Behaviour copied, for any fresh instalation
    FileUtil.DeleteFileOrDirectory(path + "/Managed/I18N.dll");
    FileUtil.DeleteFileOrDirectory(path + "/Managed/I18N.West.dll");
    FileUtil.DeleteFileOrDirectory(path + "/Levels");
    FileUtil.DeleteFileOrDirectory(path + "/Saves");
    FileUtil.DeleteFileOrDirectory(path + "/GUI/Text");

    //Creates directory for GUI stuff
    System.IO.Directory.CreateDirectory(path + "/GUI/Text");

    //Copies over DLL files and XMLs
    FileUtil.CopyFileOrDirectory("Assets/RequiredDLLs/I18N.dll", path + "/Managed/I18N.dll");
    FileUtil.CopyFileOrDirectory("Assets/RequiredDLLs/I18N.West.dll", path + "/Managed/I18N.West.dll");
    FileUtil.CopyFileOrDirectory("Assets/Resources/Levels", path + "/Levels");
    FileUtil.CopyFileOrDirectory("Assets/Resources/GUI/Text/strings.xml", path + "/GUI/Text/strings.xml");

    //Updates SaveState.lvl file, which contains path names for .lvl files
    string text = File.ReadAllText(path + "/Levels/SaveState.lvl");
    string replaced = text.Replace("Assets/Resources", path);
    File.WriteAllText(path + "/Levels/SaveState.lvl", replaced);

    UnityEngine.Debug.Log("Project Successfully Built!");

    /*
    string[] levels = {"Main.unity"};

    // Build player.
    BuildPipeline.BuildPlayer(levels, path + "/BipolarAlpha.exe", BuildTarget.StandaloneWindows, BuildOptions.None);

    // Copy a file from the project folder to the build folder, alongside the built game.
    FileUtil.CopyFileOrDirectory("Assets/Resources/Levels", path + "Levels");

    // Run the game (Process class from System.Diagnostics).
    Process proc = new Process();
    proc.StartInfo.FileName = path + "BuiltGame.exe";
    proc.Start();
     * */
  }
}
#endif