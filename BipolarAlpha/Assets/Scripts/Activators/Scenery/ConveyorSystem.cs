using UnityEngine;
using System.Collections;

public class ConveyorSystem : MonoBehaviour, Activator {
  
  [SerializeField]
  private bool _isMoving = false;

  [SerializeField]
  private int _numberOfUnits = 5;

  [SerializeField]
  private float _unitMovingSpeed = 1.0f;

  [SerializeField]
  private GameObject _unitPrefab;

  [SerializeField]
  private Vector3 _positionAdjustment = new Vector3(0.0f, 0.0f, 0.0f);
  [SerializeField]
  private Vector3 _scaleAdjustment = new Vector3(1.0f, 1.0f, 1.0f);

  [SerializeField]
  private Vector3 _rotationAdjustmentAxis = new Vector3(0.0f, 0.0f, 0.0f);
  [SerializeField]
  private float _rotationAdjustmentAngle = 0.0f;

  private Vector3 _startPos;
  private Vector3 _endPos;
  private Vector3 _movementDirection;

  private GameObject[] _unitStorage;

	void Start () {
    _unitStorage = new GameObject[_numberOfUnits];

    _startPos = this.transform.FindChild("Starting Point").transform.position;
    _endPos = this.transform.FindChild("Ending Point").transform.position;
    _movementDirection = (_endPos - _startPos).normalized;

    float unitSpacing = Vector3.Distance(_startPos, _endPos) / _numberOfUnits;

    for (int i = 0; i < _numberOfUnits; i++)
    {
      _unitStorage[i] = (GameObject)Instantiate(_unitPrefab, _startPos +  _movementDirection* unitSpacing * i + _positionAdjustment, new Quaternion());
      _unitStorage[i].transform.localScale += _scaleAdjustment;
      _unitStorage[i].transform.Rotate(_rotationAdjustmentAxis, _rotationAdjustmentAngle);
    }

	}

  void Update()
  {
    if (_isMoving)
    {
      foreach (GameObject unit in _unitStorage)
      {
        unit.transform.position += _unitMovingSpeed * _movementDirection * Time.deltaTime;
      }
    }
  }

  public void CollisionWithEndPoint(GameObject other){
    foreach(GameObject unit in _unitStorage){
      if (unit == other)
      {
        unit.transform.position = _startPos + _positionAdjustment;
      }
    }
  }


  public void Activate()
  {
    _isMoving = true;
  }


  public void Deactivate()
  {
    _isMoving = false;
  }
}
