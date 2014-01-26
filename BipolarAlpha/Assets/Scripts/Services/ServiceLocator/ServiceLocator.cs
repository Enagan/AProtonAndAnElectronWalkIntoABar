using UnityEngine;
using System.Collections;

/// <summary>
/// Service Locator provides a global point of access to services or systems.
/// Service Locator permits swapping or changing an implementation of a system with a different one, without having 
/// to change the classes that use that system, as long as the interface remains the same.
/// </summary>
public class ServiceLocator
{
  // Variable references to available services/systems
  private static BaseSystem _baseSys = new BaseSystem();

  private static SceneManager _sceneManager;

  private static ResourceSystem _resourceSystem;

  private static BipolarEventHandlerSystem _eventHandlerSystem = new BipolarEventHandlerSystem();

  private static AudioSystem _audioSystem;

  private static SaveSystem _saveSystem;
	
  private static HUDSystem _HUDSystem;

  private static StringRetrievalSystem _stringSystem = new StringRetrievalSystem();

  private static CheckPointSystem _checkpointSystem= new CheckPointSystem();

  private static CutsceneManager _cutsceneManager = new CutsceneManager();

  //--- Access functions
  //- Get<SystemInterfaceName>()
  //    Access functions to the available services/systems
  #region Access Functions
  /// <summary>
  /// Returns an implementation of BaseSystem
  /// </summary>
  public static BaseSystem GetBaseSystem()
  {
      return _baseSys;
  }
  /// <summary>
  /// Returns the current implementation of Scene Manager
  /// </summary>
  public static SceneManager GetSceneManager()
  {
    return _sceneManager;
  }
  /// <summary>
  /// Returns the current implementation of Resource System
  /// </summary>
  public static ResourceSystem GetResourceSystem()
  {
    return _resourceSystem;
  }

  /// <summary>
  /// Returns the current implementation of Event Handler System
  /// </summary>
  public static BipolarEventHandlerSystem GetEventHandlerSystem()
  {
    return _eventHandlerSystem;
  }

  /// <summary>
  /// Returns the current implementation of the Audio System
  /// </summary>
  public static AudioSystem GetAudioSystem()
  {
    return _audioSystem;
  }

  /// <summary>
  /// Returns the current implementation of the Save System
  /// </summary>
  public static SaveSystem GetSaveSystem()
  {
    return _saveSystem;
  }
	
	/// <summary>
  /// Returns the current implementation of the HUD System
  /// </summary>
  public static HUDSystem GetHUDSystem()
  {
    return _HUDSystem;
  }

  /// <summary>
  /// Returns the current implementation of the String Retrieval System
  /// </summary>
  public static StringRetrievalSystem GetStringRetrievalSystem()
  {
    return _stringSystem;
  }

  /// <summary>
  /// Returns the current implementation of the CheckPoint System
  /// </summary>
  public static CheckPointSystem GetCheckPointSystem()
  {
    return _checkpointSystem;
  }

  /// <summary>
  /// Returns the current implementation of the cutscene manager 
  /// </summary>
  public static CutsceneManager GetCutsceneManager()
  {
    return _cutsceneManager;
  }



  #endregion


  //--- Provide new system implementation
  //- Provide<SystemInterfaceName>(<SystemInterfaceName> <systemName>)
  //    Swaps the current implementation of a system
  #region Change Service Functions
  /// <summary>
  /// Changes the current implementation of BaseSystem to the specified/provided implementation
  /// </summary>
  public static void ProvideBaseSystem(BaseSystem baseSys)
  {
    _baseSys = baseSys;
  }
  /// <summary>
  /// Changes the current implementation of Scene Manager to the specified/provided implementation
  /// </summary>
  public static void ProvideSceneMananger(SceneManager sceneMananger)
  {
    _sceneManager = sceneMananger;
  }
  /// <summary>
  /// Changes the current implementation of Resource System to the specified/provided implementation
  /// </summary>
  public static void ProvideResourceSystem(ResourceSystem resourceSystem)
  {
    _resourceSystem = resourceSystem;
  }

  /// <summary>
  /// Changes the current implementation of Event Handler to the specified/provided implementation
  /// </summary>
  public static void ProvideEventHandlerSystem(BipolarEventHandlerSystem eventHandlerSystem)
  {
    _eventHandlerSystem = eventHandlerSystem;
  }

  /// <summary>
  /// Changes the current implementation of Event Handler to the specified/provided implementation
  /// </summary>
  public static void ProvideAudioSystem(AudioSystem audioSystem)
  {
    _audioSystem = audioSystem;
  }

  /// <summary>
  /// Changes the current implementation of Save System to the specified/provided implementation
  /// </summary>
  public static void ProvideSaveSystem(SaveSystem saveSystem)
  {
    _saveSystem = saveSystem;
  }
	
  /// <summary>
  /// Changes the current implementation of HUD System to the specified/provided implementation
  /// </summary>
  public static void ProvideHUDSystem(HUDSystem hudSystem)
  {
    _HUDSystem = hudSystem;
  }

  /// <summary>
  /// Changes the current implementation of String Retrieval System to the specified/provided implementation
  /// </summary>
  public static void ProvideStringRetrievalSystem(StringRetrievalSystem stringSystem)
  {
    _stringSystem = stringSystem;
  }

  /// <summary>
  /// Changes the current implementation of CheckPoint System to the specified/provided implementation
  /// </summary>
  public static void ProvideStringRetrievalSystem(CheckPointSystem checkpointSystem)
  {
    _checkpointSystem = checkpointSystem;
  }

  /// <summary>
  /// Changes the current implementation of the cutscene Manager to the specified/provided implementation
  /// </summary>
  public static void ProvideCutsceneManager(CutsceneManager cutsceneManager)
  {
    _cutsceneManager = cutsceneManager;
  }

  #endregion
}
