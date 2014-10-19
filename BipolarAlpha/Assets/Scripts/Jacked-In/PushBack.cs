using UnityEngine;
using System.Collections;

public class PushBack: MonoBehaviour{

  [SerializeField]
  private float _pushForce = 20.0f;

  private GameObject jackedInPlayer;

  public void OnTriggerExit(Collider col){
    if (col.gameObject.tag == "Player")
    {
      jackedInPlayer = col.gameObject; 
      Vector3 pushDirection = (this.transform.position - col.transform.position).normalized;
      col.gameObject.rigidbody.AddForce(pushDirection * _pushForce, ForceMode.Impulse);
      jackedInPlayer.GetComponent<JackedInPlayer>().PreventPlayerControl();
      Invoke("AllowMovement", 0.3f);
    }
  }

  //returns the a vector of magnitude 1 with the orientation of the largest compnent of the input vector
  private Vector3 largestComponent(Vector3 input){
    Vector3 absInput = new Vector3(Mathf.Abs(input.x), Mathf.Abs(input.y), Mathf.Abs(input.z));
    if (absInput.x > absInput.y && absInput.x > absInput.z)
    {
      input.y = 0.0f;
      input.z = 0.0f;
      goto End;
    }

    if (absInput.y > absInput.x && absInput.y > absInput.z)
    {
      input.x = 0.0f;
      input.z = 0.0f;
      goto End;
    }

    if (absInput.z > absInput.x && absInput.z > absInput.y)
    {
      input.y = 0.0f;
      input.x = 0.0f;
      goto End;
    }

End:
    return input.normalized;

  }

  private void AllowMovement()
  {
    jackedInPlayer.GetComponent<JackedInPlayer>().AllowPlayerControl();
  }

  
}
