//Made By: Engana
using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.IO;
using System;
using System.Security.AccessControl;
using System.Reflection;
using System.Collections.Generic;
using SMSceneManagerSystem;

#pragma warning disable 0168

/// <summary>
/// Serializes and deserializes any class instance to an XML files
/// </summary>
public class XMLSerializer
{
  private static List<Type> _extraTypes = new List<Type>();


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
      if (_extraTypes.Count <= 0)
      {
        /// Fecthing subtypes of complex state to allow polymorphic serialization of complex state subclasses
        Assembly assembly = typeof(ComplexStateDefinition).Assembly;

        foreach (Type possibleType in assembly.GetTypes())
        {
          if (possibleType.BaseType == typeof(ComplexStateDefinition))
          {
            _extraTypes.Add(possibleType);
          }
        }
      }

      XmlSerializer _xmlserializer = new XmlSerializer(typeof(T),_extraTypes.ToArray());
      if (File.Exists(filename))
      {
        File.Delete(filename);
      }
      else
      {
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
        SMConsole.Log(""+ex, "XMLSerializer", SMLogType.ERROR);
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
      if (_extraTypes.Count <= 0)
      {
        /// Fecthing subtypes of complex state to allow polymorphic serialization of complex state subclasses
        Assembly assembly = typeof(ComplexStateDefinition).Assembly;

        foreach (Type possibleType in assembly.GetTypes())
        {
          if (possibleType.BaseType == typeof(ComplexStateDefinition))
          {
            _extraTypes.Add(possibleType);
          }
        }
      }
      XmlSerializer _xmlSerializer = new XmlSerializer(typeof(T), _extraTypes.ToArray());
      Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
      var result = (T)_xmlSerializer.Deserialize(stream);
      stream.Close();
      return result;
    }
    catch (Exception ex)
    {
        SMConsole.Log(ex.Message + " In file " + filename, "XMLSerializer", SMLogType.ERROR);
      return default(T);
    }
  }
}
