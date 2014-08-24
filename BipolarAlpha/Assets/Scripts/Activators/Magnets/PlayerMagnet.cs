//Made By: Lousada
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//
// WARNING - DO NOT put this script on the magnetic layer 
//
public class PlayerMagnet : MagneticForce 
{
  // Boolean required to confirm if the magnet isn't sticking to another magnet, and can be used
  private Vector3 _magnetHitPoint = default(Vector3);
  private Vector3 _currentHitPoint = default(Vector3);
  private GameObject _magnet;

  //used when snapping
  private bool _snapingToMagnet = false;

  private bool _isAvailable = true;
  private int _raycastMask = ~( (1 << 8) | (1 << 13) | (1 << 15) | (1 << 17));  //Ignore objects in layer 8 (Magnetic Force) and 13 (Triggers) and 15 (Magnetic Blockers)

  [SerializeField]
  private Light _hittingMagnetLight = null;

  public bool isAvailable
  {
    get
    {
      return _isAvailable;
    }
    set
    {
      _isAvailable = value;
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

  public bool snapingToMagnet
  {
    get
    {
      return _snapingToMagnet;
    }
    set
    {
      _snapingToMagnet = value;
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


  public override void Start()
  {
    base.Start();

    Transform player = transform;

    //Find player object in parents
    while((player = player.parent))
    {
      if(player.gameObject.CompareTag("Player"))
      {
        break;
      }
    }

    if(player != null)
    {
      parentToAffect = player.gameObject;
    }
    else 
    {
      Debug.Log("PlayerMagnet is not child of Player, YOU ARE DOING IT WRONG");
    }
  }

  /// <summary>
  /// Fires a raycast that will make a magnetic object have influence over the player if one is hit
  /// Requiers the direction the player/camera is facing 
  /// </summary>
  /// <returns> The Magnetic Force of the object that was hit </returns>
  public MagneticForce FireRayCast(Vector3 start, Vector3 direction) 
  {
    RaycastHit hit;
    if (Physics.Raycast(start, direction, out hit, Mathf.Infinity, _raycastMask) && hit.collider.CompareTag("Magnet"))  
    {

      Transform otherTransform = hit.collider.gameObject.transform;
      MagneticForce otherMagneticForce = (MagneticForce) otherTransform.FindChild("Magnetism").GetComponent("MagneticForce");
      _currentHitPoint = hit.point;
      _magnet = hit.transform.gameObject;
      if (!IsAlreadyAffectedBy(otherMagneticForce) )
      {
        _magnetHitPoint = hit.point;
        otherMagneticForce.AffectedBy(this);
        this.AffectedBy(otherMagneticForce);       
      }
      return otherMagneticForce; // leave handling for the ability
    }
    else    {
      _magnetHitPoint = Vector3.zero;
      _currentHitPoint = Vector3.zero;
      base.NoLongerAffectingMagnets();
    }

    if (snapingToMagnet)
    {
        return this;
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
        base.ApplyForces(magnetBody, otherMagnet, _magnetHitPoint);
      }
    }
  }

  public void EnableMagnetHitHaloLight()
  {
    if (_hittingMagnetLight != null)
    {
      _hittingMagnetLight.enabled = true;
    }
  }

  public void DisableMagnetHitHaloLight()
  {
    if (_hittingMagnetLight != null)
    {
      _hittingMagnetLight.enabled = false;
    }
  }

}
