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

  // Console components
  SMConsoleHeaderBar _headerBar;
  SMConsoleTopSection _topSection;
  SMConsoleSplitWindow _splitWindow;
  SMConsoleBotSection _botSection;
  SMConsoleData _data;

  [MenuItem("Window/SM Console")]
  public static void ShowWindow()
  {
    
    //Show existing window instance. If one doesn't exist, make one.
    EditorWindow.GetWindow(typeof(SMConsole));
   

  }

  public void OnEnable()
  {
    // Create the singleton and intialize
    _data = SMConsoleData.Instance;
    _data.currentScrollViewHeight = this.position.height / 2;

     // Init components
    _headerBar = new SMConsoleHeaderBar();
    _topSection = new SMConsoleTopSection();
    _splitWindow = new SMConsoleSplitWindow();
    _botSection = new SMConsoleBotSection();
    SMConsole.Log("HAY");
    SMConsole.Log("HAY");
    SMConsole.Log("HAY","",SMLogType.ERROR);
    SMConsole.Log("HAY", "", SMLogType.WARNING);
  }

  void OnDisable()
  {
      Application.RegisterLogCallback(null);
      Application.RegisterLogCallbackThreaded(null);

  }

  // Draw Editor
  public void OnGUI()
  {
    Application.RegisterLogCallback(SMConsoleData.Instance.HandleLog);
    Application.RegisterLogCallbackThreaded(SMConsoleData.Instance.HandleLog);

    EditorGUILayout.BeginVertical();
    title = "SM Console";

    EditorGUILayout.EndVertical();
      
    _headerBar.drawHeaderBar();
    // HEADER

    // TOP
    GUILayout.BeginVertical();

    _topSection.drawTopSection(this.position.width, this.position.height);

      
      SMConsoleData data = SMConsoleData.Instance;
      if((!data.canCollapse && data.selectedLogMessage.hashKey() != new LogMessage().hashKey())
          || (data.canCollapse && data.selectedCollapsedMessage.message.hashKey() != new LogMessage().hashKey()))
      {
     
      
    // MID
    _splitWindow.drawWindow(this.position.width);
     
    // BOT
    GUILayout.BeginHorizontal();

    _botSection.drawBotSection(this.position.width, this.position.height);
          
             GUILayout.EndHorizontal();
        }
    GUILayout.EndVertical();

    if (_data.repaint)
        Repaint();
      
 
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
    Debug.Log(log);

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