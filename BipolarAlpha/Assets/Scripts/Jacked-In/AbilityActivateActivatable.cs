using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilityActivateActivatable : MonoBehaviour, Ability
{
  private int _raycastMask = ((1 << 0) | (1 << 17)); //Only consider objects in layer 0 (Default) and 17 (Jacked-In)

  private Camera _jackedInCamera;
  #pragma warning disable 414
  private JackedInPlayer _jackedInPlayer;
  private GameObject _motherConsole;

  public AbilityActivateActivatable(JackedInPlayer jackedInPlayer,Camera cam, GameObject motherConsole)
  {
    _jackedInCamera = cam;
    _jackedInPlayer = jackedInPlayer;
    _motherConsole = motherConsole;
  }

  public void Use(string key)
  {
    FireRayCast(_jackedInCamera.transform.position, _jackedInCamera.transform.forward, key); 
  }

  public void KeyUp(string key)
  {

  }

  public void KeyDown(string key)
  {

  }

  public void FireRayCast(Vector3 start, Vector3 direction, string key)
  {
    RaycastHit hit;
    if (Physics.Raycast(start, direction, out hit, Mathf.Infinity, _raycastMask) && hit.collider.CompareTag("Activatable"))
    {
      Vector3 scaleConsole = _motherConsole.transform.localScale;
      GameObject boundaryGameObject = null;
      Vector3 boundaryConsoleScale = Vector3.zero;

      foreach(Transform t in _motherConsole.GetComponentInChildren<Transform>()){
        if(t.gameObject.name == "Boundary"){
              boundaryGameObject = t.gameObject;
              boundaryConsoleScale = t.localScale;
        }
      }

      if(boundaryGameObject == null){
        Debug.Log("Boundary missing, fix it!");
        return;
      }

      float maxX = boundaryGameObject.transform.position.x + scaleConsole.x * boundaryConsoleScale.x;
      float minX = boundaryGameObject.transform.position.x - scaleConsole.x * boundaryConsoleScale.x;

      float maxY = boundaryGameObject.transform.position.y + scaleConsole.y * boundaryConsoleScale.y;
      float minY = boundaryGameObject.transform.position.y - scaleConsole.y * boundaryConsoleScale.y;

      float maxZ = boundaryGameObject.transform.position.z + scaleConsole.z * boundaryConsoleScale.z;
      float minZ = boundaryGameObject.transform.position.z - scaleConsole.z * boundaryConsoleScale.z;
      
      //check if the hit point is inside the boundary
      if(!((maxX > hit.point.x) && (hit.point.x > minX) &&
           (maxY > hit.point.y) && (hit.point.y > minY) &&
           (maxZ > hit.point.z) && (hit.point.z > minZ))){
        return; //not inside
      }


      // Activates an Activatable object hit by the raycast
      Transform otherTransform = hit.collider.gameObject.transform;
      JackedInRemoteController activatable = otherTransform.GetComponent<JackedInRemoteController>();
      activatable.Hit(key);
    }
  }
}