using UnityEngine;
using System.Collections;

public class AnimationTrigger : MonoBehaviour {

  [SerializeField]
  private GameObject _targetObject;

  private void Start()
  {
    if (!_targetObject)
    {
      throw new BipolarException("Trigger Has no target Animation");
    }
  }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Magnet")
        {
          _targetObject.GetComponent<Animation>().Play();
        }
    }
}
