using UnityEngine;
using System.Collections;
using System;
public class HUDObjectComparator : IComparer {

  // Compares two HUDObjects's priorities
  // The more priority the "smaller" the HUDObject, this is done to appear first in the priority list when using Sort
  // < 0 if first less than second
  // 0 if both have equal priorities
  // > 0 if first greater than second
  int IComparer.Compare(System.Object first, System.Object second)
  {
    HUDObject x = (HUDObject)first;
    HUDObject y = (HUDObject)second;
    if (x != null && y != null)
      return -(x.getPriority() - y.getPriority()); // Value negated so Greater objects appear first.
    else return 0;
  }
}
