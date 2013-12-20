using UnityEngine;
using System.Collections;

public class BipolarExceptionUnassignedName : BipolarException
{

  public BipolarExceptionUnassignedName(string message) : base("Unassigned Name "+message) { }
}
