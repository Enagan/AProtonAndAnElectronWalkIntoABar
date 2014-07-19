using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SceneManagerWindow : EditorWindow
{

  string filterRoomListString = "";
  string currentlyLoadedRoom = "<noRoom>";

  Vector2 _scrollPosition;

  string currentlyPressed = "";

  Dictionary<string, string> allRooms = new Dictionary<string,string>();

  float roomListUpdateClock = 0;
  float compilationSuccessLabelTimeOut = 0;


  bool loadRoomFoldoutStatus = false;
  bool roomActionFoldoutStatus = false;

  // Add menu item named "My Window" to the Window menu
  [MenuItem("Window/Scene Manager Editor")]
  public static void ShowWindow()
  {
    //Show existing window instance. If one doesn't exist, make one.
    EditorWindow.GetWindow(typeof(SceneManagerWindow));
  }

  void Update()
  {
    if(roomListUpdateClock == 0)
    {
      allRooms = getAllRooms();
    }
    roomListUpdateClock += Time.deltaTime*1000;
    if(roomListUpdateClock >= 5.0f)
    {
      roomListUpdateClock = 0.0f;
    }

    if(compilationSuccessLabelTimeOut > 0)
    {
      compilationSuccessLabelTimeOut -= Time.deltaTime * 1000;
    }
    else
    {
      compilationSuccessLabelTimeOut = 0;
    }

  }

  void OnGUI()
  {
    title = "Scene Manager Editor";

    GUILayout.Label("Scene Manager Editor", EditorStyles.boldLabel);
    //
    LoadRoomsGUI();

    //GUILayout.Label("----------------------------------------------------------------------", EditorStyles.boldLabel);
    EditorGUILayout.Separator();
    RoomActionsGUI();
  }


  void LoadRoomsGUI()
  {
    loadRoomFoldoutStatus = EditorGUILayout.Foldout(loadRoomFoldoutStatus, "Load Rooms");

    if (loadRoomFoldoutStatus)
    {
      filterRoomListString = EditorGUILayout.TextField("Filter Rooms", filterRoomListString);

      List<string> filteredRoomList = new List<string>();
      foreach (KeyValuePair<string, string> room in allRooms)
      {
        string roomName = room.Key;
        if (roomName.Contains(filterRoomListString) || filterRoomListString.Equals(""))
        {
          filteredRoomList.Add(roomName);
        }
      }

      float maxHeight = Mathf.Min(filteredRoomList.Count * 18.2f, 300);

      _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.MaxHeight(maxHeight), GUILayout.Width(position.width));

      foreach (string roomName in filteredRoomList)
      {
        bool isPressed = EditorGUILayout.ToggleLeft(roomName, currentlyPressed.Equals(roomName));
        if (isPressed)
          currentlyPressed = roomName;
      }

      EditorGUILayout.EndScrollView();

      EditorGUILayout.BeginHorizontal();

      if (GUILayout.Button("Load Room", GUILayout.Width(Mathf.Max((position.width - 10) / 2, 100))))
      {
        bool success = EditorApplication.OpenScene(allRooms[currentlyPressed]);
        if (success)
          currentlyLoadedRoom = currentlyPressed;
      }
      if (GUILayout.Button("Refresh Rooms", GUILayout.Width(Mathf.Max((position.width - 10) / 2, 100))))
      {
        allRooms = getAllRooms();
      }

      EditorGUILayout.EndHorizontal();
    }
  }

  void RoomActionsGUI()
  {
    if (currentlyLoadedRoom.Equals("<No valid room is loaded>"))
      roomActionFoldoutStatus = false;
    else
      roomActionFoldoutStatus = true;
    EditorGUILayout.Foldout(roomActionFoldoutStatus, "Loaded Room: " + currentlyLoadedRoom);
    if(roomActionFoldoutStatus)
    {
      if(compilationSuccessLabelTimeOut>0)
        GUILayout.Label("Serialization Success!", EditorStyles.boldLabel);

      if (GUILayout.Button("Serialize Current Room", GUILayout.Width(Mathf.Max(position.width - 10, 100))))
        if (SerializeCurrentRoom())
        {
          compilationSuccessLabelTimeOut = 10;
        }


    }

    //GUILayout.Label(, EditorStyles.boldLabel);
  }
  void GatewaysInRoomAction()
  {
    
  }

  bool SerializeCurrentRoom()
  {
    RoomDefinitionCreator roomDefMaker = new RoomDefinitionCreator();
    roomDefMaker._roomName = currentlyLoadedRoom;
    roomDefMaker.SerializeRoom();
    return true;
  }

  void OnHierarchyChange()
  {
    if (currentlyLoadedRoom.Equals("<No valid room is loaded>") || 
      (allRooms.ContainsKey(currentlyLoadedRoom) && !allRooms[currentlyLoadedRoom].Equals(EditorApplication.currentScene)))
    {
      if (allRooms.ContainsValue(EditorApplication.currentScene))
      {
        foreach (string key in allRooms.Keys)
        {
          if (allRooms[key].Equals(EditorApplication.currentScene))
          {
            currentlyPressed = key;
            currentlyLoadedRoom = currentlyPressed;
            break;
          }
        }
      }
      else
      {
        currentlyLoadedRoom = "<No valid room is loaded>";
      }
    }
  }

  Dictionary<string,string> getAllRooms()
  {
    Dictionary<string, string> resultDict = new Dictionary<string, string>();
    string[] files = Directory.GetFiles("Assets/Scenes/Levels", "*.unity", System.IO.SearchOption.AllDirectories);
    foreach (string f in files)
    { 
      string key = ((f.Replace("Assets/Scenes/Levels\\", "")).Replace("\\"," : ")).Replace(".unity","");
      resultDict.Add(key, f.Replace("\\","/"));
    }
      
    return resultDict;
  }

}