using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

// Stores data for the several SMConsole components
public class SMConsoleData
{
  // Log Storage
  public ArrayList logs;
  public ArrayList showingLogs;
  public Dictionary<string, CollapsedMessage> collapsedHash;

  public LogMessage selectedLogMessage;
  public CollapsedMessage selectedCollapsedMessage;

  //  Tags
  public ArrayList tags;
  public ArrayList selectedTags;

  //  SplitWindow 
  public float currentScrollViewHeight;

  // HeaderBar
  public int[] logCounter = { 0, 0, 0 };

  public bool canCollapse;
  public bool canClearOnPlay;
  public bool showWarnings;
  public bool showErrors;
  public bool showLogs;

  public string searchFilter;


  // Constants

  public const string DEFAULT_SEARCH_STR = "Search Logs";
  public const string EMPTY_TAG = "-";
  public const string EMPTY_STACK_TRACE = "";

  private static SMConsoleData instance;

  private SMConsoleData()
  {
    init();
  }

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
    logCounter[(int)SMLogType.NORMAL] = 0;
    logCounter[(int)SMLogType.WARNING] = 0;
    logCounter[(int)SMLogType.ERROR] = 0;

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


  // Utils
  public void saveLogs()
  {
    string name = "LogDump-" + DateTime.Now.ToLongDateString() + ".txt";

    string path = EditorUtility.SaveFilePanel
    ("Save current log dump",
          "",
          name,
          "txt");

    if (path.Length == 0)
      return;

    File.WriteAllLines(path, getLogsForWritting());
  }

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

  public static string getTimeStamp(DateTime time)
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

public struct LogMessage
{
  public string log;
  public string tag;
  public string stackTrace;
  public SMLogType type;
  public DateTime stamp;

  // Constructor
  public LogMessage(string log, string tag, SMLogType type, string stack)
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


  public string hashKey()
  {
    // key should be message + location + tag
    return (this.log + this.tag + this.type);

  }
}


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

