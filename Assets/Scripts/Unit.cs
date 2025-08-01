using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private MeshRenderer _mR;
    [SerializeField] private float _unitSpeed = 4f;
    private Zone _destinationZone;
    private Vector3 _direction;
    private bool _unitMoving;
    public int _tribeIndex { get; private set; }

    public void SetTribe(int tribeIndex)
    {
        _tribeIndex = tribeIndex;
        _mR.material.color = GameManager.Singleton.GetTribeColor(tribeIndex);
    }

    public void SetUnitDestination(Zone zoneScr)
    {
        _unitMoving = true;
        _destinationZone = zoneScr;
        _direction = zoneScr.transform.position - transform.position;
        _direction.Normalize();
    }

    void Update()
    {
        if(_unitMoving)
            MoveTowardsDestination();
    }

    private void MoveTowardsDestination()
    {
        transform.Translate(_direction * _unitSpeed * Time.deltaTime);
        if (Vector3.Distance(_destinationZone.transform.position, transform.position) < 0.05f)
        {
            _unitMoving = false;
            transform.position = _destinationZone.transform.position;
            _destinationZone.UnitEnterZone(this);
        }
    }
}
