//#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System;

/*
 * SMConsole, a cutomized console with multiple functions
 * To Log call the several SMConsole.Log functions
 */
public class SMConsole : EditorWindow
{

  SMConsoleHeaderBar _headerBar;
  SMConsoleTopSection _topSection;
  SMConsoleSplitWindow _splitWindow;
  SMConsoleBotSection _botSection;

  [MenuItem("Window/SM Console")]
  public static void ShowWindow()
  {
    //Show existing window instance. If one doesn't exist, make one.
    EditorWindow.GetWindow(typeof(SMConsole));

    Application.RegisterLogCallback(SMConsoleData.Instance.HandleLog);
    Application.RegisterLogCallbackThreaded(SMConsoleData.Instance.HandleLog);
  }

  public void OnEnable()
  {
    // Create the singleton and intialize
    SMConsoleData.Instance.currentScrollViewHeight = this.position.height / 2;

    // Init components
    _headerBar = new SMConsoleHeaderBar();
    _topSection = new SMConsoleTopSection();
    _splitWindow = new SMConsoleSplitWindow();
    _botSection = new SMConsoleBotSection();

    SMConsole.Log("HelloWorld", "A");
    SMConsole.Log("HelloWorld", "A");
    SMConsole.Log("HelloWorld", "B");
    SMConsole.Log("HelloWorld", "C",SMLogType.WARNING);
    SMConsole.Log("HelloWorld", "C", SMLogType.NORMAL);
    SMConsole.Log("HelloWorld", "C", SMLogType.ERROR);
    SMConsole.Log("HelloWorldHelloWorldHelloWorldHelloWorldHelloWorldHelloWorldHelloWorldHelloWorld\nHelloWorldHelloWorldHelloWorldHelloWorld\n", "E", SMLogType.ERROR);

    //Application.RegisterLogCallback(SMConsoleData.Instance.HandleLog);
  

    Debug.Log("The system is working");
  }

  void OnDisable()
  {
    Application.RegisterLogCallback(null);
    Application.RegisterLogCallbackThreaded(null);
  }



  // RegisterLogCallback has a bug that only allows it to be registered from the second frame onwards
  private int frameCount = 0;
  private const int MIN_REGISTER_FRAME = 3;
  void registerDebugHandler()
  {
    if(frameCount > MIN_REGISTER_FRAME)
      return;
    else if (frameCount == MIN_REGISTER_FRAME)
      Application.RegisterLogCallback(SMConsoleData.Instance.HandleLog);
    else if (frameCount < MIN_REGISTER_FRAME)
      frameCount++;
  }

  // Draw Editor
  void OnGUI()
  {
    
    //registerDebugHandler();

    if (frameCount < MIN_REGISTER_FRAME)
      return;

    title = "SM Console";

    // HEADER
    _headerBar.drawHeaderBar();

    // TOP
    GUILayout.BeginVertical();

    _topSection.drawTopSection(this.position.width);

    // MID
    _splitWindow.drawWindow(this.position.width);

    // BOT
    GUILayout.BeginHorizontal();

    _botSection.drawBotSection(this.position.width, this.position.height);

    GUILayout.EndHorizontal();
    
    GUILayout.EndVertical();

   }

  #region static Log functions

  public static void Log(string log)
  {
    Log(log, SMConsoleData.EMPTY_TAG);
  }

  public static void Log(string log, string tag)
  {
    Log(log, tag, SMLogType.NORMAL);
  }

  public static void Log(string log, string tag, SMLogType type)
  {
    Log(log, tag, type, SMConsoleData.EMPTY_STACK_TRACE);
  }

  private static void Log(string log, string tag, SMLogType type, string stackTrace)
  {
    LogMessage message;
    SMConsoleData _data = SMConsoleData.Instance;

    if (stackTrace == SMConsoleData.EMPTY_STACK_TRACE)
      message = new LogMessage(log, tag, type,Environment.StackTrace);
    else
      message = new LogMessage(log, tag, type, stackTrace);

    _data.logs.Add(message);

    _data.logCounter[(int)type]++;

    string hashKey = message.hashKey();

    if (!_data.collapsedHash.ContainsKey(hashKey))
    {
      CollapsedMessage collapsed = new CollapsedMessage(message);
      _data.collapsedHash.Add(hashKey, collapsed);
    }
    else
    {
      CollapsedMessage collapsed = _data.collapsedHash[hashKey];
      collapsed.counter++;
      _data.collapsedHash[hashKey] = collapsed;
    }

    if (!_data.tags.Contains(message.tag))
    {
      _data.tags.Add(message.tag);
      _data.selectedTags.Add(message.tag);
    }

    // See if should be added to currently showing
     if (_data.searchFilter == SMConsoleData.DEFAULT_SEARCH_STR || _data.searchFilter == "")
    {
      _data.showingLogs.Add(message);
    }
    else
    {
       if(!_data.canCollapse)
       {
         if(message.log.IndexOf(_data.searchFilter, StringComparison.OrdinalIgnoreCase) >= 0)
            _data.showingLogs.Add(message);
       }
       else if(_data.collapsedHash[hashKey].counter == 1) // first instance
       {
         if (message.log.IndexOf(_data.searchFilter, StringComparison.OrdinalIgnoreCase) >= 0)
           _data.showingLogs.Add(_data.collapsedHash[hashKey]);
       }
    }
  }

  #endregion

}

//#endif