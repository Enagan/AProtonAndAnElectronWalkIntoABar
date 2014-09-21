  
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public class SceneManagerWindow : EditorWindow
{
  const float SIZE_OF_ELEMENT_IN_LIST = 18.5f;
  const float TIME_TO_UPDATE_BIG_VARIABLES = 5.0f;

  string _filterRoomListString = "";
  string _currentlyLoadedRoom = "<noRoom>";
  string _rootPath = "";

  string _currentlyPressed = "";

  string _activeSceneBeforePlayWasPressed = null;

  Dictionary<string, string> _allRooms = new Dictionary<string,string>();
  Dictionary<string, bool> _roomsSelectedForLoadIntoGame = new Dictionary<string, bool>();

  float _bigVariablesUpdateTimeout = 0.0f;
  float _compilationSuccessLabelTimeOut = 0;

  SaveState _initialWorldState = null;

  // Foldouts
  bool _loadRoomFoldoutStatus = false;
  bool _roomActionFoldoutStatus = false;
  bool _gatewaysFoldoutStatus = true;
  bool _gameStateFoldoutStatus = true;
  bool _roomsToLoadIntoGameFoldout = false;

  // Scroll Positions
  Vector2 _fullWindowScrollView;
  Vector2 _roomListScrollPosition;
  Vector2 _gatewaysScrollPosition;
  Vector2 _roomsToLoadIntoGameScrollPosition;

  // Popup Positions
  int _gameStateStartRoomPopup = 0;

  GameObject _instacedRoomDefCreator = null;

  // Add menu item named "My Window" to the Window menu
  [MenuItem("Window/Scene Manager Editor")]
  public static void ShowWindow()
  {
    //Show existing window instance. If one doesn't exist, make one.
    EditorWindow.GetWindow(typeof(SceneManagerWindow));
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

    if (_initialWorldState == null || _roomsSelectedForLoadIntoGame.Count == 0)
    {
      resetGameState();
    }


    if(_bigVariablesUpdateTimeout == 0)
    {
      _allRooms = getAllRooms();
    }
    _bigVariablesUpdateTimeout += Time.deltaTime*1000;
    if (_bigVariablesUpdateTimeout >= TIME_TO_UPDATE_BIG_VARIABLES)
    {
      _bigVariablesUpdateTimeout = 0.0f;
    }

    if(_compilationSuccessLabelTimeOut > 0)
    {
      _compilationSuccessLabelTimeOut -= Time.deltaTime * 1000;
    }
    else
    {
      _compilationSuccessLabelTimeOut = 0;
    }

    if (!EditorApplication.isPlayingOrWillChangePlaymode && _activeSceneBeforePlayWasPressed != null)
    {
      EditorApplication.OpenScene(_activeSceneBeforePlayWasPressed);
      _activeSceneBeforePlayWasPressed = null;
      if (_instacedRoomDefCreator)
        DestroyImmediate(_instacedRoomDefCreator);
    }

  }

  void OnGUI()
  {
    title = "Scene Manager Editor";
    GUILayout.Label("Scene Manager Editor", EditorStyles.boldLabel);

    //_fullWindowScrollView = EditorGUILayout.BeginScrollView(_fullWindowScrollView, GUILayout.Width(position.width));

    if (EditorApplication.isPlayingOrWillChangePlaymode)
      GUILayout.Label("Scene Manager Editor\n disabled while in play mode.\n\nClick here to refresh\n if you stopped play mode", EditorStyles.boldLabel);
    else
    {
        GameStartState();
        EditorGUILayout.Separator();
        LoadRoomsGUI();
        EditorGUILayout.Separator();
        RoomActionsGUI();
        GatewaysInRoomAction();
    }

    //EditorGUILayout.EndScrollView();
  }

  #region Option Pannels

  void GameStartState()
  {
    GUILayout.Label("Game Start State", EditorStyles.boldLabel);
    _gameStateFoldoutStatus = EditorGUILayout.Foldout(_gameStateFoldoutStatus, "Game Start Params");
    if (_gameStateFoldoutStatus)
    {
      if (_initialWorldState == null)
      {
        GUILayout.Label("Loading data from savestate...");
      }
      else
      {
        _roomsToLoadIntoGameFoldout = EditorGUILayout.Foldout(_roomsToLoadIntoGameFoldout, "Rooms to load into game");

        if (_roomsToLoadIntoGameFoldout)
        {
          float maxHeight = Mathf.Min(_roomsSelectedForLoadIntoGame.Count * SIZE_OF_ELEMENT_IN_LIST, 10 * SIZE_OF_ELEMENT_IN_LIST);
          _roomsToLoadIntoGameScrollPosition = EditorGUILayout.BeginScrollView(_roomsToLoadIntoGameScrollPosition, GUILayout.MaxHeight(maxHeight), GUILayout.Width(position.width));

          Dictionary<string, bool> tempDict = new Dictionary<string, bool>();
          foreach (KeyValuePair<string,bool> roomSelected in _roomsSelectedForLoadIntoGame)
          {
            bool isSelected = EditorGUILayout.ToggleLeft(roomSelected.Key, roomSelected.Value);
            tempDict.Add(roomSelected.Key, isSelected);
          }
          _roomsSelectedForLoadIntoGame = new Dictionary<string, bool>(tempDict);

          EditorGUILayout.EndScrollView();

        }

        GUILayout.Label("Starting room ");
        GUILayout.BeginHorizontal();

        List<string> presentableFoldoutStrings = new List<string>();
        List<string> loadableFoldoutStrings = new List<string>();
        foreach (string roomName in _roomsSelectedForLoadIntoGame.Keys)
        {
          if (_roomsSelectedForLoadIntoGame[roomName])
          {
            string simpleRoomName = roomName.Substring(roomName.IndexOf(":") + 2);
            loadableFoldoutStrings.Add(simpleRoomName);
            presentableFoldoutStrings.Add(roomName);
          }
        }

        int popUpPosition = loadableFoldoutStrings.IndexOf(_initialWorldState.activeRoom);
        if (popUpPosition > loadableFoldoutStrings.Count || popUpPosition == -1)
          popUpPosition = 0;

        popUpPosition = EditorGUILayout.Popup(popUpPosition, presentableFoldoutStrings.ToArray());

        if(loadableFoldoutStrings.Count > 0)
          _initialWorldState.activeRoom = loadableFoldoutStrings[popUpPosition];

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

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save Changes"))
        {
          if (_initialWorldState != null)
            saveGameState();
        }
        if (GUILayout.Button("Revert Changes"))
        {
          resetGameState();
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Play Bipolar"))
        {
          PlayGame();
        }
      }
    }
  }

  void LoadRoomsGUI()
  {
    GUILayout.Label("Room Loader", EditorStyles.boldLabel);
    _loadRoomFoldoutStatus = EditorGUILayout.Foldout(_loadRoomFoldoutStatus, "Load Rooms");

    if (_loadRoomFoldoutStatus)
    {
      _filterRoomListString = EditorGUILayout.TextField("Filter Rooms", _filterRoomListString);

      List<string> filteredRoomList = new List<string>();
      foreach (KeyValuePair<string, string> room in _allRooms)
      {
        string roomName = room.Key;
        if (roomName.Contains(_filterRoomListString) || _filterRoomListString.Equals(""))
        {
          filteredRoomList.Add(roomName);
        }
      }

      float maxHeight = Mathf.Min(filteredRoomList.Count * SIZE_OF_ELEMENT_IN_LIST, 300);


      int popUpPosition = filteredRoomList.IndexOf(_currentlyPressed);
      if (popUpPosition > filteredRoomList.Count || popUpPosition == -1)
        popUpPosition = 0;

      popUpPosition = EditorGUILayout.Popup(popUpPosition, filteredRoomList.ToArray());

      if (filteredRoomList.Count > 0)
        _currentlyPressed = filteredRoomList[popUpPosition];

      //_roomListScrollPosition = EditorGUILayout.BeginScrollView(_roomListScrollPosition, GUILayout.MaxHeight(maxHeight), GUILayout.Width(position.width));

      //foreach (string roomName in filteredRoomList)
      //{
      //  bool isPressed = EditorGUILayout.ToggleLeft(roomName, _currentlyPressed.Equals(roomName));
      //  if (isPressed)
      //    _currentlyPressed = roomName;
      //}

      //EditorGUILayout.EndScrollView();

      EditorGUILayout.BeginHorizontal();

      if (GUILayout.Button("Load Room", GUILayout.Width(Mathf.Max((position.width - 10) / 2, 100))))
      {
        bool success = EditorApplication.OpenScene(_allRooms[_currentlyPressed]);
        if (success)
          _currentlyLoadedRoom = _currentlyPressed;
      }
      if (GUILayout.Button("Refresh Rooms", GUILayout.Width(Mathf.Max((position.width - 10) / 2, 100))))
      {
        _allRooms = getAllRooms();
      }

      EditorGUILayout.EndHorizontal();
    }
  }

  void RoomActionsGUI()
  {
    GUILayout.Label("Loaded Room Actions", EditorStyles.boldLabel);

    if (_currentlyLoadedRoom.Equals("<No valid room is loaded>"))
      _roomActionFoldoutStatus = false;
    else
      _roomActionFoldoutStatus = true;
    EditorGUILayout.Foldout(_roomActionFoldoutStatus, "Loaded Room: " + _currentlyLoadedRoom);
    if(_roomActionFoldoutStatus)
    {
      if(_compilationSuccessLabelTimeOut>0)
        GUILayout.Label("Serialization Success!", EditorStyles.boldLabel);

      if (GUILayout.Button("Serialize Current Room", GUILayout.Width(Mathf.Max(position.width - 10, 100))))
        if (SerializeCurrentRoom())
        {
          _compilationSuccessLabelTimeOut = 10;
        }


    }

    //GUILayout.Label(, EditorStyles.boldLabel);
  }
  void GatewaysInRoomAction()
  {
    _gatewaysFoldoutStatus = EditorGUILayout.Foldout(_gatewaysFoldoutStatus, "Gateways in room:");
    if (_roomActionFoldoutStatus && _gatewaysFoldoutStatus)
    {
      List<GameObject> Gateways = new List<GameObject>();

      object[] allObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject));

      foreach (object thisObject in allObjects)
      {
        GameObject castObject = ((GameObject)thisObject);
        if (castObject.activeInHierarchy && 
          (castObject.transform.parent != null && castObject.transform.parent.name.Equals("ParentObject" + _currentlyLoadedRoom.Substring(_currentlyLoadedRoom.IndexOf(":") + 2))) && 
          BPUtil.GetComponentsInHierarchy<GatewayTriggerScript>(castObject.transform).Count > 0)
        {
          Gateways.Add(castObject);
        }
      }

      float maxHeight = Mathf.Min(Gateways.Count * 2 * SIZE_OF_ELEMENT_IN_LIST, 20 * SIZE_OF_ELEMENT_IN_LIST);
      _gatewaysScrollPosition = EditorGUILayout.BeginScrollView(_gatewaysScrollPosition, GUILayout.MaxHeight(maxHeight+20), GUILayout.Width(position.width));

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
          //gateTrigger.connectsTo = EditorGUILayout.TextArea(gateTrigger.connectsTo, GUILayout.Width(position.width / 2));
          bool isConnected = false;
          string gatewayConnectedTo = "";
          List<string> loadableFoldoutRoomNames = new List<string>();
          foreach (string roomName in _allRooms.Keys)
          {
            string simpleRoomName = roomName.Substring(roomName.IndexOf(":") + 2);
            loadableFoldoutRoomNames.Add(simpleRoomName);
            isConnected = isConnected || gateTrigger.connectsTo.Equals(simpleRoomName);
            if (gateTrigger.connectsTo.Equals(simpleRoomName))
              gatewayConnectedTo = roomName;
          }

          int popUpPosition = loadableFoldoutRoomNames.IndexOf(gateTrigger.connectsTo);
          if (popUpPosition > loadableFoldoutRoomNames.Count || popUpPosition == -1)
            popUpPosition = 0;

          popUpPosition = EditorGUILayout.Popup(popUpPosition, loadableFoldoutRoomNames.ToArray());

          string previousGatewayConnection = gateTrigger.connectsTo;
          if (loadableFoldoutRoomNames.Count > 0)
            gateTrigger.connectsTo = loadableFoldoutRoomNames[popUpPosition];
          if (!previousGatewayConnection.Equals(gateTrigger.connectsTo))
            EditorUtility.SetDirty(gateTrigger);

          if (GUILayout.Button("Goto") && isConnected)
          {
            bool success = EditorApplication.OpenScene(_allRooms[gatewayConnectedTo]);
            if (success)
              _currentlyLoadedRoom = _currentlyPressed;
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

#endregion
  #region Actuators
  bool SerializeCurrentRoom()
  {
    if (_currentlyLoadedRoom != "<noRoom>")
    {
      //EditorApplication.isPlaying = true;

      GameObject roomDefer = new GameObject();

      roomDefer.AddComponent<RoomDefinitionCreator>();
      RoomDefinitionCreator roomDefMaker = roomDefer.GetComponent<RoomDefinitionCreator>();

      //while (!EditorApplication.isPlayingOrWillChangePlaymode) { }

      //RoomDefinitionCreator roomDefMaker = new RoomDefinitionCreator();
      roomDefMaker._roomName = _currentlyLoadedRoom.Substring(_currentlyLoadedRoom.IndexOf(":") + 2);
      //roomDefMaker.SerializeRoom();
      _instacedRoomDefCreator = roomDefer;
      EditorApplication.isPlaying = true;
      return true;
    }
    return false;
  }

  void OnHierarchyChange()
  {
    if (_currentlyLoadedRoom.Equals("<No valid room is loaded>") || 
      (_allRooms.ContainsKey(_currentlyLoadedRoom) && !_allRooms[_currentlyLoadedRoom].Equals(EditorApplication.currentScene)))
    {
      if (_allRooms.ContainsValue(EditorApplication.currentScene))
      {
        foreach (string key in _allRooms.Keys)
        {
          if (_allRooms[key].Equals(EditorApplication.currentScene))
          {
            _currentlyPressed = key;
            _currentlyLoadedRoom = _currentlyPressed;
            break;
          }
        }
      }
      else
      {
        _currentlyLoadedRoom = "<No valid room is loaded>";
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

  void PlayGame()
  {
    if (_initialWorldState != null)
      saveGameState();

    string playingFromScene = EditorApplication.currentScene;

    bool success = EditorApplication.OpenScene("Assets/Scenes/Main/Main.unity");
    if (success)
    {
      EditorApplication.isPlaying = true;
      _activeSceneBeforePlayWasPressed = playingFromScene;
    }
  }

  void resetGameState()
  {
    _initialWorldState = XMLSerializer.Deserialize<SaveState>(_rootPath + "SaveState.lvl");
    _roomsSelectedForLoadIntoGame.Clear();
    foreach (string room in _allRooms.Keys)
    {
      string simpleRoomName = room.Substring(room.IndexOf(":") + 2);
      string resourceFolderPath = "Assets/Resources/Levels/" + simpleRoomName + ".lvl";
      _roomsSelectedForLoadIntoGame.Add(room, _initialWorldState.roomPaths.Contains(resourceFolderPath));
    }
  }

  void saveGameState()
  {
    _initialWorldState.roomPaths.Clear();
    foreach (KeyValuePair<string,bool> roomSelected in _roomsSelectedForLoadIntoGame)
    {
      if (roomSelected.Value)
      {
        string simpleRoomName = roomSelected.Key.Substring(roomSelected.Key.IndexOf(":") + 2);
        string resourceFolderPath = "Assets/Resources/Levels/" + simpleRoomName + ".lvl";
        _initialWorldState.roomPaths.Add(resourceFolderPath);
      }
    }
    XMLSerializer.Serialize<SaveState>(_initialWorldState, _rootPath + "SaveState.lvl");
  }
  #endregion
}

#endif