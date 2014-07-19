using UnityEngine;
using System.Collections;

public class MoveWithPlayer : MonoBehaviour {

  enum PlaneAxis
  {
    NONE,
    X,
    XMINUS,
    Y,
    YMINUS,
    Z,
    ZMINUS,
  };

  [SerializeField]
  Transform _player;

  Material _mat;

  // Previous player position
  Vector3 _oriPos;

  // Constants for fixing position


  [SerializeField]
  PlaneAxis _UAxis;
  [SerializeField]
  PlaneAxis _VAxis;
  [SerializeField]
  PlaneAxis _UAxisPlayer;
  [SerializeField]
  PlaneAxis _VAxisPlayer;

  [SerializeField]
  PlaneAxis _normalAxis;

  [SerializeField]
  Vector2 _centerOffset;

  [SerializeField]
  float _innerRadius = 0.03f;
  [SerializeField]
  float _outerRadius = 0.06f;
  [SerializeField]
  float _scaleRange = 10.0f;


	// Use this for initialization
	void Start () {
    _mat = transform.renderer.material;
    _oriPos = _player.position;
    _mat.SetFloat("_innerRadius", _innerRadius);
    _mat.SetFloat("_outerRadius", _outerRadius);

    float newUOffset = -(getAxisPos(_UAxisPlayer, _player.position) - _centerOffset.x) / (getAxisScale(_UAxis, transform) * 10);
    _mat.SetFloat("_xOffset", newUOffset);
    float newVOffset = -(getAxisPos(_VAxisPlayer, _player.position) - _centerOffset.y) / (getAxisScale(_VAxis, transform) * 10);
    _mat.SetFloat("_yOffset", newVOffset);
	}
	
	// Update is called once per frame
  void Update()
  {

    bool change = false;

    // Check for changes

    if (!equalInAxis(_UAxis, _oriPos, _UAxisPlayer, _player.position))
    {
      change = true;
      float newUOffset = -(getAxisPos(_UAxisPlayer, _player.position) - _centerOffset.x) / (getAxisScale(_UAxis, transform) * 10);
      _mat.SetFloat("_xOffset", newUOffset);

    }

    if (!equalInAxis(_VAxis, _oriPos, _VAxisPlayer, _player.position))
    {

      change = true;

      float newVOffset = -(getAxisPos(_VAxisPlayer, _player.position) - _centerOffset.y) / (getAxisScale(_VAxis, transform) * 10);
      _mat.SetFloat("_yOffset", newVOffset);

    }

    if (change)
    {


      _oriPos = _player.position;

      // ascertain circle scale
      float point =getAxisPos(_normalAxis, transform.position);
      float dist = getAxisPos(_normalAxis,_oriPos) - point; // third angle

      float scale = 0.0f;
      if (dist < _scaleRange)
      {
        scale = 1 - (dist) / (_scaleRange);
      }

      _mat.SetFloat("_innerRadius", _innerRadius*scale);
      _mat.SetFloat("_outerRadius", _outerRadius*scale);


      //BipolarConsole.AllLog(dist + "-" + (point + _scaleRange) + "-" + _scaleRange + "-" + scale);
    }
  }

  float getAxisPos(PlaneAxis axis, Vector3 trans)
  {
    
    switch(axis)
    {
      case PlaneAxis.X:
        return trans.x;
      case PlaneAxis.XMINUS:
        return -trans.x;
      case PlaneAxis.Y:
        return trans.y;
      case PlaneAxis.YMINUS:
        return -trans.y;
      case PlaneAxis.Z:
       return trans.z;
      case PlaneAxis.ZMINUS:
       return -trans.z;
    }
    return 0;
  }

  float getAxisScale(PlaneAxis axis, Transform trans)
  {
    switch (axis)
    {
      case PlaneAxis.X:
      case PlaneAxis.XMINUS:
        return trans.localScale.x;
      case PlaneAxis.Y:
      case PlaneAxis.YMINUS:
        return trans.localScale.y;
      case PlaneAxis.Z:
      case PlaneAxis.ZMINUS:
        return trans.localScale.z;
    }
    return 0;
  }

  bool equalInAxis(PlaneAxis axis, Vector3 trans1,PlaneAxis axis2, Vector3 trans2)
  {
    float pos1 = getAxisPos(axis, trans1);
    float pos2 = getAxisPos(axis2, trans2);
    return pos1 == pos2;
  }

  Vector3 setAxisPos(PlaneAxis axis, Vector3 trans, float val)
  {
    switch (axis)
    {
      case PlaneAxis.X:
      case PlaneAxis.XMINUS:
        trans.x = val;
        break;
      case PlaneAxis.Y:
      case PlaneAxis.YMINUS:
        trans.y = val;
        break;
      case PlaneAxis.Z:
      case PlaneAxis.ZMINUS:
        trans.z = val;
        break;
    }
    return trans;
  }
}
