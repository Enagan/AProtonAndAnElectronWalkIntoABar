//Made by: Lousada
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// The MagneticForce script is used to allow objects to interact with one another on a magnetic level
/// each object using this script must be on the magnetic layer and have a trigger collider
/// </summary>
public class MagneticForce : MonoBehaviour, Activator
{
  #region MagneticForce Constants
  private const float DUMMY_DISTANCE = 2.0f;
  private const float DUMMY_FORCE = 100.0f;

  private static float DISTANT_FORCE_CUTOFF = 0.5f;

  //ToDo - Needs fine tuning // Change these 3 variables to const
  private static float LOW_FORCE_FACTOR = 100.0f;
  private static float MEDIUM_FORCE_FACTOR = 500.0f;
  private static float HIGH_FORCE_FACTOR = 1000.0f;

  #endregion
	
  #region MagneticForce Variables
  [SerializeField]
  private bool _isActivated = true;

  [SerializeField]
  private bool _isMoveable = false;

  [SerializeField]
  private bool _isHoldable = false;
	
  public enum Force { LOW, MEDIUM, HIGH };
  public enum Charge { NEGATIVE, POSITIVE};
  [SerializeField]
  private Force _force = Force.MEDIUM;
  [SerializeField]
  private Charge _charge = Charge.NEGATIVE;

  [SerializeField]
  private List<Light> _magnetLights = new List<Light>();

  private List<MagneticForce> _affectingMagnets = new List<MagneticForce>();
  #endregion
	
  #region MagneticForce Properties
	
  public Force force
  {
    get { return _force; }
  }
	
  public Charge charge
  {
    get { return _charge; }
  }

  public bool isActivated
  {
    get { return _isActivated; }
    set 
    { 
	  if(_isActivated)
	    NoLongerAffectingMagnets();    
	  _isActivated = value;
	}
		
  }

  public bool isMoveable
  {
    get
    {
      return _isMoveable;
    }
    set
    {
      _isMoveable = value;
    }
  }

  public bool isHoldable
  {
    get
    {
      return _isHoldable;
    }
    set
    {
      _isHoldable = value;
    }
  }

  public List<MagneticForce> affectingMagnets
  {
    get { return _affectingMagnets; }
  }

  public float low_force_factor 
  {
    get { return LOW_FORCE_FACTOR; }
  }

  public float medium_force_factor 
  {
    get { return MEDIUM_FORCE_FACTOR; }
  }

  public float high_force_factor
  {
    get { return HIGH_FORCE_FACTOR; }
  }
  #endregion


  void Activator.Activate()
  {
    _isActivated = true;

    TurnOnLights();
  }

  void Activator.Deactivate()
  {
    if(_isActivated)
			NoLongerAffectingMagnets();
	_isActivated = false;

  TurnOffLights();
  }

  private void TurnOnLights()
  {
    foreach (Light light in _magnetLights)
    {
      light.enabled = true;
    }
  }

  private void TurnOffLights()
  {
    foreach (Light light in _magnetLights)
    {
      light.enabled = false;
    }
  }

  /// <summary>
  /// The AffectedBy function makes the MagneticForce provided begin to influence this object
  /// </summary>
  public void AffectedBy(MagneticForce otherMagnet)
  {
    _affectingMagnets.Add(otherMagnet);
  }

  /// <summary>
  /// Stops the influence of the MagneticForce provided
  /// </summary>
  public void NoLongerAffectedBy(MagneticForce otherMagnet) 
  {
    foreach (MagneticForce m in _affectingMagnets) {
      float aux = Vector3.Distance(otherMagnet.transform.position, m.transform.position);
      if (aux < 0.5f) {
        _affectingMagnets.Remove(m);
        break;
      }

    }
  }

  /// <summary>
  /// Checks if magnet is already being influenced by a certain MagneticForce
  /// </summary>
  public bool IsAlreadyAffectedBy(MagneticForce otherMagnet)
  {
    foreach (MagneticForce m in _affectingMagnets)
    {
      float aux = Vector3.Distance(otherMagnet.transform.position, m.transform.position);
      if (aux < 0.5f)
      {
        return true;
      }
    }
    return false;
  }

  /// <summary>
  /// Stops all MagneticForce influences to and from this magnet
  /// </summary>
  public void NoLongerAffectingMagnets ()
  {
	foreach (MagneticForce m in _affectingMagnets) {
	  m.NoLongerAffectedBy(this);
    }
	_affectingMagnets.Clear();	
  }

  public void OnDestroy()
  {
    
    NoLongerAffectingMagnets();
  }

  public void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.transform.parent != null && other.gameObject.transform.parent.tag == "Magnet")
    {
      MagneticForce otherMagnet = (MagneticForce)other.gameObject.GetComponent("MagneticForce");
      AffectedBy(otherMagnet);
    }
  }

  public void OnTriggerExit(Collider other) 
  {
    if (other.gameObject.transform.parent != null && other.gameObject.transform.parent.tag == "Magnet")
    {
      MagneticForce otherMagnet = (MagneticForce)other.gameObject.GetComponent("MagneticForce");
      NoLongerAffectedBy(otherMagnet);
    }
  }

  public virtual void Start()
  {
    SphereCollider collider = this.GetComponent<SphereCollider>();

    //If the magnetic force has a collider attached it is it's interaction area, and this should be scaled to it's strength
    if (collider != null)
    {
      collider.radius = Mathf.Sqrt(getForceValue(force) * HIGH_FORCE_FACTOR) / Mathf.Sqrt(DISTANT_FORCE_CUTOFF);
    }

    float g = 52f / 255f;
    float b = 174f / 255f;

    foreach(Light light in _magnetLights)
    {
      if(charge == Charge.NEGATIVE)
      {
        light.color = new Color(0, g, b);
      }
      else
      {
        light.color = Color.red;
      }
    }

    if (!_isActivated)
    {
      TurnOffLights();
    }
    else
    {
      TurnOnLights();
    }
  }

  public virtual void  Update()
  {
    ApplyOtherMagnetsForces(this.transform.parent.rigidbody);
  }

  /// <summary>
  /// Applies the influence other objects have over this one
  /// </summary>
  public virtual void ApplyForces(Rigidbody magnetBody, MagneticForce otherMagnet, Vector3 hit = default(Vector3))
  {
    Vector3 forceDirection = new Vector3();
    if(hit != Vector3.zero)
    {
      forceDirection = hit - this.transform.position;
    }
    else
    {
      forceDirection = otherMagnet.transform.position - this.transform.position;
    }
    forceDirection.Normalize();
    if (otherMagnet.charge == this.charge)
    {
      forceDirection = (-1) * forceDirection;
    }
    float totalForce = getTotalForce(otherMagnet);
    magnetBody.AddForce(totalForce * forceDirection * Time.deltaTime, ForceMode.Force);
  }


  /// <summary>
  /// Checks if magnets will influence each other
  /// </summary>
  public virtual void ApplyOtherMagnetsForces(Rigidbody magnetBody) 
  {
    bool magneticBlockerFound = false;
    if (!_isMoveable)
    {
      return;
    }
    foreach (MagneticForce otherMagnet in _affectingMagnets) {
      if (otherMagnet != null && otherMagnet.isActivated && !(otherMagnet is PlayerMagnet)) {
        Vector3 thisPosition = this.transform.position;
        Vector3 otherPosition = otherMagnet.transform.position;
        //This is used to see if there is any Magnetic Blocker between the two magnets
        RaycastHit[] hits = Physics.RaycastAll(thisPosition, otherPosition - thisPosition, Vector3.Distance(thisPosition,otherPosition));

        foreach(RaycastHit singleHit in hits){
          if (singleHit.collider.gameObject.tag == "MagneticBlocker")
          {
            magneticBlockerFound = true;
            break;
          }

        }
        if(magneticBlockerFound)
        {
          continue;
        }
      }
      ApplyForces(magnetBody, otherMagnet);
      
    }
  }
	
  
  /// <summary>
  /// Calculates the totalForce of two magnet's interactions given the otherMagneticForce
  /// </summary>
  /// <param name="otherMagneticForce">MagneticForce from another Magnet that is interacting with this one</param>
  /// <returns></returns>
	public float getTotalForce (MagneticForce otherMagneticForce)
	{
    float distance = Vector3.Distance(otherMagneticForce.transform.position, this.transform.position);
    float forceFactor = 0.0f;
    float otherForceFactor = 0.0f;
    float totalForce = 0.0f;

    if (distance < DUMMY_DISTANCE) 
	  {   //This is used to prevent object with different forces to push each other after colliding
      forceFactor = DUMMY_FORCE;
    }
    else 
    {
      forceFactor = getForceValue(force);
    }  


    if (distance < DUMMY_DISTANCE) 
	  {   //This is used to prevent object with different forces to push each other after colliding
      otherForceFactor = DUMMY_FORCE;
    }
    else 
    {
      otherForceFactor = getForceValue(otherMagneticForce.force);
    }

    totalForce = ((forceFactor * otherForceFactor) / (distance*distance));
	  return totalForce;
  }

  /// <summary>
  /// Turns a enum force value into it's actual float value
  /// </summary>
  private float getForceValue(Force force)
  {
    float result = DUMMY_FORCE;
    switch (force)
    {
      case Force.LOW:
        result = LOW_FORCE_FACTOR;
        break;
      case Force.MEDIUM:
        result = MEDIUM_FORCE_FACTOR;
        break;
      case Force.HIGH:
        result = HIGH_FORCE_FACTOR;
        break;
      default:
        //throw exception perhaps
        break;
    }
    return result;
  }
}
