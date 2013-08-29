using UnityEngine;
using System.Collections;

public class BipolarExceptionUnexpectedMissingCircuit : BipolarException 
{
  public BipolarExceptionUnexpectedMissingCircuit(string message) : base(message) { }
}
