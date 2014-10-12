#if UNITY_EDITOR
using UnityEditor;
#endif


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

using System.Xml;
using System.Xml.Serialization;

// Stores data for the several SMConsole components
public class SMConsoleData
{
  // Log Storage
  public ArrayList logs; // The full log list
  public ArrayList showingLogs; // The visible log list
  public Dictionary<string, CollapsedMessage> collapsedHash; // The collection of collapsed logs

  public LogMessage selectedLogMessage; // The selected Log message
  public CollapsedMessage selectedCollapsedMessage; // The collapsed log message
  
  //  Tags
  public ArrayList tags;
  public ArrayList selectedTags;

  //  SplitWindow 
  public float currentScrollViewHeight; // Height of the split window

  // HeaderBar
  // Counter for number of message of each type (Normal, Warning, Error)
  public int[] logCounter = { 0, 0, 0 };

  // Flags for header buttons
  public bool canCollapse;
  public bool canClearOnPlay;
  public bool showWarnings;
  public bool showErrors;
  public bool showLogs;

  public string searchFilter;
     
  public bool repaint = false; // whether onGUI should repaint

  // Constants
  public const string DEFAULT_SEARCH_STR = "Search Logs";
  public const string EMPTY_TAG = "-";
  public const string EMPTY_STACK_TRACE = "";
  const string ASSET_START_TOKEN = "Asset";
  const string LINE_START_TOKEN = ":line ";

  private static SMConsoleData instance;

  private SMConsoleData()
  {
    init();
  }

  // Singleton
  public static SMConsoleData Instance
   {
      get 
      {
         if (instance == null)
         {
           instance = new SMConsoleData();
         }
         return instance;
      }
   }

  private void init()
  {
    // Initialize counter
    logCounter[(int)SMLogType.NORMAL] = 0;
    logCounter[(int)SMLogType.WARNING] = 0;
    logCounter[(int)SMLogType.ERROR] = 0;

    // init log arrays
    logs = new ArrayList();
    showingLogs = new ArrayList();
    tags = new ArrayList();
    selectedTags = new ArrayList();
    collapsedHash = new Dictionary<string, CollapsedMessage>();

    selectedLogMessage = new LogMessage();
    selectedCollapsedMessage = new CollapsedMessage(new LogMessage());

    canCollapse = false;
    canClearOnPlay = false;
    showWarnings = true;
    showErrors = true;
    showLogs = true;
    searchFilter = DEFAULT_SEARCH_STR;

  }

  // Checks if there is a currently selected message
  public bool isSelectedEmpty()
  {
      if (canCollapse)
      {
          return selectedLogMessage.hashKey().CompareTo(new LogMessage().hashKey()) == 0;
      }
      else
      {
          return selectedCollapsedMessage.message.hashKey().CompareTo(new LogMessage().hashKey()) == 0;
      }
  }

  // Utils
  public void saveLogs()
  {
#if UNITY_EDITOR
    string name = "LogDump-" + DateTime.Now.ToLongDateString() + ".txt";

    string path = EditorUtility.SaveFilePanel
    ("Save current log dump",
          "",
          name,
          "txt");

    if (path.Length == 0)
      return;

    File.WriteAllLines(path, getLogsForWritting());
#endif
  }

  // returns logs as a string format
  private string[] getLogsForWritting()
  {
    string[] forWritting = new string[showingLogs.Count];
    int i = 0;

    if (canCollapse)
    {
      foreach (CollapsedMessage cmessage in showingLogs)
      {
        LogMessage message = cmessage.message;
        forWritting[i] = message.type + "  " + getTimeStamp(message.stamp) + "   " + message.log + "  [" + message.tag + "] ";
        i++;
      }
    }
    else
    {
      foreach (LogMessage message in showingLogs)
      {
        forWritting[i] = message.type + "  " + getTimeStamp(message.stamp) + "   " + message.log + "  [" + message.tag + "] ";
        i++;
      }
    }
    return forWritting;
  }

  // Creates a string timestamp from a date
  public static string getTimeStamp(DateTime time)
  {
    string timeStamp = "";

    timeStamp += time.Hour < 10 ? "0" : "";
    timeStamp += +time.Hour + ":";
    timeStamp += time.Minute < 10 ? "0" : "";
    timeStamp += +time.Minute + ":";
    timeStamp += time.Second < 10 ? "0" : "";
    timeStamp += +time.Second + "::";
    timeStamp += time.Millisecond;

    return timeStamp;
  }

  // Callback for default Debug console logs
  public void HandleLog(string logString, string stackTrace, UnityEngine.LogType type)
  {
    SMLogType BPType = SMLogType.NORMAL;
    switch (type)
    {
      case UnityEngine.LogType.Assert:
      case UnityEngine.LogType.Error:
      case UnityEngine.LogType.Exception:
        BPType = SMLogType.ERROR;
        break;
      case UnityEngine.LogType.Warning:
        BPType = SMLogType.WARNING;
        break;
      case UnityEngine.LogType.Log:
        BPType = SMLogType.NORMAL;
        break;
    }

    SMConsole.Log(logString, "Default Console", BPType);
  }

  // Helper to jump to a Log
  public void goToSelectedLog(LogMessage selected)
  {
    if (canCollapse)
      selected = selectedCollapsedMessage.message;
    else
      selected = selectedLogMessage;

    string[] stackTraces = selectedLogMessage.stackTrace.Split('\n');
    string lineEntry = stackTraces[stackTraces.Length - 1];
    this.jumpToSelectedScript(lineEntry);
  }

  // Helper to know if stack entry allows jumping
  public bool isEntryJumpable(string stackEntry)
  {
    int pathStart = stackEntry.IndexOf(ASSET_START_TOKEN);
    int pathEnd = stackEntry.IndexOf(LINE_START_TOKEN);
    int lineStart = stackEntry.IndexOf(LINE_START_TOKEN) + LINE_START_TOKEN.Length;
    int lineEnd = stackEntry.Length;

    if (pathStart < 0 || pathEnd < 0 || lineStart < 0 || lineEnd < 0)
    {
      return false;
    }
    return true;
  }

  // Helper to jump to the script of a stack Entry
  public bool jumpToSelectedScript(string stackEntry)
  {
#if UNITY_EDITOR
    int pathStart = stackEntry.IndexOf(ASSET_START_TOKEN);
    int pathEnd = stackEntry.IndexOf(LINE_START_TOKEN);
    int lineStart = stackEntry.IndexOf(LINE_START_TOKEN) + LINE_START_TOKEN.Length;
    int lineEnd = stackEntry.Length;

    if (pathStart < 0 || pathEnd < 0 || lineStart < 0 || lineEnd < 0)
    {
      return false;
    }

    string path = stackEntry.Substring(pathStart, pathEnd - pathStart);
    int line = int.Parse(stackEntry.Substring(lineStart, lineEnd - lineStart));

    if (path != null && line > 0)
    {
      UnityEngine.Object script = Resources.LoadAssetAtPath(path, typeof(UnityEngine.Object));
      if (script != null)
      {
        AssetDatabase.OpenAsset(script.GetInstanceID(), line);
        return true;
      }
    }
    #endif
    return false;
  }
}

#region Types

public enum SMLogType
{
  NORMAL = 0,
  WARNING = 1,
  ERROR = 2,
}

#endregion


#region Message Structures

// The LogMessage structure for normal logs
public struct LogMessage
{
  public string log;
  public string tag;
  public string stackTrace;
  public SMLogType type;
  public DateTime stamp;
  public int messageID;

  private static int IDCounter = 0;
  // Constructor
  public LogMessage(string log, string tag, SMLogType type, string stack)
  {
    this.log = log;
    this.tag = tag;
    this.type = type;
    this.stamp = DateTime.Now;
    this.stackTrace = stack;
    this.messageID = ++IDCounter;
  }

  public override string ToString()
  {

    return "[" + base.ToString() + "]";
  }

  public bool isEqualTo(LogMessage message)
  {
    return message.messageID == this.messageID;
  }


  public string hashKey()
  {
    // key should be message + location + tag
    return (this.log + this.tag + this.type);

  }
}

// The CollapsedMessage structure for collapsed logs
public struct CollapsedMessage
{
  public LogMessage message;
  public int counter;

  public CollapsedMessage(LogMessage message)
  {
    this.message = message;
    this.counter = 1;
  }
}

#endregion
