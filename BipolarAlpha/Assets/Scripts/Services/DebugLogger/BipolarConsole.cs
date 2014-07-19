using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices; 

public enum Devs { ENGANA, LOUSADA, HUGO, IVO, RUI }

// Bipolar Console
/// <summary>
/// Bipolar Console is an utility class which provides a way to use the unity console in a more encapsulated manner.
/// These static functions allow for developers to write their own debug messages without them being visible to other
/// developers. It also avoids the printing of messages if the game is being run in a release version, thus allowing
/// Debug message calls to stay in the codebase without resulting in performance losses.
/// Bipolar Console depends upon the BipolarDevID static constants to determine which dev is running the code
/// and showing the appropriate messages
/// </summary>
public class BipolarConsole : MonoBehaviour
{

  public string output = "";
  public string stack = "";
  private void Start()
  {
    Application.RegisterLogCallbackThreaded(HandleLog);
    /*Debug.Log("Handle Log registered");*/
  }
  void OnDisable()
  {
    Application.RegisterLogCallback(null);
  }
  void HandleLog(string logString, string stackTrace, LogType type)
  {
    output = logString;
    stack = stackTrace;
    logString = "Stuff" + logString;
  }
  #region Core Functions

  /// Log <summary>
  /// logs a message from a specific developer only if the
  ///      BipolarDevId is the same developer as "from" or if it has
  ///      enabled the "see all logs" option. Only visible in editor mode.
  /// </summary>
  private static void Log(Devs from, object message, Object objectSource = null) 
  {
#if UNITY_EDITOR
    if (BipolarDevID.iAm == from || BipolarDevID.seeAll)
    {
      Debug.Log("[" + from + "] " + message, objectSource);
    }
#endif
	}

  /// ExceptionLog <summary>
  /// Logs an exception from Unity, usefull for debugging exceptions.Only visible in editor mode.
  /// </summary>
  public static void ExceptionLog(UnityException exception, Object objectSource = null)
  {
#if UNITY_EDITOR
//    Debug.LogException(exception, objectSource);
#endif
  }

  #endregion

  #region Developer Specific Logs
  /// <summary>
  /// Write a message to the console as developer Pedro Engana
  /// </summary>
  public static void EnganaLog(object message, Object objectSource = null)
  {
    Log(Devs.ENGANA, message, objectSource);
  }

  /// <summary>
  /// Write a message to the console as developer Hugo Goncalves
  /// </summary>
  public static void HugoLog(object message, Object objectSource = null)
  {
    Log(Devs.HUGO, message, objectSource);
  }

  /// <summary>
  /// Write a message to the console as developer Ivo Capelo
  /// </summary>
  public static void IvoLog(object message, Object objectSource = null)
  {
    Log(Devs.IVO, message, objectSource);
  }

  /// <summary>
  /// Write a message to the console as developer Pedro Lousada
  /// </summary>
  public static void LousadaLog(object message, Object objectSource = null)
  {
    Log(Devs.LOUSADA, message, objectSource);
  }

  /// <summary>
  /// Write a message to the console as developer Rui Dias
  /// </summary>
  public static void RuiLog(object message, Object objectSource = null)
  {
    Log(Devs.RUI, message, objectSource);
  }

  /// <summary>
  /// Write a message to the console which will be visible to all developers
  /// </summary>
  public static void AllLog(object message)
  {
#if UNITY_EDITOR
    Debug.Log("[ALL] " + message);
#endif
  }

  #endregion
}
