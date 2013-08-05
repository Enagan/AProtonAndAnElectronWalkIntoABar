using UnityEngine;
using System.Collections;

/// <summary>
/// Bipolar Dev ID is a utility class used by Bipolar Console to determine which developer is currently running the software.
/// Should never be versioned to the repository.
/// </summary>
public class BipolarDevID
{
  // Change these variables for your current run definitions
  private const Devs _iAm = Devs.LOUSADA;
  private const bool _seeAll = false;


  //-- Acessable properties
  //    iAm - Current developer
  //    seeAll - boolean that indicates if all debug messages are enabled
  #region
  /// <summary>
  /// Returns the developer currently running the program
  /// </summary>
  public static Devs iAm
  {
    get
    {
      return _iAm;
    }
  }

  /// <summary>
  /// Returns true in case the current developer wants to see all debug messages from all other developers
  /// </summary>
  public static bool seeAll
  {
    get
    {
      return _seeAll;
    }
  }
  #endregion
}
