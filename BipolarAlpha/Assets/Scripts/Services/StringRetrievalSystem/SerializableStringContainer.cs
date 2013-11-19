// Made by: Ivo Capelo
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// SerializableStringContainer contains all game strings and keys as a List so that it can be serialized/deserialized to xml
/// When being deserialized it can be return a Dictionary (string, string)
/// </summary>
/// 
public class SerializableStringContainer
{
  #region Private Variables
  //Abstraction of a Dictionary containing<string,string> for deserialization
  private List<SerializableStringToken> _stringAssets = new List<SerializableStringToken>();

  /// <summary>
  /// List with all string assets
  /// </summary>
  public List<SerializableStringToken> stringAssets
  {
    get
    {
      return _stringAssets;
    }
    set
    {
      _stringAssets = value;
    }
  }

  #endregion

  #region Constructors
  // Needed for Serialization to work
  public SerializableStringContainer() {}

  public SerializableStringContainer(List<SerializableStringToken> assets) 
  { 
    _stringAssets = assets; 
  }
  #endregion

  #region class converting methods
  /// <summary>
  /// Returns a Dictionary containing stored parings of key and string assets
  /// </summary>
  public Dictionary<string, string> toDictionary()
  {
    Dictionary<string, string> dict = new Dictionary<string, string>();

    foreach (SerializableStringToken token in _stringAssets)
    {
      //  BipolarConsole.AllLog("Dict["+token.key+"] = " +token.stringAsset); // For Debugging Strings input
      dict[token.key] = token.stringAsset;
    }
    return dict;
  }
  #endregion
}
