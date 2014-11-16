#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class SMConsoleTopSection {


  SMConsoleData _data;
  GUISkin _buttonSkin;
  GUISkin _buttonSkin2;
  GUISkin _selectedButtonSkin;
  Texture2D _logTex;
  Texture2D _warningTex;
  Texture2D _errorTex;
  float _labelWidth;

  GUISkin _logSkin;
  

  public SMConsoleTopSection()
  {

    // create skins for buttons
    _data = SMConsoleData.Instance;
    _buttonSkin = Resources.Load("GUI/Skins/SMConsoleSkin") as GUISkin;
    _buttonSkin2 = Resources.Load("GUI/Skins/SMConsoleSkin2") as GUISkin;
    _selectedButtonSkin = Resources.Load("GUI/Skins/SMConsoleSkin3") as GUISkin;
    _logSkin = Resources.Load("GUI/Skins/LogButton") as GUISkin;

    // create textures for icons
    _logTex = (Texture2D)Resources.Load("GUI/Sprites/log", typeof(Texture2D));
    _warningTex = (Texture2D)Resources.Load("GUI/Sprites/warning", typeof(Texture2D));
    _errorTex = (Texture2D)Resources.Load("GUI/Sprites/error", typeof(Texture2D));

      
  }
  
  public void drawTopSection(float width, float height)
  {

     
    _labelWidth = (width * 0.8f +3) - _logTex.width*2;
    float areaHeight = _data.isSelectedEmpty() ? height : _data.currentScrollViewHeight;

    // Upper section
    GUILayout.BeginHorizontal();

    // Log Scroll
    _logWindowScroll = GUILayout.BeginScrollView(_logWindowScroll, GUILayout.MaxHeight(areaHeight - 16), GUILayout.Width(width * 0.8f + 3));
    drawLogWindow();
    GUILayout.EndScrollView();

    // Tag Scroll
    _tagWindowScroll = GUILayout.BeginScrollView(_tagWindowScroll, GUILayout.MaxHeight(areaHeight - 16));
    drawTagWindow();
    GUILayout.EndScrollView();

    GUILayout.EndHorizontal();
  }

  // Tag Window

  Vector2 _tagWindowScroll;

  // draws tags
  void drawTagWindow()
  {
    foreach (string tag in _data.tags)
    {

      // if selected message is not visible reset it
      if (tag == _data.selectedLogMessage.tag && !_data.canCollapse && !_data.selectedTags.Contains(tag))
      {
        _data.selectedLogMessage = new LogMessage();
      }
      else if (tag == _data.selectedCollapsedMessage.message.tag && _data.canCollapse && !_data.selectedTags.Contains(tag))
      {
        _data.selectedCollapsedMessage = new CollapsedMessage(new LogMessage());
      }

      // check if tag is selected and create toggle button
      bool isSelected = _data.selectedTags.Contains(tag);
      isSelected = GUILayout.Toggle(isSelected, tag, "ToolbarButton");

      // Add or remove message according to tag being selected
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
    if (!_data.canCollapse) // Draw normal messages
    {
      foreach (LogMessage message in _data.showingLogs)
      {
        logCount++;
        bool canDisplay = isMessageTypeAvailable(message.type) && isMessageTagAvailable(message.tag);

        if (canDisplay)
        {
          if (_data.selectedLogMessage.isEqualTo(message))
            displayMessage(message, _selectedButtonSkin, true);
          else if (logCount % 2 == 0)
            displayMessage(message, _buttonSkin,false);
          else
              displayMessage(message, _buttonSkin2, false);
        }
      }
    }
    else // Draw collapsed messages
    {
      foreach (KeyValuePair<string, CollapsedMessage> entry in _data.collapsedHash) 
      {
        LogMessage message = entry.Value.message;
        bool canDisplay = isMessageTypeAvailable(message.type) && isMessageTagAvailable(message.tag);

        if (canDisplay)
        {
          logCount++;
          if (_data.selectedCollapsedMessage.message.hashKey() == message.hashKey())
            displayCollapsedMessage(entry.Value, _selectedButtonSkin,true);
          else if (logCount % 2 == 0)
            displayCollapsedMessage(entry.Value,_buttonSkin,false);
          else
            displayCollapsedMessage(entry.Value, _buttonSkin2,false);
          
        }
      }
    }

  }

  // returns true if the tag is available to display
  bool isMessageTagAvailable(string tag)
  {
    return _data.selectedTags.Contains(tag);
  }

  // returns true if the type (normal, warning , error) is available to display
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

  // Displays a collapsed message with a specific button skin
  void displayCollapsedMessage(CollapsedMessage cmessage, GUISkin skin, bool isSelected)
  {
    LogMessage message = cmessage.message;

    GUILayout.BeginHorizontal();

    bool selected = drawIconLabel(cmessage.message.type,skin);
  

    float unselectedWidth = 0.025f;
    // The Log


    selected |= GUILayout.Button(message.log, skin.button, GUILayout.Width(_labelWidth * 0.65f), GUILayout.Height(_logTex.height));



    skin.button.alignment = TextAnchor.MiddleCenter;

    // Counter
    selected |= GUILayout.Button("#"+cmessage.counter, skin.button, GUILayout.Width(_labelWidth * 0.15f), GUILayout.Height(_logTex.height));
    
    // Tag
    
      selected |= GUILayout.Button(message.tag, skin.button, GUILayout.Width(_labelWidth * 0.14f), GUILayout.Height(_logTex.height));


    // Unselect
      if (isSelected && GUILayout.Button("X", skin.button, GUILayout.Width(_labelWidth * unselectedWidth), GUILayout.Height(_logTex.height)))
    {
        _data.selectedLogMessage = new LogMessage();
    }

    skin.button.alignment = TextAnchor.UpperLeft;

    if(selected) 
    {
      if (_data.selectedCollapsedMessage.message.hashKey() == cmessage.message.hashKey())
        _data.goToSelectedLog(cmessage.message); // Jump to Line
      else
        _data.selectedCollapsedMessage = cmessage;
    }

    GUILayout.EndHorizontal();
  }

  // Displays a normal message
  void displayMessage(LogMessage message, GUISkin skin, bool isSelected)
  {

    GUILayout.BeginHorizontal();

    bool selected = drawIconLabel(message.type, skin);


    float unselectedWidth = 0.025f;
    // The Log

    selected |= GUILayout.Button(message.log, skin.button, GUILayout.Width(_labelWidth * 0.65f), GUILayout.Height(_logTex.height));

    skin.button.alignment = TextAnchor.MiddleCenter;
    // Timer
    selected |= GUILayout.Button(SMConsoleData.getTimeStamp(message.stamp),skin.button, GUILayout.Width(_labelWidth * 0.15f), GUILayout.Height(_logTex.height));

    // Tag
    selected |= GUILayout.Button(message.tag, skin.button, GUILayout.Width(_labelWidth * 0.14f), GUILayout.Height(_logTex.height));

    // Unselect
    if (isSelected && GUILayout.Button("X", skin.button, GUILayout.Width(_labelWidth * unselectedWidth), GUILayout.Height(_logTex.height)))
    {
        _data.selectedLogMessage = new LogMessage();
    }

    skin.button.alignment = TextAnchor.UpperLeft;

    if (selected)
    {
      if (_data.selectedLogMessage.isEqualTo(message))
        _data.goToSelectedLog(message);
      else
        _data.selectedLogMessage = message;
    }

    GUILayout.EndHorizontal();

 }

  // Draws icon of message type
  bool drawIconLabel(SMLogType type, GUISkin skin)
  {
    Texture2D texture = _logTex;

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


    _logSkin.button.normal.background = texture;
    _logSkin.button.active.background = texture;
    _logSkin.button.focused.background = texture;
    _logSkin.button.hover.background = texture;
    _logSkin.button.onNormal.background = texture;
    _logSkin.button.onActive.background = texture;
    _logSkin.button.onFocused.background = texture;
    _logSkin.button.onHover.background = texture;
    return GUILayout.Button("", _logSkin.button, GUILayout.Height(texture.height), GUILayout.Width(texture.width));
  }

 



}


#endif
