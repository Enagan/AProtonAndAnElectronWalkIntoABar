using UnityEngine;
using System.Collections;

public class JackedInPlayer : MonoBehaviour {

  [SerializeField]
  private float _mouseHorizontalSensitivity = 0f;
  [SerializeField]
  private float _mouseVerticalSensitivity = 0f;

  private Quaternion _baseRotation = Quaternion.identity;

  [SerializeField]
  private float _maximumVerticalRotation = 0f;


  private GameObject jackedInCamera;


	// Use this for initialization
	void Start () {
    Screen.lockCursor = true;
    Screen.showCursor = true;
    Screen.showCursor = false;

    if ((jackedInCamera = GameObject.Find("Jacked-In Camera")) == null)
      throw new BipolarExceptionComponentNotFound("Camera component not found");


    //Initial rotation saved, used to clamp min and max rotation
    _baseRotation = jackedInCamera.transform.localRotation;
	}
	
	// Update is called once per frame
	void Update () {
    //This is for centering the mouse on the editor after pressing escape
    Screen.showCursor = false;
    Screen.lockCursor = true;
    ManageRotation();
	
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
}
