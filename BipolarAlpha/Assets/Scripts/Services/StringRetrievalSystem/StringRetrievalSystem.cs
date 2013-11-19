using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class StringRetrievalSystem : MonoBehaviour
{

  #region private variables 
  //path to file with string assets
  private string _stringPath;

  private static string _fileName = "strings.xml";

  private Dictionary<string,string> _stringAssets;

  /// <summary>
  /// Dictionary with all string assets and associated keys
  /// </summary>
  public Dictionary<string, string> stringAssets
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
  #region Monobehavior methods

  // Use this for initialization
	void Start () {

    if (Application.isEditor)
    {
      _stringPath = "Assets/Resources/GUI/Text/";
    }
    else
    {
      _stringPath = "BipolarAlpha_Data/GUI/Text/";
    }

    string fileLocation = _stringPath + _fileName;
    
    //createNewStringFile(fileLocation); //For strings.xml creation purposes

    SerializableStringContainer container = XMLSerializer.Deserialize<SerializableStringContainer>(fileLocation); //Deserialize File

    _stringAssets = container.toDictionary();
    ServiceLocator.ProvideStringRetrievalSystem(this);
	}
  #endregion

  #region String Retrieval access methods

  //checks if has key
  public bool hasStringWithKey(string key)
  {
    return _stringAssets.ContainsKey(key);
  }

  //returns text associated with key
  public string getStringWithKey(string key)
  {
    if (hasStringWithKey(key))
      return _stringAssets[key];
    else
      throw new BipolarExceptionStringNotFound("StringRetrievalService was unable to find string for key: "+key);
  }
  #endregion

  #region Auxiliary methods
  /// <summary>
  /// Creates a new String file with sample strings
  /// For editor launch only
  /// </summary>
  private void createNewStringFile(string fileLocation)
  {
    SerializableStringContainer container = new SerializableStringContainer();
    container.stringAssets.Add(new SerializableStringToken("key1","This is a sample text"));
    container.stringAssets.Add(new SerializableStringToken("key2", "Potato"));
    XMLSerializer.Serialize<SerializableStringContainer>(container,fileLocation);
  }

  #endregion
}
