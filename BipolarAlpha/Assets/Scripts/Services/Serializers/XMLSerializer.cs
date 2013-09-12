//Made By: Engana
using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.IO;
using System;
using System.Security.AccessControl;

#pragma warning disable 0168

/// <summary>
/// Serializes and deserializes any class instance to an XML files
/// </summary>
public class XMLSerializer
{
  /// <summary>
  /// Writes the template class instance to an XML file
  /// </summary>
  public static bool Serialize<T>(T value, string filename)
  {
    if (value == null)
    {
      return false;
    }
    try
    {
      XmlSerializer _xmlserializer = new XmlSerializer(typeof(T));
      if (File.Exists(filename))
      {
        File.Delete(filename);
      }
      else
      {
        BipolarConsole.EnganaLog(filename.Substring(0,filename.LastIndexOf("/")));
        Directory.CreateDirectory(filename.Substring(0,filename.LastIndexOf("/")));
      }
      Stream stream = new FileStream(filename, FileMode.Create);
      _xmlserializer.Serialize(stream, value);
      stream.Flush();
      stream.Close();
      return true;
    }
    catch (Exception ex)
    {
      Debug.Log(ex);
      return false;
    }
  }

  /// <summary>
  /// Reads the template class instance from an XML file
  /// </summary>
  public static T Deserialize<T>(string filename)
  {
    if (string.IsNullOrEmpty(filename))
    {
      return default(T);
    }
    try
    {
      XmlSerializer _xmlSerializer = new XmlSerializer(typeof(T));
      Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
      var result = (T)_xmlSerializer.Deserialize(stream);
      stream.Close();
      return result;
    }
    catch (Exception ex)
    {
      Debug.Log(ex.Message);
      return default(T);
    }
  }
}
