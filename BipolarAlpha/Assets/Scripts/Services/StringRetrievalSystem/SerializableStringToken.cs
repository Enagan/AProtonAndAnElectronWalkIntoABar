using UnityEngine;
using System.Collections;


// Data class that represents a string and its key
public class SerializableStringToken
{

  #region Private variables
  private string _key = "";
  private string _stringAsset = "";
	// Use this for initialization

  /// <summary>
  /// key for acessing string
  /// </summary>
  public string key
  {
    get
    {
      return _key;
    }
    set
    {
      _key = value;
    }
  }

  /// <summary>
  /// serializable string asset
  /// </summary>
  public string stringAsset
  {
    get
    {
      return _stringAsset;
    }
    set
    {
      _stringAsset = value;
    }
  }
  #endregion

  #region constructors
  // for Serialization purposes
  public SerializableStringToken(){}

  public SerializableStringToken(string key, string asset)
  {
    _key = key;
    _stringAsset = asset;

  }
  #endregion
}
