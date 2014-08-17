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
  ArrayList _showingLogs;
  ArrayList _tags;
  ArrayList _selectedTags;
  Dictionary<string,CollapsedMessage> _collapsedHash;
  Texture2D _whiteTexture;

  int[] _logCounter = { 0, 0, 0 };

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
    _showingLogs = new ArrayList();
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

    int xSize = (int)this.position.width;
    int ySize = 16;
    _whiteTexture = new Texture2D(xSize, ySize);
    for (int x = 0; x < xSize; x++)
      for (int y = 0; y < ySize; y++)
        _whiteTexture.SetPixel(x, y, Color.white);
 
    _whiteTexture.Apply();
    
    
    initHeaderBar();
    
  }

  void OnGUI()
  {

    title = "Bipolar Console";
    drawHeaderBar();
    GUILayout.BeginVertical();

    // Upper section
    GUILayout.BeginHorizontal();

    // Log Scroll
    _logWindowScroll = GUILayout.BeginScrollView(_logWindowScroll, GUILayout.MaxHeight(_splitWindow.currentScrollViewHeight- 16), GUILayout.Width(this.position.width*0.8f+3));
    drawLogWindow();
    GUILayout.EndScrollView();

    // Tag Scroll
    _tagWindowScroll = GUILayout.BeginScrollView(_tagWindowScroll, GUILayout.MaxHeight(_splitWindow.currentScrollViewHeight - 16));
    drawTagWindow();
    GUILayout.EndScrollView();

    GUILayout.EndHorizontal();

    _splitWindow.drawWindow(this.position.width);


    GUILayout.BeginHorizontal();

    displayBotConsole();
    //GUILayout.EndScrollView();
    /*
     
   
    GUILayout.EndScrollView();

    _botRightWindowScroll = GUILayout.BeginScrollView(_botRightWindowScroll, GUILayout.Width(this.position.width * 0.3f));
    
    GUILayout.EndScrollView();
    */
    GUILayout.EndHorizontal();
    
    GUILayout.EndVertical();

  }

  // Tag Window

   Vector2 _tagWindowScroll;
   LogMessage _selectedLogMessage;
   CollapsedMessage _selectedCollapsedMessage;

   void drawTagWindow()
   {
     foreach (string tag in _tags)
     {
  
       bool isSelected = _selectedTags.Contains(tag);
       isSelected = GUILayout.Toggle(isSelected,tag, "ToolbarButton");

       if (!isSelected && _selectedTags.Contains(tag))
       {
         _selectedTags.Remove(tag);
       }
       else if(isSelected && !_selectedTags.Contains(tag))
       {
         _selectedTags.Add(tag);
       }
     }
   }
    
  // Log Window

  Vector2 _logWindowScroll;
  Vector2 _botLeftWindowScroll;
  Vector2 _botMidWindowScroll;
  Vector2 _botRightWindowScroll;
  
  void drawLogWindow()
  {
    if (!_canCollapse)
    {
      foreach (LogMessage message in _showingLogs)
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

  void displayBotConsole()
  {
    _botLeftWindowScroll = GUILayout.BeginScrollView(_botLeftWindowScroll, GUILayout.MaxHeight(this.position.height - _splitWindow.currentScrollViewHeight), GUILayout.Width(this.position.width * 0.3f));
    displaySelectedMessage();
    GUILayout.EndScrollView();

    _botMidWindowScroll = GUILayout.BeginScrollView(_botMidWindowScroll, GUILayout.MaxHeight(this.position.height - _splitWindow.currentScrollViewHeight), GUILayout.Width(this.position.width * 0.7f));
    displayStackTrace();
    GUILayout.EndScrollView();
  }

  void displaySelectedMessage()
  {
    string selectedMessage;
    if (!_canCollapse)
    {
      selectedMessage = _selectedLogMessage.log;
    }
    else
    {
      selectedMessage = _selectedCollapsedMessage.message.log;
    }
    EditorGUILayout.SelectableLabel(selectedMessage, GUI.skin.label);
  }

  void displayStackTrace()
  {
    string stackTrace;
    if (!_canCollapse)
    {
      stackTrace = _selectedLogMessage.stackTrace;
    }
    else
    {
      stackTrace = _selectedCollapsedMessage.message.stackTrace;
    }

    GUIStyle skin = GUI.skin.label;
    skin.wordWrap = true;
    GUILayout.Label(stackTrace, skin);
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
    if(GUILayout.Button(message.type + "   " + message.log + "  [" + message.tag + "] " + cmessage.counter,GUI.skin.label))
      _selectedCollapsedMessage = cmessage;
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
   
    string messageStr = message.type + "  " + getTimeStamp(message.stamp) + "   " + message.log + "  [" + message.tag + "] ";
    GUI.contentColor = messageColor;
    //EditorGUILayout.(message.type + "  " + getTimeStamp(message.stamp) + "   " + message.log + "  [" + message.tag + "] ");
    
    if (GUILayout.Button(messageStr))
      _selectedLogMessage = message;

      GUI.contentColor = prevColor;
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

     string prevSearch = _searchFilter;
     _searchFilter = GUILayout.TextField(_searchFilter, EditorStyles.textField, GUILayout.MaxWidth(SEARCH_WIDTH));

      // soft reset
     if (_searchFilter != DEFAULT_SEARCH_STR && prevSearch == DEFAULT_SEARCH_STR)
     {
       if (_searchFilter.Length < DEFAULT_SEARCH_STR.Length) // Something was removed
         _searchFilter = "";
       else if (_searchFilter.Length > DEFAULT_SEARCH_STR.Length) // Something was added
       {
         _searchFilter = _searchFilter.Trim().Replace(DEFAULT_SEARCH_STR, "");
       }
     }

      // will search
     if (_searchFilter != DEFAULT_SEARCH_STR && prevSearch != _searchFilter)
     {
       searchLogs();
     }
      
     if (GUILayout.Button("X", EditorStyles.toolbarButton, new GUILayoutOption[1] { GUILayout.Width(20) }))
       clearSearch();
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

     if (_showLogs = GUILayout.Toggle(_showLogs, "N(" + _logCounter[(int)LogType.NORMAL] + ")", "ToolbarButton", new GUILayoutOption[1] { GUILayout.Width(80) }))
        normalButton();

     if (_showWarnings = GUILayout.Toggle(_showWarnings, "W(" + _logCounter[(int)LogType.WARNING] + ")", "ToolbarButton", new GUILayoutOption[1] { GUILayout.Width(80) }))
       warningButton();

     if (_showErrors = GUILayout.Toggle(_showErrors, "E(" + _logCounter[(int)LogType.ERROR]+")", "ToolbarButton", new GUILayoutOption[1] { GUILayout.Width(80) }))
       errorButton();

     GUILayout.Space(SEARCH_MARGIN * 7);

     if (GUILayout.Button("All", EditorStyles.toolbarButton, new GUILayoutOption[1] { GUILayout.Width(50) }))
       allTagsButton();

     if (GUILayout.Button("None", EditorStyles.toolbarButton, new GUILayoutOption[1] { GUILayout.Width(50) }))
       noTagsButton();


    GUILayout.EndHorizontal();

    GUI.backgroundColor = c; // reset to old value
  }

    void clearSearch()
    {
      _showingLogs = new ArrayList(_logs);
      _searchFilter = DEFAULT_SEARCH_STR;
    }

    void searchLogs()
    {
      _showingLogs = new ArrayList();
      if (!_canCollapse)
      {
        foreach (LogMessage message in _logs)
          if (message.log.IndexOf(_searchFilter, StringComparison.OrdinalIgnoreCase) >= 0)
            _showingLogs.Add(message);
      }
      else
      {
        foreach (CollapsedMessage message in _logs)
          if (message.message.log.IndexOf(_searchFilter, StringComparison.OrdinalIgnoreCase) >= 0)
            _showingLogs.Add(message);
      }

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
    for (int i = 0; i < _logCounter.Length; i++)
      _logCounter[i] = 0;

    _showingLogs = new ArrayList();
    _logs = new ArrayList();
    _tags = new ArrayList();
    _selectedTags = new ArrayList();
    _collapsedHash = new Dictionary<string, CollapsedMessage>();
  }

  void saveButton()
  {
    string name = "LogDump-" + DateTime.Now.ToLongDateString() + ".txt";

    string path = EditorUtility.SaveFilePanel
    ("Save current log dump",
          "",
          name,
          "txt");

    if(path.Length == 0)
      return;

    File.WriteAllLines(path, getLogsForWritting());
  }

  string[] getLogsForWritting()
  {
    string[] forWritting = new string[_showingLogs.Count];
    int i = 0;
    
    if(_canCollapse)
    {
      foreach(CollapsedMessage cmessage in _showingLogs)
      {
        LogMessage message = cmessage.message;
        forWritting[i] = message.type + "  " + getTimeStamp(message.stamp) + "   " + message.log + "  [" + message.tag + "] ";
        i++;
      }
    }
    else
    {
      foreach(LogMessage message in _showingLogs)
      {
        forWritting[i] = message.type + "  " + getTimeStamp(message.stamp) + "   " + message.log + "  [" + message.tag + "] ";
        i++;
      }
    }

    return forWritting;
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
    public string stackTrace;
    public LogType type;
    public DateTime stamp; 

    // Constructor
    public LogMessage(string log, string tag, LogType type,string stack) 
   {
      this.log = log;
      this.tag = tag;
      this.type = type;
      this.stamp = DateTime.Now;
      this.stackTrace = stack;
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
    NORMAL = 0,
    WARNING = 1,
    ERROR = 2,
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
    LogMessage message = new LogMessage(log, tag, type,Environment.StackTrace);
    _logs.Add(message);

    _logCounter[(int)type]++;

    string hashKey = generateHashKey(message);

    if (!_collapsedHash.ContainsKey(hashKey))
    {
      CollapsedMessage collapsed = new CollapsedMessage(message);
      _collapsedHash.Add(hashKey, collapsed);
    }
    else
    {
      CollapsedMessage collapsed = _collapsedHash[hashKey];
      collapsed.counter++;
      _collapsedHash[hashKey] = collapsed;
    }

    if (!_tags.Contains(message.tag))
    {
      _tags.Add(message.tag);
      _selectedTags.Add(message.tag);
    }

    // See if should be added to currently showing
     if (_searchFilter == DEFAULT_SEARCH_STR || _searchFilter == "")
    {
      _showingLogs.Add(message);
    }
    else
    {
       if(!_canCollapse)
       {
         if(message.log.IndexOf(_searchFilter, StringComparison.OrdinalIgnoreCase) >= 0)
            _showingLogs.Add(message);
       }
       else if(_collapsedHash[hashKey].counter == 1) // first instance
       {
         if (message.log.IndexOf(_searchFilter, StringComparison.OrdinalIgnoreCase) >= 0)
           _showingLogs.Add(_collapsedHash[hashKey]);
       }

      
      
    }
  }

  protected string generateHashKey(LogMessage message)
  {
    // key should be message + location + tag
    return (message.log + message.tag);

  }
}

//#endif