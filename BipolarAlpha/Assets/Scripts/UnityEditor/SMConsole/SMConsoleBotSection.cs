using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class SMConsoleBotSection : MonoBehaviour {


  SMConsoleData _data;
  
  Vector2 _botLeftWindowScroll;
  Vector2 _botMidWindowScroll;
  Vector2 _botRightWindowScroll;


  public SMConsoleBotSection()
  {
    _data = SMConsoleData.Instance;
  }

  public void drawBotSection(float width, float height)
  {
    _botLeftWindowScroll = GUILayout.BeginScrollView(_botLeftWindowScroll, GUILayout.MaxHeight(height - _data.currentScrollViewHeight), GUILayout.Width(width * 0.3f));
    displaySelectedMessage(height);
    GUILayout.EndScrollView();

    _botMidWindowScroll = GUILayout.BeginScrollView(_botMidWindowScroll, GUILayout.MaxHeight(height - _data.currentScrollViewHeight), GUILayout.Width(width * 0.7f));
    displayStackTrace(height);
    GUILayout.EndScrollView();
  }

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

    GUIStyle skin = GUI.skin.label;
    skin.wordWrap = true;
    EditorGUILayout.SelectableLabel(stackTrace, skin, GUILayout.MaxHeight(height - _data.currentScrollViewHeight));
  }
    


}
