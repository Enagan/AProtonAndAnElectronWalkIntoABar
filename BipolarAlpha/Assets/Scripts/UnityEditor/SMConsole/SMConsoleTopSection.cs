using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class SMConsoleTopSection : MonoBehaviour {


  SMConsoleData _data;
  GUISkin _buttonSkin;
  GUISkin _buttonSkin2;
  GUISkin _selectedButtonSkin;
  Texture2D _logTex;
  Texture _warningTex;
  Texture _errorTex;
  float _labelWidth;
  

  public SMConsoleTopSection()
  {


    
    _data = SMConsoleData.Instance;
    _buttonSkin = Resources.Load("GUI/Skins/SMConsoleSkin") as GUISkin;
    _buttonSkin2 = Resources.Load("GUI/Skins/SMConsoleSkin2") as GUISkin;
    _selectedButtonSkin = Resources.Load("GUI/Skins/SMConsoleSkin3") as GUISkin;

    _logTex = (Texture2D)Resources.Load("GUI/Sprites/log", typeof(Texture2D));
    _warningTex = (Texture2D)Resources.Load("GUI/Sprites/warning", typeof(Texture2D));
    _errorTex = (Texture2D)Resources.Load("GUI/Sprites/error", typeof(Texture2D));
    
  }
  
  public void drawTopSection(float width)
  {


    _labelWidth = (width * 0.8f +3) - _logTex.width*2;
    
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
    int logCount = 0;
    if (!_data.canCollapse)
    {
      foreach (LogMessage message in _data.showingLogs)
      {
        logCount++;
        bool canDisplay = isMessageTypeAvailable(message.type) && isMessageTagAvailable(message.tag);

        if (canDisplay)
        {
          if (_data.selectedLogMessage.isEqualTo(message))
            displayMessage(message, _selectedButtonSkin);
          else if (logCount % 2 == 0)
            displayMessage(message, _buttonSkin);
          else
            displayMessage(message, _buttonSkin2);
        }
      }
    }
    else
    {
      foreach (KeyValuePair<string, CollapsedMessage> entry in _data.collapsedHash)
      {
        LogMessage message = entry.Value.message;
        bool canDisplay = isMessageTypeAvailable(message.type) && isMessageTagAvailable(message.tag);

        if (canDisplay)
        {
          logCount++;
          if (_data.selectedCollapsedMessage.message.hashKey() == message.hashKey())
            displayCollapsedMessage(entry.Value, _selectedButtonSkin);
          else if (logCount % 2 == 0)
            displayCollapsedMessage(entry.Value,_buttonSkin);
          else
            displayCollapsedMessage(entry.Value, _buttonSkin2);
          
        }
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

  void displayCollapsedMessage(CollapsedMessage cmessage, GUISkin skin)
  {
    LogMessage message = cmessage.message;

    GUILayout.BeginHorizontal();

    bool selected = drawIconLabel(cmessage.message.type,skin);
    
    // Log
    selected |= GUILayout.Button(message.log, skin.button, GUILayout.Width(_labelWidth*0.7f), GUILayout.Height(_logTex.height));


    skin.button.alignment = TextAnchor.MiddleCenter;

    // Counter
    selected |= GUILayout.Button("#"+cmessage.counter, skin.button, GUILayout.Width(_labelWidth * 0.15f), GUILayout.Height(_logTex.height));
    
    // Tag
    selected |= GUILayout.Button(message.tag, skin.button, GUILayout.Width(_labelWidth * 0.14f), GUILayout.Height(_logTex.height));

    skin.button.alignment = TextAnchor.UpperLeft;

    if(selected)
    {
      if (_data.selectedCollapsedMessage.message.hashKey() == cmessage.message.hashKey())
        goToSelectedLog(cmessage.message);
      else
        _data.selectedCollapsedMessage = cmessage;
    }

    GUILayout.EndHorizontal();
  }

  void displayMessage(LogMessage message, GUISkin skin)
  {

    GUILayout.BeginHorizontal();

    bool selected = drawIconLabel(message.type, skin);

    // The Log
    selected |= GUILayout.Button(message.log, skin.button, GUILayout.Width(_labelWidth * 0.7f), GUILayout.Height(_logTex.height));

    skin.button.alignment = TextAnchor.MiddleCenter;
    // Timer
    selected |= GUILayout.Button(SMConsoleData.getTimeStamp(message.stamp),skin.button, GUILayout.Width(_labelWidth * 0.15f), GUILayout.Height(_logTex.height));

    // Tag
    selected |= GUILayout.Button(message.tag, skin.button, GUILayout.Width(_labelWidth * 0.14f), GUILayout.Height(_logTex.height));

    skin.button.alignment = TextAnchor.UpperLeft;

    if (selected)
    {
      if (_data.selectedLogMessage.isEqualTo(message))
        goToSelectedLog(message);
      else
        _data.selectedLogMessage = message;
    }

    GUILayout.EndHorizontal();

 }

  bool drawIconLabel(SMLogType type, GUISkin skin)
  {
    Texture texture = _logTex;

    switch (type)
    {
      case SMLogType.NORMAL:
        texture = _logTex;
        break;

      case SMLogType.WARNING:
        texture = _warningTex;
        break;

      case SMLogType.ERROR:
        texture = _errorTex;
        break;

    }

    return GUILayout.Button(texture, skin.button, GUILayout.Height(texture.height), GUILayout.Width(texture.width+5));
  }

  const string ASSET_START_TOKEN = "Asset";
  const string LINE_START_TOKEN = ":line ";
  void goToSelectedLog(LogMessage selected)
  {
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
