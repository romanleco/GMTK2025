using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;

public class Zone : MonoBehaviour
{
    [SerializeField] protected ZoneSO _zoneInfo;
    [SerializeField] protected (int, int) _indexesInGrid;
    protected int _ownerTribeIndex;
    protected int _zoneLevel = 0;
    protected List<Unit> _unitsInZone = new List<Unit>();
    protected List<Unit> _unitsMovingTowardsZone = new List<Unit>();
    [SerializeField] private Transform _unitPatrollingPositionsHolder;
    [SerializeField] private Transform[] _unitPatrollingPositions;

    void OnEnable() => LoopTimerManager.OnLoopComplete += OnLoopCompletedAction;
    void OnDisable() => LoopTimerManager.OnLoopComplete -= OnLoopCompletedAction;

    protected virtual void Start()
    {
        int i = 0;
        foreach (Transform t in _unitPatrollingPositionsHolder)
        {
            _unitPatrollingPositions[i] = t.GetChild(0);
            i++;
        }
    }

    void Update()
    {
        if (_unitsInZone.Count > 0)
            _unitPatrollingPositionsHolder.Rotate(Vector3.up * 16 * Time.deltaTime, Space.Self);
    }

    protected virtual void OnLoopCompletedAction()
    {
        if (_unitsInZone.Count > 0)
        {
            int newTribeIndex = _unitsInZone[0].TribeIndex;
            if (newTribeIndex != _ownerTribeIndex)
            {
                _ownerTribeIndex = newTribeIndex;
            }
        }
        //If zone conquered
        //give resource to the corresponding tribe depending on level (by _zoneInfo)
    }

    public virtual void Select() //What happens when the zone gets selected
    {
        Debug.Log($"Selected | T: {Time.time}");
        UIManager.Singleton.DisplayZoneBaseInfo(_zoneInfo, _unitsInZone.Count, _zoneLevel, _ownerTribeIndex, this);
        ExtraUIInfoUpdate();
    }

    public void SelectUnit(int unitIndex)
    {
        GameManager.Singleton.GetPlayerScript().SelectUnit(_unitsInZone[unitIndex]);
    }

    public virtual void ExtraUIInfoUpdate() { }

    public virtual bool UpgradeZone(int tribeIndex)
    {
        if (tribeIndex == GameManager.PLAYER_TRIBE_INDEX)
        {
            if (_zoneLevel + 1 >= _zoneInfo.maximumUnitsPerLevel.Length) return false; //Max Level

            int resourceQuantNeeded = _zoneInfo.resourceQuantNeededForUpgrade[_zoneLevel];
            if (GameManager.Singleton.GetPlayerScript().GetResources()[_zoneInfo.resourceNeededForUpgrade[_zoneLevel]] >= resourceQuantNeeded)
            {
                GameManager.Singleton.GetPlayerScript().SubtractToResource(_zoneInfo.resourceNeededForUpgrade[_zoneLevel], resourceQuantNeeded);
                _zoneLevel++;
                UIManager.Singleton.DisplayZoneBaseInfo(_zoneInfo, _unitsInZone.Count, _zoneLevel, _ownerTribeIndex, this);
                ExtraUIInfoUpdate();
                return true;
            }
        }
        else
        {
            //Tribe AI is trying to upgrade this
            //is the tribe owner of this zone??
        }

        return false;
    }

    protected Transform GetAvailablePatrollingPosition()
    {
        for (int i = 0; i < _unitPatrollingPositions.Length; i++)
        {
            if (_unitPatrollingPositions[i].childCount == 0)
                return _unitPatrollingPositions[i];
        }

        return null;
    }

    public void UnitEnterZone(Unit unitScr)
    {
        if(_unitsMovingTowardsZone.Contains(unitScr))
            _unitsMovingTowardsZone.Remove(unitScr);

        if (_unitsInZone.Count > 0)
        {
            if (_unitsInZone[0].TribeIndex != unitScr.TribeIndex)
            {
                _unitsInZone[_unitsInZone.Count - 1].Die();
                unitScr.Die();
            }
            else
                ReceiveUnitInZone(unitScr);
        }
        else
            ReceiveUnitInZone(unitScr);
    }

    private void ReceiveUnitInZone(Unit unitScr)
    {
        _unitsInZone.Add(unitScr);
        unitScr.transform.parent = GetAvailablePatrollingPosition();
        unitScr.transform.localPosition = Vector3.zero;
        unitScr.transform.localRotation = Quaternion.Euler(Vector3.zero);

        unitScr.SetCurrentInZone(this);
    }

    public ZoneSO GetZoneInfo() => _zoneInfo;

    public void AddUnitMovingToZone(Unit unit) => _unitsMovingTowardsZone.Add(unit);
    public void RemoveUnitFromZone(Unit unit) => _unitsInZone.Remove(unit);
    public int GetUnitsInAndMovingTowardsZone(int tribeIndex)
    {
        int res = 0;

        for (int i = 0; i < _unitsInZone.Count; i++)
        {
            if (_unitsInZone[i].TribeIndex == tribeIndex) res++;
        }

        for (int e = 0; e < _unitsMovingTowardsZone.Count; e++)
        {
            if (_unitsMovingTowardsZone[e].TribeIndex == tribeIndex) res++;
        }

        return res;
    }

    public int GetPossibleNumberOfUnitsToSend(int tribeIndex)
    {
        int res = 0;

        res = _zoneInfo.maximumUnitsPerLevel[_zoneLevel];
        res -= GetUnitsInAndMovingTowardsZone(tribeIndex);

        return res;
    }
}
