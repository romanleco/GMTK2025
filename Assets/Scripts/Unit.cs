using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private MeshRenderer _mR;
    [SerializeField] private float _unitSpeed = 4f;
    private Zone _destinationZone;
    private ZSettlement _zoneOfGeneration;
    private Zone _currentInZone;
    private bool _unitMoving;
    public int TribeIndex { get; private set; }

    public void SetTribe(int tribeIndex)
    {
        this.TribeIndex = tribeIndex;
        _mR.material.color = GameManager.Singleton.GetTribeColor(tribeIndex);
    }

    public void SetUnitDestination(Zone zoneScr)
    {
        _unitMoving = true;
        transform.parent = null;
        _destinationZone = zoneScr;

        if (_currentInZone != null) _currentInZone.RemoveUnitFromZone(this);
        _currentInZone = null;

        Vector3 dir = transform.position - zoneScr.transform.position;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    void Update()
    {
        if (_unitMoving)
            MoveTowardsDestination();
    }

    private void MoveTowardsDestination()
    {
        transform.position = Vector3.MoveTowards(transform.position, _destinationZone.transform.position, _unitSpeed * Time.deltaTime);
        if (Vector3.Distance(_destinationZone.transform.position, transform.position) < 0.05f)
        {
            _unitMoving = false;
            transform.position = _destinationZone.transform.position;
            _destinationZone.UnitEnterZone(this);
        }
    }

    public void Die()
    {
        _zoneOfGeneration.RemoveUnitFromUnitsGenerated(this);
        _currentInZone.RemoveUnitFromZone(this);
        GameManager.Singleton.GetPlayerScript().SubtractToResource(3, 1); //subtracts to units from tribe

        Destroy(this.gameObject);
    }

    public void SetZoneOfGeneration(ZSettlement zoneScr) => _zoneOfGeneration = zoneScr;
    public void SetCurrentInZone(Zone zoneScr) => _currentInZone = zoneScr;
}
