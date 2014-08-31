using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JackedInPlayer : MonoBehaviour {

  [SerializeField]
  private float _mouseHorizontalSensitivity = 0f;
  [SerializeField]
  private float _mouseVerticalSensitivity = 0f;

  private Quaternion _baseRotation = Quaternion.identity;

  [SerializeField]
  private float _maximumVerticalRotation = 0f;

  [SerializeField]
  private float _movementSpeed = 5.0f;


  private GameObject jackedInCamera;

  private Console motherConsole;

  //This dictionary holds the players' currently usable abilities and their cooresponding activation keys
  private Dictionary<string, Ability> _usableAbilities = new Dictionary<string, Ability>();

  public Console MotherConsole
  {
    get
    {
      return motherConsole;
    }
    set
    {
      motherConsole = value;
      }
    }
  

	// Use this for initialization
	void Start () {
    Screen.lockCursor = true;
    Screen.showCursor = true;
    Screen.showCursor = false;

    if ((jackedInCamera = GameObject.Find("Jacked-In Camera")) == null)
      throw new BipolarExceptionComponentNotFound("Camera component not found");


    //Initial rotation saved, used to clamp min and max rotation
    _baseRotation = jackedInCamera.transform.localRotation;

    InstantiateAbilities();
	}
	
	// Update is called once per frame
	void Update () {
    //This is for centering the mouse on the editor after pressing escape
    Screen.showCursor = false;
    Screen.lockCursor = true;
    ManageRotation();
    ManageMovement();
    ManageAbilities();

    if (Input.GetKeyDown(KeyCode.Q))
    {
      motherConsole.DeleteSpawn();
    }
	}


  #region Managers

  private void ManageMovement()
  {
    //Check for the values in the Vertical and Horizontal Axis
    //When Using the keyboard, Vertical -> W & S, Horizontal -> A & D
    //Values range from 1 to -1
    float fowardMovement = Input.GetAxis("Vertical");
    float sideMovement = Input.GetAxis("Horizontal");

    this.transform.position += fowardMovement * _movementSpeed * jackedInCamera.transform.forward * Time.deltaTime;
    this.transform.position += sideMovement * _movementSpeed * jackedInCamera.transform.right * Time.deltaTime;

  }


  private void ManageRotation()
  {

    //Get input from mouse
    float mouseY = Input.GetAxis("Mouse X") * _mouseHorizontalSensitivity * Time.deltaTime;
    float mouseX = Input.GetAxis("Mouse Y") * _mouseVerticalSensitivity * Time.deltaTime;

    //Save rotation, in case the next rotate causes the camera to pass the _maxiumVerticalRotation
    Quaternion previousRotation = jackedInCamera.transform.localRotation;

    transform.Rotate(Vector3.up, mouseY);

    //Rotate camera on the x axis. We don't need to rotate it on the Y axis because it's a child of the player object
    jackedInCamera.transform.Rotate(Vector3.left, mouseX);
    if (Quaternion.Angle(jackedInCamera.transform.localRotation, _baseRotation) >= _maximumVerticalRotation)
    {
      jackedInCamera.transform.localRotation = previousRotation;
    }

  }


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
        ability.Value.Use(ability.Key);
      }

    }
  }

  #endregion

  /// <summary>
  /// Adds an ability to the player that can be activated by the specified key
  /// Will override other abilities that use the specified key
  /// </summary>
  public void AddAbility(string key, Ability ability)
  {
    _usableAbilities[key] = ability;
  }

  public void InstantiateAbilities()
  {
    Camera playerCamera = this.GetComponentInChildren<Camera>();

    _usableAbilities.Add("Fire1", new AbilityActivateActivatable(this,playerCamera));
    _usableAbilities.Add("Fire2", _usableAbilities["Fire1"]);
  }
}
