//#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System;

public class BipolarConsoleWindow : EditorWindow
{

  SplitWindow _splitWindow;
  ArrayList _logs;
  ArrayList _tags;
  ArrayList _selectedTags;
  Dictionary<string,CollapsedMessage> _collapsedHash;
  Texture2D _whiteTexture;

  [MenuItem("Window/Bipolar Console")]
  public static void ShowWindow()
  {
    //Show existing window instance. If one doesn't exist, make one.
    EditorWindow.GetWindow(typeof(BipolarConsoleWindow));
  }

  public void OnEnable()
  {

    _splitWindow = (SplitWindow)ScriptableObject.CreateInstance("SplitWindow");
    _splitWindow.OnEnable();
    _logs = new ArrayList();
    _tags = new ArrayList();
    _selectedTags = new ArrayList();
    _collapsedHash = new Dictionary<string, CollapsedMessage>();
    BPLog("Hello");
    BPLog("HelloTagged","A");
    BPLog("HelloNormal", "B",LogType.NORMAL);
    BPLog("HelloError", "B", LogType.ERROR);
    BPLog("HelloWarning", "C",LogType.WARNING);
    BPLog("HelloWarning", "D", LogType.WARNING);
    BPLog("HelloWarning", "E", LogType.WARNING);


    _whiteTexture = new Texture2D(1, 1);
    _whiteTexture.SetPixel(1, 1, Color.white);
    
    
    initHeaderBar();
    
  }

  void Update()
  {
    if (_searchFilter != DEFAULT_SEARCH_STR && isKeyPressed(KeyCode.Return))
    {
      searchLogs();

      if (_searchFilter == "")
        _searchFilter = DEFAULT_SEARCH_STR;
    }
  }

  void OnGUI()
  {

    title = "Bipolar Console";
    drawHeaderBar();
    GUILayout.BeginVertical();

    // Upper section
    GUILayout.BeginHorizontal();

    // Log Scroll
    _logWindowScroll = GUILayout.BeginScrollView(_logWindowScroll, GUILayout.MaxHeight(_splitWindow.currentScrollViewHeight- 16), GUILayout.Width(this.position.width*0.8f-1));
    drawLogWindow();
    GUILayout.EndScrollView();

    // Tag Scroll
    _tagWindowScroll = GUILayout.BeginScrollView(_tagWindowScroll, GUILayout.MaxHeight(_splitWindow.currentScrollViewHeight - 16));
    drawTagWindow();
    GUILayout.EndScrollView();

    GUILayout.EndHorizontal();

    _splitWindow.drawWindow(this.position.width);
    GUILayout.EndVertical();
 
  }

  // Tag Window

   Vector2 _tagWindowScroll;
   void drawTagWindow()
   {
     foreach (string tag in _tags)
     {
       GUILayout.Label(tag);
     }
   }

  // Log Window

  Vector2 _logWindowScroll;
  
  void drawLogWindow()
  {
    if (!_canCollapse)
    {
      foreach (LogMessage message in _logs)
      {

        bool canDisplay = isMessageTypeAvailable(message.type) && isMessageTagAvailable(message.tag);

        if (canDisplay)
          displayMessage(message);
      }
    }
    else
    {
      foreach (KeyValuePair<string, CollapsedMessage> entry in _collapsedHash)
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
    return _selectedTags.Contains(tag);
  }
  bool isMessageTypeAvailable(LogType type)
  {
    bool isAvailable = false;

    switch (type)
    {
      case LogType.NORMAL:
        isAvailable = _showLogs;
        break;
      case LogType.WARNING:
        isAvailable = _showWarnings;
        break;

      case LogType.ERROR:
        isAvailable = _showErrors;
        break;
    }
    return isAvailable;
  }

  void displayCollapsedMessage(CollapsedMessage cmessage)
  {
    LogMessage message = cmessage.message;
    GUILayout.Label(message.type + "   " + message.log + "  [" + message.tag + "] " + cmessage.counter);
  }

  void displayMessage(LogMessage message)
  {
    Color prevColor = GUI.backgroundColor;
    Color messageColor  = Color.white;
    switch(message.type)
    {
      case LogType.WARNING:
         messageColor = Color.yellow;
        break;

      case LogType.ERROR:
        messageColor = Color.red;
        break;
    }
    GUI.backgroundColor = messageColor;
    EditorGUILayout.SelectableLabel(message.type + "  " + getTimeStamp(message.stamp) + "   " + message.log + "  [" + message.tag + "] ");
    GUI.backgroundColor = prevColor;
  }

  string getTimeStamp(DateTime time)
  {
    string timeStamp = "[";

    timeStamp += time.Hour < 10 ? "0" : "";
    timeStamp += +time.Hour + ":";
    timeStamp += time.Minute < 10 ? "0" : "";
    timeStamp += +time.Minute + "::";
    timeStamp += time.Second < 10 ? "0" : "";
    timeStamp += +time.Second + ":";
    timeStamp += time.Millisecond + "]";

    return timeStamp;
  }

 
  // HeaderBar

  private bool _canCollapse;
  private bool _canClearOnPlay;
  private bool _showWarnings;
  private bool _showErrors;
  private bool _showLogs;

  
  private string _searchFilter;
  const string DEFAULT_SEARCH_STR = "Search Logs";
  const int SEARCH_WIDTH = 300;
  const int SEARCH_MARGIN = 10;

  void initHeaderBar()
  {
    _canCollapse = false;
    _canClearOnPlay = false;
    _showWarnings = true;
    _showErrors = true;
    _showLogs = true;
    _searchFilter = DEFAULT_SEARCH_STR;

  }

    void drawHeaderBar()
    {

     Color c = GUI.backgroundColor; // store value

     GUILayout.BeginHorizontal();

     // Search Field

     Vector2 cursorPos = Input.compositionCursorPos;

     Rect searchFilterRect = new Rect(SEARCH_MARGIN, 1, SEARCH_WIDTH, 14);

     _searchFilter = GUILayout.TextField(_searchFilter, EditorStyles.textField, GUILayout.MaxWidth(SEARCH_WIDTH));



    // Buttons

     GUILayout.Space(SEARCH_MARGIN *2);

     if (GUILayout.Button("Clear", EditorStyles.toolbarButton, new GUILayoutOption[1] { GUILayout.Width(50) }))
       clearButton();

     if (GUILayout.Button("Save", EditorStyles.toolbarButton, new GUILayoutOption[1] { GUILayout.Width(50) }))
       saveButton();

     GUILayout.Space(SEARCH_MARGIN * 2);

     if (_canCollapse = GUILayout.Toggle(_canCollapse, "Collapse", "ToolbarButton", new GUILayoutOption[1] { GUILayout.Width(80) }))
       collapseButton();

     if (_canClearOnPlay = GUILayout.Toggle(_canClearOnPlay, "Clear on Play", "ToolbarButton", new GUILayoutOption[1] { GUILayout.Width(80) }))
       clearOnPlayButton();

     GUILayout.Space(SEARCH_MARGIN * 2);

     if (_showLogs = GUILayout.Toggle(_showLogs, "N", "ToolbarButton", new GUILayoutOption[1] { GUILayout.Width(80) }))
        normalButton();

     if (_showWarnings = GUILayout.Toggle(_showWarnings, "W", "ToolbarButton", new GUILayoutOption[1] { GUILayout.Width(80) }))
       warningButton();

     if (_showErrors = GUILayout.Toggle(_showErrors, "E", "ToolbarButton", new GUILayoutOption[1] { GUILayout.Width(80) }))

       errorButton();

     GUILayout.Space(SEARCH_MARGIN * 6);

     if (GUILayout.Button("All", EditorStyles.toolbarButton, new GUILayoutOption[1] { GUILayout.Width(50) }))
       allTagsButton();

     if (GUILayout.Button("None", EditorStyles.toolbarButton, new GUILayoutOption[1] { GUILayout.Width(50) }))
       noTagsButton();


    GUILayout.EndHorizontal();

    GUI.backgroundColor = c; // reset to old value
  }

    void searchLogs()
    {
      BPLog("LOGS");
    }

  void normalButton()
  {

  }
  void warningButton()
  {

  }
  void errorButton()
  {

  }
  void collapseButton()
  {

  }

  void clearOnPlayButton()
  {

  }

  void clearButton()
  {
    _logs = new ArrayList();
    _tags = new ArrayList();
    _selectedTags = new ArrayList();
    _collapsedHash = new Dictionary<string, CollapsedMessage>();
  }

  void saveButton()
  {

  }

  void allTagsButton()
  {
     _selectedTags = new ArrayList(_tags);
  }

  void noTagsButton()
  {
    _selectedTags = new ArrayList();
  }

  protected struct CollapsedMessage
  {
    public LogMessage message;
    public int counter;

    public CollapsedMessage(LogMessage message)
    {
      this.message = message;
      this.counter = 1;
    }
  }

  // General purpose
  protected struct LogMessage
  {
    public string log;
    public string tag;
    public LogType type;
    public DateTime stamp; 

    // Constructor
    public LogMessage(string log, string tag, LogType type) 
   {
      this.log = log;
      this.tag = tag;
      this.type = type;
      this.stamp = DateTime.Now;
   }
    public override string ToString()
    {
      
      return "[" + base.ToString() + "]";
    }
  }

  bool isKeyPressed(KeyCode code)
  {
    bool isPressed = Input.GetKeyDown(KeyCode.Return);

    return isPressed;
  }
  
  const string EMPTY_TAG = "-";

  public enum LogType
  {
    NORMAL,
    WARNING,
    ERROR,
  }

  public void BPLog(string log)
  {
    BPLog(log, EMPTY_TAG);
  }

  public void BPLog(string log, string tag)
  {
    BPLog(log, tag,LogType.NORMAL);
  }

  public void BPLog(string log, string tag, LogType type)
  {
    LogMessage message = new LogMessage(log, tag, type);
    _logs.Add(message);

    string hashKey = generateHashKey(message);

    if (!_collapsedHash.ContainsKey(hashKey))
    {
      CollapsedMessage collapsed = new CollapsedMessage(message);
      _collapsedHash.Add(hashKey, collapsed);
    }
    else
    {
      Debug.Log("incrementing" + _collapsedHash[hashKey] + _collapsedHash[hashKey].counter);
      CollapsedMessage collapsed = _collapsedHash[hashKey];
      collapsed.counter++;
      _collapsedHash[hashKey] = collapsed;
    }

    if (!_tags.Contains(message.tag))
    {
      _tags.Add(message.tag);
      _selectedTags.Add(message.tag);
    }
  }

  protected string generateHashKey(LogMessage message)
  {
    // key should be message + location + tag
    return (message.log + message.tag);

  }
}

//#endif