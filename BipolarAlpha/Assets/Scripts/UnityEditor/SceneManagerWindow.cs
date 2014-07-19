  
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public class SceneManagerWindow : EditorWindow
{
  const float SIZE_OF_ELEMENT_IN_LIST = 18.5f;

  string filterRoomListString = "";
  string currentlyLoadedRoom = "<noRoom>";

  Vector2 _roomListScrollPosition;
  Vector2 _gatewaysScrollPosition;

  string currentlyPressed = "";

  Dictionary<string, string> allRooms = new Dictionary<string,string>();

  float roomListUpdateClock = 0;
  float compilationSuccessLabelTimeOut = 0;


  bool loadRoomFoldoutStatus = false;
  bool roomActionFoldoutStatus = false;
  bool gatewaysFoldoutStatus = true;

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

    GatewaysInRoomAction();

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

      float maxHeight = Mathf.Min(filteredRoomList.Count * SIZE_OF_ELEMENT_IN_LIST, 300);

      _roomListScrollPosition = EditorGUILayout.BeginScrollView(_roomListScrollPosition, GUILayout.MaxHeight(maxHeight), GUILayout.Width(position.width));

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
    gatewaysFoldoutStatus = EditorGUILayout.Foldout(gatewaysFoldoutStatus, "Gateways in room:");
    if (roomActionFoldoutStatus && gatewaysFoldoutStatus)
    {
      List<GameObject> Gateways = new List<GameObject>();

      object[] allObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject));

      foreach (object thisObject in allObjects)
      {
        GameObject castObject = ((GameObject)thisObject);
        if (castObject.activeInHierarchy && castObject.transform.parent == null && BipolarUtilityFunctions.GetComponentsInHierarchy<GatewayTriggerScript>(castObject.transform).Count > 0)
        {
          Gateways.Add(castObject);
        }
      }

      float maxHeight = Mathf.Min(Gateways.Count * 5 * allRooms.Keys.Count * SIZE_OF_ELEMENT_IN_LIST, 20 * SIZE_OF_ELEMENT_IN_LIST);
      _gatewaysScrollPosition = EditorGUILayout.BeginScrollView(_gatewaysScrollPosition, GUILayout.MaxHeight(maxHeight), GUILayout.Width(position.width));

      foreach (GameObject gate in Gateways)
      {
        if (gate != null)
        {
          GatewayTriggerScript gateTrigger = BipolarUtilityFunctions.GetComponentsInHierarchy<GatewayTriggerScript>(gate.transform)[0];
          GUILayout.BeginHorizontal();

          GUILayout.Label(" - " + gate.name);


          if (GUILayout.Button("Select"))
          {
            //SceneView.currentDrawingSceneView.AlignViewToObject(gate.transform);
            Selection.activeGameObject = gate;
          }

          GUILayout.EndHorizontal();

          GUILayout.BeginHorizontal();
          GUILayout.Label("  To");
          gateTrigger.connectsTo = EditorGUILayout.TextArea(gateTrigger.connectsTo, GUILayout.Width(position.width / 2));

          bool isConnected = false;
          string gatewayConnectedTo = "";
          foreach (string roomName in allRooms.Keys)
          {
            string simpleRoomName = roomName.Substring(roomName.IndexOf(":") + 2);
            isConnected = isConnected || gateTrigger.connectsTo.Equals(simpleRoomName);
            if (gateTrigger.connectsTo.Equals(simpleRoomName))
              gatewayConnectedTo = roomName;
          }

          if (GUILayout.Button("Goto") && isConnected)
          {
            bool success = EditorApplication.OpenScene(allRooms[gatewayConnectedTo]);
            if (success)
              currentlyLoadedRoom = currentlyPressed;
          }

          GUILayout.EndHorizontal();

          GUIStyle style = new GUIStyle();
          if (isConnected)
          {
            style.normal.textColor = Color.green;
            GUILayout.Label("   Connected!", style);
          }

          else
          {
            style.normal.textColor = Color.red;
            GUILayout.Label("   Connection Error!", style);
          }

        }
      }
      EditorGUILayout.EndScrollView();
    }
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