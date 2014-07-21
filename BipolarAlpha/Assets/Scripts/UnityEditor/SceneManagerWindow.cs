  
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public class SceneManagerWindow : EditorWindow
{
  const float SIZE_OF_ELEMENT_IN_LIST = 18.5f;
  const float TIME_TO_UPDATE_BIG_VARIABLES = 5.0f;

  string filterRoomListString = "";
  string currentlyLoadedRoom = "<noRoom>";
  string _rootPath = "";

  string currentlyPressed = "";

  Dictionary<string, string> allRooms = new Dictionary<string,string>();

  float bigVariablesUpdateTimeout = 0.0f;
  float compilationSuccessLabelTimeOut = 0;

  SaveState _initialWorldState = null;

  // Foldouts
  bool loadRoomFoldoutStatus = false;
  bool roomActionFoldoutStatus = false;
  bool gatewaysFoldoutStatus = true;
  bool gameStateFoldoutStatus = true;

  // Scroll Positions
  Vector2 _fullWindowScrollView;
  Vector2 _roomListScrollPosition;
  Vector2 _gatewaysScrollPosition;

  // Add menu item named "My Window" to the Window menu
  [MenuItem("Window/Scene Manager Editor")]
  public static void ShowWindow()
  {
    //Show existing window instance. If one doesn't exist, make one.
    EditorWindow.GetWindow(typeof(SceneManagerWindow));
  }

  void Start()
  {
    
  }

  void Update()
  {
    if (Application.isEditor)
    {
      _rootPath = "Assets/Resources/Levels/";
    }
    else
    {
      _rootPath = Application.dataPath + "/Levels/";
    }

    if (_initialWorldState == null)
      _initialWorldState = XMLSerializer.Deserialize<SaveState>(_rootPath + "SaveState.lvl");


    if(bigVariablesUpdateTimeout == 0)
    {
      allRooms = getAllRooms();
      if (_initialWorldState != null)
        XMLSerializer.Serialize<SaveState>(_initialWorldState, _rootPath + "SaveState.lvl");
    }
    bigVariablesUpdateTimeout += Time.deltaTime*1000;
    if (bigVariablesUpdateTimeout >= TIME_TO_UPDATE_BIG_VARIABLES)
    {
      bigVariablesUpdateTimeout = 0.0f;
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

    //_fullWindowScrollView = EditorGUILayout.BeginScrollView(_fullWindowScrollView, GUILayout.Width(position.width));

    GameStartState();
    EditorGUILayout.Separator();
    LoadRoomsGUI();
    EditorGUILayout.Separator();
    RoomActionsGUI();
    GatewaysInRoomAction();

    //EditorGUILayout.EndScrollView();
  }

  #region Option Pannels
  void LoadRoomsGUI()
  {
    GUILayout.Label("Room Loader", EditorStyles.boldLabel);
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
    GUILayout.Label("Loaded Room Actions", EditorStyles.boldLabel);

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
        if (castObject.activeInHierarchy && castObject.transform.parent == null && BPUtil.GetComponentsInHierarchy<GatewayTriggerScript>(castObject.transform).Count > 0)
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
          GatewayTriggerScript gateTrigger = BPUtil.GetComponentsInHierarchy<GatewayTriggerScript>(gate.transform)[0];
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

          if (!isConnected)
          {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.red;
            GUILayout.Label("   Invalid Room Name!", style);
          }

        }
      }
      EditorGUILayout.EndScrollView();
    }
  }

  void GameStartState()
  {
    GUILayout.Label("Game Start State", EditorStyles.boldLabel);
    gameStateFoldoutStatus = EditorGUILayout.Foldout(gameStateFoldoutStatus, "Game Start Params");
    if (gameStateFoldoutStatus)
    {
      if (_initialWorldState == null)
      {
        GUILayout.Label("Loading data from savestate...");
      }
      else
      {
        GUILayout.Label("Starting room ");
        GUILayout.BeginHorizontal();
        _initialWorldState.activeRoom = EditorGUILayout.TextArea(_initialWorldState.activeRoom, GUILayout.Width(position.width / 2));

        bool isConnected = false;
        foreach (string roomName in allRooms.Keys)
        {
          string simpleRoomName = roomName.Substring(roomName.IndexOf(":") + 2);
          isConnected = isConnected || _initialWorldState.activeRoom.Equals(simpleRoomName);
        }

        if (!isConnected)
        {
          GUIStyle style = new GUIStyle();
          style.normal.textColor = Color.red;
          GUILayout.Label("Invalid Room Name!", style);
        }
        GUILayout.EndHorizontal();

        GUILayout.Label("Player Start Position");
        GUILayout.BeginHorizontal();
        float xPos = EditorGUILayout.FloatField(_initialWorldState.playerPosition.x, GUILayout.Width(position.width * 0.9f / 3));
        float yPos = EditorGUILayout.FloatField(_initialWorldState.playerPosition.y, GUILayout.Width(position.width * 0.9f / 3));
        float zPos = EditorGUILayout.FloatField(_initialWorldState.playerPosition.z, GUILayout.Width(position.width * 0.9f / 3));
        _initialWorldState.playerPosition = new Vector3(xPos, yPos, zPos);
        GUILayout.EndHorizontal();


        GUILayout.Label("Player Start Rotation");
        GUILayout.BeginHorizontal();
        float xRot = EditorGUILayout.FloatField(_initialWorldState.playerRotation.x, GUILayout.Width(position.width * 0.9f / 3));
        float yRot = EditorGUILayout.FloatField(_initialWorldState.playerRotation.y, GUILayout.Width(position.width * 0.9f / 3));
        float zRot = EditorGUILayout.FloatField(_initialWorldState.playerRotation.z, GUILayout.Width(position.width * 0.9f / 3));
        _initialWorldState.playerRotation = new Vector3(xRot, yRot, zRot);
        GUILayout.EndHorizontal();
      }
    }
  }

#endregion
  #region Actuators
  bool SerializeCurrentRoom()
  {
    RoomDefinitionCreator roomDefMaker = new RoomDefinitionCreator();
    roomDefMaker._roomName = currentlyLoadedRoom.Substring(currentlyLoadedRoom.IndexOf(":") + 2);
    roomDefMaker.SerializeRoom();
    Debug.Log("Room " + roomDefMaker._roomName + " Successefully serialized");
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
  #endregion
}