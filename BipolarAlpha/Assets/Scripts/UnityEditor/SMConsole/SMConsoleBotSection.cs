#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
public class SMConsoleBotSection {


  SMConsoleData _data;
  
  // Scroll Vectors for ScrollViews
  Vector2 _botLeftWindowScroll;
  Vector2 _botMidWindowScroll;
  Vector2 _botRightWindowScroll;

  // Texture for stack trace jump button
  Texture2D _warpTex;


  public SMConsoleBotSection()
  {
    _data = SMConsoleData.Instance;
    _warpTex = (Texture2D)Resources.Load("GUI/Sprites/warp", typeof(Texture2D));
  }

  public void drawBotSection(float width, float height)
  {
    // Selected Message display
    _botLeftWindowScroll = GUILayout.BeginScrollView(_botLeftWindowScroll, GUILayout.MaxHeight(height - _data.currentScrollViewHeight), GUILayout.Width(width * 0.3f));
    displaySelectedMessage(height);
    GUILayout.EndScrollView();

    // Stack trace display
    _botMidWindowScroll = GUILayout.BeginScrollView(_botMidWindowScroll, GUILayout.MaxHeight(height - _data.currentScrollViewHeight), GUILayout.Width(width * 0.7f));
    displayStackTrace(height);
    GUILayout.EndScrollView();
  }

  // displays the selected message
  void displaySelectedMessage(float height)
  {
    string selectedMessage;
    if (!_data.canCollapse)
    {
      selectedMessage = _data.selectedLogMessage.log;
    }
    else
    {
      selectedMessage = _data.selectedCollapsedMessage.message.log;
    }
    EditorGUILayout.SelectableLabel(selectedMessage, GUI.skin.label, GUILayout.MaxHeight(height - _data.currentScrollViewHeight));
  }

  // displays the stack trace of selected message
  void displayStackTrace(float height)
  {
    string stackTrace;
    if (!_data.canCollapse)
    {
      stackTrace = _data.selectedLogMessage.stackTrace;
    }
    else
    {
      stackTrace = _data.selectedCollapsedMessage.message.stackTrace;
    }

    if (stackTrace != null)
    {
      int index = 0;
      string[] stackTraces = stackTrace.Split('\n');
      Array.Reverse(stackTraces);
      foreach (string trace in stackTraces)
      {

          string lineEntry = stackTraces[stackTraces.Length - 1];
          if (trace.Contains("SMConsole.Log") || trace.Contains("get_StackTrace()"))
              continue;
          index++;
        GUILayout.BeginHorizontal();

        if (_data.isEntryJumpable(trace))
        {
          if (GUILayout.Button(_warpTex, GUILayout.Width(32), GUILayout.Height(32))) 
            _data.jumpToSelectedScript(trace);
        }

        GUIStyle skin = GUI.skin.label;
        skin.wordWrap = true;
        string[] readableTraces = trace.Split(new string[] {"AProtonAndAnElectronWalkIntoABar"},System.StringSplitOptions.RemoveEmptyEntries);

        string toPrint = trace;
        if(readableTraces.Length == 2) // should be using regex
        {
          string suffix = readableTraces[1];
          readableTraces = readableTraces[0].Split(new string[] { "C:\\" }, System.StringSplitOptions.RemoveEmptyEntries);
          toPrint = readableTraces[0] + "\n" + suffix;
        }
        EditorGUILayout.SelectableLabel(" " +index + ". " + toPrint, skin, GUILayout.MaxHeight(height - _data.currentScrollViewHeight));
        GUILayout.EndHorizontal();
      }
    }
  }
    


}

#endif
