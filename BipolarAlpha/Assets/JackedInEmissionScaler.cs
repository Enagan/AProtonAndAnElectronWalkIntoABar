using UnityEngine;
using System.Collections;

public class JackedInEmissionScaler : MonoBehaviour {

  [SerializeField]
  float _xScale = 1;

  [SerializeField]
  float _yScale = 1;

  float _currentXScale;
  float _currentYScale;

  Material _mat;

	// Use this for initialization
	void Start () {
    _mat = transform.renderer.material;
    _mat.SetFloat("_xScale",_xScale);
    _mat.SetFloat("_yScale",_yScale);

    _currentXScale = _xScale;
    _currentYScale = _yScale;
	}

  void Update()
  {
    if (_currentXScale != _xScale)
    {
      _mat.SetFloat("_xScale", _xScale);
      _currentXScale = _xScale;
    }

    if (_currentYScale != _yScale)
    {
      _mat.SetFloat("_yScale", _yScale);
      _currentYScale = _yScale;
    }
  }
	

}
