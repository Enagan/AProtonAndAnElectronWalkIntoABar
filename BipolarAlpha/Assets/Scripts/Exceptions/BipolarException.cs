//Made by: Engana
using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Super class of all custom made excpetions for bipolar
/// </summary>
public class BipolarException : UnityException {

  public BipolarException(string message) : base(message) { }

  public BipolarException(string message, Exception innerException): base(message,innerException) { }
	
}
