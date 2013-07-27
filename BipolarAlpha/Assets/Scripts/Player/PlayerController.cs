using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{

  #region Player Settings Variables
  //Angle cutoff to check if player is colliding with the ground
  [SerializeField]
  private float _groundAngleThreshold = 0f;

  [SerializeField]
  private float _acceleration = 0f;
  [SerializeField]
  private float _maxVelocity = 0f;

  [SerializeField]
  private float _floorFriction = 0f;
  [SerializeField]
  private float _wallFriction = 0f;

  [SerializeField]
  private float _mouseHorizontalSensitivity = 0f;
  [SerializeField]
  private float _mouseVerticalSensitivity = 0f;

  [SerializeField]
  private float _minimumVerticalRotation = 0f;
  [SerializeField]
  private float _maximumVerticalRotation = 0f;

  #endregion
  #region Player State Variables
  private float _rotationX = 0f;
  private float _rotationY = 0f;

  private bool _airborne = true;

  private bool _collidingWithWall = false;
  private ContactPoint _samplePointOfCollidingSurface;
  #endregion

  private GameObject mainCamera;
  //This dictionary holds the players' currently usable abilities and their cooresponding activation keys
  private Dictionary<string, Ability> _usableAbilities = new Dictionary<string, Ability>();

  #region Player Properties
  public bool airborne
  {
    get
    {
      return _airborne;
    }
  }
  public bool collidingWithWall
  {
    get
    {
      return _collidingWithWall;
    }
  }
  #endregion

  #region Unity MonoBehaviour Functions

  /// <summary>
  /// Initializes Variables
  /// </summary>
  private void Start () {
   _rotationX = -transform.localEulerAngles.x;
   Screen.lockCursor = true;
   if ((mainCamera = GameObject.Find("Camera")) == null)
     throw new BipolarExceptionComponentNotFound("Camera component not found");
   //TestAbility
   _usableAbilities.Add("Jump", new AbilityJump());
  }
	
  /// <summary>
  /// Calls the players' Subsystem Managers
  /// </summary>
  private void Update () {
    ManageMovement();
    ManageRotation();
    ManageAbilities();
  }

  /// <summary>
  /// Updates the players state, checks for ground or wall collision, 
  /// and changes the cooresponding flags as well as variables that 
  /// support the current state
  /// </summary>
  private void OnCollisionStay(Collision collision)
  {

    ContactPoint[] contactPoints = collision.contacts;
    //Check how far from Vector.UP the colision normals are
    foreach (ContactPoint point in contactPoints)
    {
      //If the angle is greater than GROUND_ANGLE_THRESHOLD passes, we're colliding with a wall
      if (Vector3.Angle(point.normal, Vector3.up) > _groundAngleThreshold)
      {
        setCollidingWithWall(point);
      }
      //Otherwise, we're colliding with the floor, in which case, if we're airborne, we shouldn't be so anymore
      else if (airborne)
      {
        setGrounded();
      }
    }
  }

  /// <summary>
  /// Updates the players state, sets his airborne flag to true and wall collision to false
  /// It's not necessary to check exactly which type of surface we've stop colliding with
  /// because if we're still in collision with one kind, the variable will be swiftly
  /// set again to it's proper value by OnCollisionStay
  /// </summary>
  private void OnCollisionExit(Collision collision)
  {
    //If the player stops colliding, it means it went airborne
    //So we adjust the cooresponding state flags.
    setAirborne();
    setWallCollisionFree();
  }

  #endregion

  #region Players' Public Methods
  /// <summary>
  /// Adds an ability to the player that can be activated by the specified key
  /// Will override other abilities that use the specified key
  /// </summary>
  public void AddAbility(string key, Ability ability)
  {
    _usableAbilities.Add(key, ability);
  }
  /// <summary>
  /// Removes the ability which can be activated with the specified key
  /// </summary>
  public void removeAbility(string key)
  {
    _usableAbilities.Remove(key);
  }
  #endregion

  #region Player State Modifiers

  /// <summary>
  /// Adjusts the player state to be colliding with the ground
  /// </summary>
  private void setGrounded()
  {
    _airborne = false;
    //We turn off gravity when we're grounded to prevent the player 
    //from sliding off not too steep slopes with a normal vector 
    //within our GROUND_ANGLE_THRESHOLD
    rigidbody.useGravity = false;
  }
  /// <summary>
  /// Adjusts the player state to be airborne
  /// </summary>
  private void setAirborne()
  {
    _airborne = true;
    //We turn gravity back on to allow the player to fall
    rigidbody.useGravity = true;
  }

  /// <summary>
  /// Signals the player that he is colliding with a wall and
  /// sets up a support variable with the collision point
  /// </summary>
  private void setCollidingWithWall(ContactPoint pointOnCollidingSurface)
  {
    _collidingWithWall = true;
    _samplePointOfCollidingSurface = pointOnCollidingSurface;
  }
  /// <summary>
  /// Signals that the player is no longer colliding with a wall
  /// </summary>
  private void setWallCollisionFree()
  {
    _collidingWithWall = false;
  }

  #endregion

  #region Player SubSystem Managers

  /// <summary>
  /// Checks if any of the players' currently usable abilities have been triggered
  /// and if so, uses the triggered ability
  /// </summary>
  private void ManageAbilities()
  {
    foreach(KeyValuePair<string,Ability> ability in _usableAbilities)
    {
      if (Input.GetButtonDown(ability.Key))
      {
        ability.Value.Use(this);
      }
    }
  }

  /// <summary>
  /// Checks for player input and applies all logic regarding the translation of the player object
  /// </summary>
  private void ManageMovement()
  {
    //Check which movement keys were pressed
    float fowardMovement = Input.GetAxis("Vertical");
    float sideMovement = Input.GetAxis("Horizontal");

    //Create velocity vector
    Vector3 desiredVelocity = new Vector3(sideMovement, 0, fowardMovement);
    desiredVelocity = rigidbody.transform.TransformDirection(desiredVelocity);

    //If player is not airborne, apply drag to the X and Z axis
    if (!airborne)
    {
      rigidbody.velocity = new Vector3(rigidbody.velocity.x * 1 / (_floorFriction + 1), 
                                        rigidbody.velocity.y, 
                                        rigidbody.velocity.z * 1 / (_floorFriction + 1));
    }

    //Apply Sliding if player is colliding with a wall surface and if the desired
    //velocity vector point towards the wall
    //If so, projects the desired velocity vector into the surface of the plane 
    //the player is collising with, thus causing a sliding effect
    if (collidingWithWall
      && Vector3.Angle(desiredVelocity, _samplePointOfCollidingSurface.normal) > 90f)
    {
      desiredVelocity = Vector3.Project(desiredVelocity, Vector3.Cross(_samplePointOfCollidingSurface.normal, Vector3.up).normalized);
      desiredVelocity *= 1/(_wallFriction+1);
    }

    //Applies calculated velocity adjusted for deltaTime and player acceleration
    desiredVelocity *= Time.deltaTime * _acceleration;
    rigidbody.AddForce(desiredVelocity, ForceMode.VelocityChange);

    //Prevent the Player from exceeding maxVelocity
    Vector3 currentVelocity = rigidbody.velocity;
    currentVelocity.y = 0;
    if (currentVelocity.sqrMagnitude > _maxVelocity * _maxVelocity)
    {
      currentVelocity = currentVelocity.normalized * _maxVelocity;
      currentVelocity.y = rigidbody.velocity.y;
      rigidbody.velocity = currentVelocity;
    }
  }

  /// <summary>
  /// Checks for player mouse input and applies all logic regarding the rotation of the player object
  /// </summary>
  private void ManageRotation()
  {
    _rotationY = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * _mouseHorizontalSensitivity * Time.deltaTime;

    _rotationX += Input.GetAxis("Mouse Y") * _mouseVerticalSensitivity *  Time.deltaTime;
    _rotationX = Mathf.Clamp(_rotationX, _minimumVerticalRotation, _maximumVerticalRotation);

    //hackish..I only rotate the player at y axis, so the collision box doesn't get affected by other rotations
    transform.localEulerAngles = new Vector3(0, _rotationY, 0);
    //Rotate camera on the x axis. We don't need to rotate it on the Y axis because it's a child of the player object
    mainCamera.transform.localEulerAngles = new Vector3(-_rotationX, 0, 0);
  }
  #endregion
}
