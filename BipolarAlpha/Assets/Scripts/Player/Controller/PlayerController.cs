using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour, IPlayerAbilityObtainListener, IPauseListener
{

  #region Player Settings Variables
  //Angle cutoff to check if player is colliding with the ground
  [SerializeField]
  private float _groundAngleThreshold = 0f;

  [SerializeField]
  private float _snapAngleThereshold = 0f;

  //Force applied when the player is moving the mouse from the magnet, valid values are [0,1]
  [SerializeField]
  private float _snapInstantForce = 0f;

  //Is snap active
  [SerializeField]
  private bool _snapActive = false;

  //Mouse movement force in which the snap breaks
  [SerializeField]
  private float _minMouseSnapMovement = 0.15f;

  //Force applied when the player stops moving the mouse
  [SerializeField]
  private float _snapContinuousForce = 0f;

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
  private Vector3 _rotation = Vector3.zero;
  private Vector3 _acumulatedRotation = Vector3.zero;
  private Quaternion _baseRotation = Quaternion.identity;

  private float _totalStartRotationInSnap = 0;
  private Vector2 _startRotationInSnap = Vector2.zero;
  private bool _currentlyAffectingMagnet = false;
  private float _snapTimer = 0;

  private bool _airborne = true;

  private bool _collidingWithWall = false;
  private ContactPoint _samplePointOfCollidingSurface;

  private bool _magnetColliding = false;
  private MagneticForce _magnetCollidingWith = null;

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
  public bool magnetColliding
  {
    get
    {
      return _magnetColliding;
    }
  }
  public MagneticForce magnetCollidingWith
  {
    get
    {
      return _magnetCollidingWith;
    }
  }

  #endregion

  #region Player Children Entities

  [SerializeField]
  private PlayerMagnet _leftMagnet;
  [SerializeField]
  private PlayerMagnet _rightMagnet;

  #endregion


  #region Unity MonoBehaviour Functions

  /// <summary>
  /// Initializes Variables
  /// </summary>
  private void Start()
  {
    _snapContinuousForce = Mathf.Clamp01(_snapContinuousForce);
    Screen.lockCursor = true;
    Screen.showCursor = true;
    Screen.showCursor = false;
    if ((mainCamera = GameObject.Find("Camera")) == null)
      throw new BipolarExceptionComponentNotFound("Camera component not found");

    //Initial rotation saved, used to clamp min and max rotation
    _baseRotation = mainCamera.transform.localRotation;

    _leftMagnet = GameObject.Find("Left Player Magnet").transform.FindChild("Left Magnetism").GetComponent<PlayerMagnet>();
    _rightMagnet = GameObject.Find("Right Player Magnet").transform.FindChild("Right Magnetism").GetComponent<PlayerMagnet>();

    //add initial abilities
    instantiateAbilities();
    ServiceLocator.GetEventHandlerSystem().RegisterPlayerAbilityObtainListener(this);
    ServiceLocator.GetEventHandlerSystem().RegisterPauseListener(this);
  }

  /// <summary>
  /// Calls the players' Subsystem Managers
  /// </summary>
  private void Update()
  {
    //This for centering the mouse for editor after pressing escape
    Screen.showCursor = false;
    Screen.lockCursor = true;

    ManageMovement();
    ManageRotation();
    ManageAbilities();
  }

  /// <summary>
  /// Updates the players state, checks for ground or wall collision,
  /// or if the player is not sticking to a magnet,
  /// and changes the cooresponding flags as well as variables that 
  /// support the current state
  /// </summary>
  private void OnCollisionStay(Collision collision)
  {
    ContactPoint[] contactPoints = collision.contacts;

    bool wallColliding = false;
    //Check how far from Vector.UP the colision normals are
    foreach (ContactPoint point in contactPoints)
    {
      //If the angle is greater than GROUND_ANGLE_THRESHOLD passes, we're colliding with a wall
      if (Vector3.Angle(point.normal, Vector3.up) > _groundAngleThreshold)
      {
        setCollidingWithWall(point);
        wallColliding = true;
        if (collision.gameObject.tag == "Magnet")
        {
          _magnetColliding = true;
          _magnetCollidingWith = collision.gameObject.GetComponentInChildren<MagneticForce>();
        }
      }
      //Otherwise, we're colliding with the floor, in which case, if we're airborne, we shouldn't be so anymore
      else if (airborne)
      {
        setGrounded();
      }
    }

    if (!wallColliding)
    {
      setWallCollisionFree();
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
    //If the player stops colliding, and is not sticking to a magnet, it means it went airborne
    //So we adjust the cooresponding state flags.
    setAirborne();
    setWallCollisionFree();

    if (collision.gameObject.tag == "Magnet")
    {
      _magnetColliding = false;
      _magnetCollidingWith = null;
    }
  }

  #endregion

  #region Players' Public Methods
  /// <summary>
  /// Adds an ability to the player that can be activated by the specified key
  /// Will override other abilities that use the specified key
  /// </summary>
  public void AddAbility(string key, Ability ability)
  {
    _usableAbilities[key] = ability;
  }

  #region Players' Private Methods

  /// <summary>
  /// Instanciates the usable abilities dictionary of the player.
  /// Intended to be called during Start() MonoBehaviour method
  /// Any starting ability must be added here
  /// </summary>
  private void instantiateAbilities()
  {
    Camera playerCamera = this.GetComponentInChildren<Camera>();

    _usableAbilities.Add("Jump", new AbilityJump());
    //_usableAbilities.Add("Fire1", new AbilityUseMagnet(_leftMagnet, playerCamera, this));
    //_usableAbilities.Add("Fire2", new AbilityUseMagnet(_rightMagnet, playerCamera, this));

    // To test sticky ability, comment the two above AbilityUseMagnet and uncomment the following ability adding

    _usableAbilities.Add("Fire1", new AbilityStickMagnet(_leftMagnet, playerCamera));
    _usableAbilities.Add("Fire2", new AbilityStickMagnet(_rightMagnet, playerCamera));

    _usableAbilities.Add("Release1", _usableAbilities["Fire1"]);
    _usableAbilities.Add("Release2", _usableAbilities["Fire2"]);

    // To test scan ability
    _usableAbilities.Add("Scan", new AbilityScan());
    //Pause Game
    _usableAbilities.Add("Menu", new AbilityPauseGame());
  }
  #endregion
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
    foreach (KeyValuePair<string, Ability> ability in _usableAbilities)
    {

      if (Input.GetButtonUp(ability.Key))
      {
        ability.Value.KeyUp(ability.Key);
        continue;
      }

      if (Input.GetButtonDown(ability.Key))
      {
        ability.Value.KeyDown(ability.Key);
        continue;
      }

      // Uses the Input.GetButton method to read keys currently pressed, be careful due to its continuous reading
      if (Input.GetButton(ability.Key))
      {
        ability.Value.Use(this, ability.Key);
      }
    }
  }

  /// <summary>
  /// Checks for player input and applies all logic regarding the translation of the player object
  /// Parts of unitys' physics simulation is turned off, and those components are added manually
  /// in the calculations of the players' movement
  /// </summary>
  private void ManageMovement()
  {
    //Check for the values in the Vertical and Horizontal Axis
    //When Using the keyboard, Vertical -> W & S, Horizontal -> A & D
    //Values range from 1 to -1
    float fowardMovement = Input.GetAxis("Vertical");
    float sideMovement = Input.GetAxis("Horizontal");

    //Create a local velocity vector 
    //(with Z+ being the direction the camera is facing)
    Vector3 desiredVelocity = new Vector3(sideMovement, 0, fowardMovement);
    //Transform the local vector into world coordinates
    desiredVelocity = rigidbody.transform.TransformDirection(desiredVelocity);

    //If player is not airborne, apply friction to the X and Z axis
    if (!airborne)
    {
      rigidbody.velocity = new Vector3(rigidbody.velocity.x * 1 / (_floorFriction + 1),
                                        rigidbody.velocity.y,
                                        rigidbody.velocity.z * 1 / (_floorFriction + 1));
    }

    //If player is colliding with a wall surface and if the desired
    //velocity vector points towards the wall, project the 
    //desired velocity vector into the surface of the plane 
    //the player is colliding with, thus causing a sliding motion
    //instead of being "stuck" on the wall
    if (collidingWithWall
      && Vector3.Angle(desiredVelocity, _samplePointOfCollidingSurface.normal) > 90f)
    {
      desiredVelocity = Vector3.Project(desiredVelocity, Vector3.Cross(_samplePointOfCollidingSurface.normal, Vector3.up).normalized);
      //Apply friction from dragging along the wall, on top of floor friction
      desiredVelocity *= 1 / (_wallFriction + 1);
    }

    //Applies the previously calculated desired velocity adjusted for deltaTime and player acceleration
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

    PlayerMagnet activePlayerMagnet = null;

    //Check if a magnet is active
    if (_leftMagnet.currentHitPoint != Vector3.zero)
    {
      activePlayerMagnet = _leftMagnet;
    }
    else if (_rightMagnet.currentHitPoint != Vector3.zero)  //player is hitting a magnet
    {
      activePlayerMagnet = _rightMagnet;
    }

    if (activePlayerMagnet)
    {
      //Snapping to magnet
      if (!_currentlyAffectingMagnet)
      {
        _acumulatedRotation = Vector2.zero;
        _currentlyAffectingMagnet = true;
        activePlayerMagnet.snapingToMagnet = true;
        _snapTimer = 0;
      }
    }
    else
    {
      _currentlyAffectingMagnet = false;
    }

    //Get input from mouse
    float mouseY = Input.GetAxis("Mouse X") * _mouseHorizontalSensitivity * Time.deltaTime;
    float mouseX = Input.GetAxis("Mouse Y") * _mouseVerticalSensitivity * Time.deltaTime;

    //Save rotation, in case the next rotate causes the camera to pass the _maxiumVerticalRotation
    Quaternion previousRotation = mainCamera.transform.localRotation;

    transform.Rotate(Vector3.up, mouseY);

    //Rotate camera on the x axis. We don't need to rotate it on the Y axis because it's a child of the player object
    mainCamera.transform.Rotate(Vector3.left, mouseX);
    print(Quaternion.Angle( _baseRotation, mainCamera.transform.localRotation));
    if (Quaternion.Angle(mainCamera.transform.localRotation, _baseRotation) >= _maximumVerticalRotation)
    {
      mainCamera.transform.localRotation = previousRotation;
    }

    //If snap is not active, don't do snap calculations
    if (!_snapActive)
    {
      return;
    }

    if (_currentlyAffectingMagnet)
    {
      Vector3 center = activePlayerMagnet.hittedMagnet.transform.position;
      Vector3 camForward = mainCamera.transform.forward;

      Vector3 dir = (center - mainCamera.transform.position).normalized;

      //Find angle between camera foward and vector directed to magnet center
      float angle = Vector3.Angle(camForward, dir);
      angle = Mathf.Abs(angle);

      //Rotating mouse with force, unsnap from magnet
      if (Mathf.Abs(Input.GetAxis("Mouse X")) + Mathf.Abs(Input.GetAxis("Mouse Y")) > _minMouseSnapMovement)
      {
        _currentlyAffectingMagnet = false;
        _snapTimer = 0;
        activePlayerMagnet.snapingToMagnet = false;
      }

      // Verify if angle is large enough to unsnap
      if (angle > _snapAngleThereshold)
      {
        activePlayerMagnet.snapingToMagnet = false;
        Vector3 currentRotation = Quaternion.LookRotation(mainCamera.transform.forward).eulerAngles;
      }
      else
      {
        //Snap the foward vector to the center of the affecting magnet
        _snapTimer += Time.deltaTime;
        Vector3 lerpedVector = Vector3.Lerp(camForward, dir, _snapTimer * _snapInstantForce);
        Vector3 eulerAngleLerped = Quaternion.LookRotation(lerpedVector).eulerAngles;
        transform.localEulerAngles = new Vector3(0, eulerAngleLerped.y, 0);
        mainCamera.transform.localEulerAngles = new Vector3(eulerAngleLerped.x, 0, 0);

      }
    }

  }

  /// <summary>
  /// Calculates the continuous force to apply to the mouse, using a linear function
  /// </summary>
  private float GetCounterActionForce(float angleDifference)
  {

    //angleDifference should always be positive
    if (angleDifference < 0)
    {
      return 0;
    }
    float force = -(1f / _snapAngleThereshold) * angleDifference + 1; // y = (-1/threshold)x + 1
    return Mathf.Clamp(force, 0, _snapContinuousForce);
  }

  #endregion

  #region Event Listeners
  public void ListenPlayerAbilityObtain(string newAbilityName)
  {
    if (newAbilityName == "Stick")
    {
      Camera playerCamera = this.GetComponentInChildren<Camera>();
      AddAbility("Fire1", new AbilityStickMagnet(_leftMagnet, playerCamera));
      AddAbility("Fire2", new AbilityStickMagnet(_rightMagnet, playerCamera));

      AddAbility("Release1", _usableAbilities["Fire1"]);
      AddAbility("Release2", _usableAbilities["Fire2"]);
      AddAbility("Scan", new AbilityScan());
      AddAbility("Menu", new AbilityPauseGame());
    }
  }

  public void ListenPause(bool isPaused)
  {
    if (isPaused)
      Screen.lockCursor = false;
    else
      Screen.lockCursor = true;
  }
  #endregion
}
