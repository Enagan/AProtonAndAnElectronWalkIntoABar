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
  #endregion
}
