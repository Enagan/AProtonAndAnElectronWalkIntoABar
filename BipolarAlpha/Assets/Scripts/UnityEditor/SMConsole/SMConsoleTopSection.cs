using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class SMConsoleTopSection : MonoBehaviour {


  SMConsoleData _data;

  public SMConsoleTopSection()
  {
    _data = SMConsoleData.Instance;
  }

  public void drawTopSection(float width)
  {
    // Upper section
    GUILayout.BeginHorizontal();

    // Log Scroll
    _logWindowScroll = GUILayout.BeginScrollView(_logWindowScroll, GUILayout.MaxHeight(_data.currentScrollViewHeight - 16), GUILayout.Width(width * 0.8f + 3));
    drawLogWindow();
    GUILayout.EndScrollView();

    // Tag Scroll
    _tagWindowScroll = GUILayout.BeginScrollView(_tagWindowScroll, GUILayout.MaxHeight(_data.currentScrollViewHeight - 16));
    drawTagWindow();
    GUILayout.EndScrollView();

    GUILayout.EndHorizontal();
  }

  // Tag Window

  Vector2 _tagWindowScroll;

  void drawTagWindow()
  {
    foreach (string tag in _data.tags)
    {

      if (tag == _data.selectedLogMessage.tag && !_data.canCollapse && !_data.selectedTags.Contains(tag))
      {
        _data.selectedLogMessage = new LogMessage();
      }
      else if (tag == _data.selectedCollapsedMessage.message.tag && _data.canCollapse && !_data.selectedTags.Contains(tag))
      {
        _data.selectedCollapsedMessage = new CollapsedMessage(new LogMessage());
      }

      bool isSelected = _data.selectedTags.Contains(tag);
      isSelected = GUILayout.Toggle(isSelected, tag, "ToolbarButton");

      if (!isSelected && _data.selectedTags.Contains(tag))
      {
        _data.selectedTags.Remove(tag);
      }
      else if (isSelected && !_data.selectedTags.Contains(tag))
      {
        _data.selectedTags.Add(tag);
      }
    }
  }

  // Log Window

  Vector2 _logWindowScroll;

  void drawLogWindow()
  {
    if (!_data.canCollapse)
    {
      foreach (LogMessage message in _data.showingLogs)
      {
        bool canDisplay = isMessageTypeAvailable(message.type) && isMessageTagAvailable(message.tag);

        if (canDisplay)
          displayMessage(message);
      }
    }
    else
    {
      foreach (KeyValuePair<string, CollapsedMessage> entry in _data.collapsedHash)
      {
        LogMessage message = entry.Value.message;
        bool canDisplay = isMessageTypeAvailable(message.type) && isMessageTagAvailable(message.tag);

        if (canDisplay)
          displayCollapsedMessage(entry.Value);
      }
    }

  }


    bool isMessageTagAvailable(string tag)
  {
    return _data.selectedTags.Contains(tag);
  }
  bool isMessageTypeAvailable(SMLogType type)
  {
    bool isAvailable = false;

    switch (type)
    {
      case SMLogType.NORMAL:
        isAvailable = _data.showLogs;
        break;
      case SMLogType.WARNING:
        isAvailable = _data.showWarnings;
        break;

      case SMLogType.ERROR:
        isAvailable = _data.showErrors;
        break;
    }
    return isAvailable;
  }

  void displayCollapsedMessage(CollapsedMessage cmessage)
  {
    LogMessage message = cmessage.message;
    if(GUILayout.Button(message.type + "   " + message.log + "  [" + message.tag + "] " + cmessage.counter,GUI.skin.label))
      _data.selectedCollapsedMessage = cmessage;
  }

  void displayMessage(LogMessage message)
  {
    Color prevColor = GUI.backgroundColor;
    Color messageColor  = Color.white;
    switch(message.type)
    {
      case SMLogType.WARNING:
         messageColor = Color.yellow;
        break;

      case SMLogType.ERROR:
        messageColor = Color.red;
        break;
    }
   
    string messageStr = message.type + "  " + SMConsoleData.getTimeStamp(message.stamp) + "   " + message.log + "  [" + message.tag + "] ";
    GUI.contentColor = messageColor;
    //EditorGUILayout.(message.type + "  " + SMConsoleData.getTimeStamp(message.stamp) + "   " + message.log + "  [" + message.tag + "] ");
    
    if (GUILayout.Button(messageStr))
    {
      if (_data.selectedLogMessage.hashKey().Equals(message.hashKey()))
        goToSelectedLog();
      else
        _data.selectedLogMessage = message;
    }

      GUI.contentColor = prevColor;
  }


  const string ASSET_START_TOKEN = "Asset";
  const string LINE_START_TOKEN = ":line ";
  void goToSelectedLog()
  {
    
    LogMessage selected;
    if(_data.canCollapse)
      selected = _data.selectedCollapsedMessage.message;
    else
      selected = _data.selectedLogMessage;

    string[] stackTraces = _data.selectedLogMessage.stackTrace.Split('\n');
    string lineEntry = stackTraces[stackTraces.Length - 1];

    int pathStart = lineEntry.IndexOf(ASSET_START_TOKEN);
    int pathEnd = lineEntry.IndexOf(LINE_START_TOKEN);
    int lineStart = lineEntry.IndexOf(LINE_START_TOKEN) + LINE_START_TOKEN.Length;
    int lineEnd = lineEntry.Length;
    if (pathStart < 0 || pathEnd < 0 || lineStart < 0 || lineEnd < 0)
    {
      SMConsole.Log("Incorrectly opening entry " + lineEntry, "SM Error", SMLogType.ERROR);
      return;
    }

    string path = lineEntry.Substring(pathStart, pathEnd - pathStart);
    int line = int.Parse(lineEntry.Substring(lineStart, lineEnd - lineStart));
    
    if(path != null && line > 0)
    {
     UnityEngine.Object script = Resources.LoadAssetAtPath(path, typeof(UnityEngine.Object));
     if (script != null)
     {
       AssetDatabase.OpenAsset(script.GetInstanceID(), line);
     }
    }
  }



}
