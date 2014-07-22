//Made By: Lousada
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//
// WARNING - DO NOT put this script on the magnetic layer 
//
public class MagnetBoots : MagneticForce
{
  // Boolean required to confirm if the magnet isn't sticking to another magnet, and can be used
  private Vector3 _magnetHitPoint = default(Vector3);
  private Vector3 _currentHitPoint = default(Vector3);
  private GameObject _magnet;

  private bool _isActive = false;
  private int _raycastMask = ~((1 << 8) | (1 << 13) | (1 << 15));  //Ignore objects in layer 8 (Magnetic Force) and 13 (Triggers) and 15 (Magnetic Blockers)

  
  
  public bool isAvailable
  {
    get
    {
      return _isActive;
    }
    set
    {
      _isActive = value;
    }
  }

  public Vector3 currentHitPoint
  {
    get
    {
      return _currentHitPoint;
    }
    set
    {
      _currentHitPoint = value;
    }
  }
  

  public Vector3 magnetHitPoint
  {
    get
    {
      return _magnetHitPoint;
    }
    set
    {
      _magnetHitPoint = value;
    }
  }

  public GameObject hittedMagnet
  {
    get
    {
      return _magnet;
    }
    set
    {
      _magnet = value;
    }
  }

  public void Activate() {
    _isActive = true;
  }

  public void Deactivate()
  {
    NoLongerAffectingMagnets();
    _isActive = false;
    TurnOffLights();

  }
  public override void Start()
  {
    base.Start();

    Transform player = transform;

    //Find player object in parents
    while ((player = player.parent))
    {
      if (player.gameObject.CompareTag("Player"))
      {
        break;
      }
    }

    if (player != null)
    {
      parentToAffect = player.gameObject;
    }
    else
    {
      Debug.Log("MagnetBoots is not child of Player, YOU ARE DOING IT WRONG");
    }
  }

  /// <summary>
  /// Fires a raycast straight down that will make a magnetic object have influence over the player if one is hit
  /// </summary>
  /// <returns> The Magnetic Force of the object that was hit </returns>
  public MagneticForce FireRayCast(Vector3 start)
  {
    
    RaycastHit hit;
    if (Physics.Raycast(start, this.transform.up * -1.0f, out hit, Mathf.Infinity, _raycastMask) && hit.collider.CompareTag("Magnet"))
    {
      this.TurnOnLights();
      Transform otherTransform = hit.collider.gameObject.transform;
      MagneticForce otherMagneticForce = (MagneticForce)otherTransform.FindChild("Magnetism").GetComponent("MagneticForce");
      _currentHitPoint = hit.point;
      _magnet = hit.transform.gameObject;
      
        this.charge = otherMagneticForce.charge;
        _magnetHitPoint = hit.point;
        NoLongerAffectingMagnets();
        otherMagneticForce.AffectedBy(this);
        this.AffectedBy(otherMagneticForce);
      

      InitLights();

      return this; // leave handling for the ability
    }
    else
    {
      _magnetHitPoint = Vector3.zero;
      _currentHitPoint = Vector3.zero;
      base.NoLongerAffectingMagnets();
      TurnOffLights();
    }

    return null; //Maybe change this
  }

  /// <summary>
  /// Checks if magnets will influence each other
  /// </summary>
  public override void ApplyOtherMagnetsForces(Rigidbody magnetBody)
  {
   
    foreach (MagneticForce otherMagnet in base.affectingMagnets)
    {
      if (base.isActivated && otherMagnet.isActivated && !otherMagnet.isMoveable)
      {
        base.ApplyForces(magnetBody, otherMagnet, Vector3.Scale(_magnetHitPoint, Vector3.up) + this.transform.position);

      }
    }
  }
}
