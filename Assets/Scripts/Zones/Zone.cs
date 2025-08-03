using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private int _tribeColorUIDisplayIndex = -1;
    public bool IsZoneMaxLevel { get; private set; } = false;

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
                if (newTribeIndex != GameManager.PLAYER_TRIBE_INDEX)
                {
                    //Let the tribe know the zone is theirs
                    GameManager.Singleton.GetTribeScript(newTribeIndex).AddToOwnedZones(_indexesInGrid.Item1,
                                                                                        _indexesInGrid.Item2);

                    if (_ownerTribeIndex != 0) //It was owned by another tribe
                    {
                        //let the tribe know the zone is no longer theirs
                        GameManager.Singleton.GetTribeScript(_ownerTribeIndex).RemoveFromOwnedZones(_indexesInGrid.Item1,
                                                                                                    _indexesInGrid.Item2);
                    }
                }

                _ownerTribeIndex = newTribeIndex;
                MapManager.Singleton.SetGridTribeControl(_indexesInGrid.Item1, _indexesInGrid.Item2, _ownerTribeIndex);

                if (_tribeColorUIDisplayIndex == -1)
                {
                    _tribeColorUIDisplayIndex = WorldSpaceUIManager.Singleton.GetColorDisplayIndex(transform.position + (Vector3.up * 1.5f));
                }
                WorldSpaceUIManager.Singleton.ChangeTribeColorDisplay(_tribeColorUIDisplayIndex, _ownerTribeIndex);
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
        if (IsZoneMaxLevel) return false; //Max Level

        int resourceQuantNeeded = _zoneInfo.resourceQuantNeededForUpgrade[_zoneLevel];

        int resourceQuant;
        if (tribeIndex == GameManager.PLAYER_TRIBE_INDEX)
            resourceQuant = GameManager.Singleton.GetPlayerScript().GetResources()[_zoneInfo.resourceNeededForUpgrade[_zoneLevel]];
        else
            resourceQuant = GameManager.Singleton.GetTribeScript(tribeIndex).GetResources()[_zoneInfo.resourceNeededForUpgrade[_zoneLevel]];

        if (resourceQuant >= resourceQuantNeeded)
        {
            _zoneLevel++;

            if (tribeIndex == GameManager.PLAYER_TRIBE_INDEX)
            {
                GameManager.Singleton.GetPlayerScript().SubtractToResource(_zoneInfo.resourceNeededForUpgrade[_zoneLevel], resourceQuantNeeded);
                UIManager.Singleton.DisplayZoneBaseInfo(_zoneInfo, _unitsInZone.Count, _zoneLevel, _ownerTribeIndex, this);
                ExtraUIInfoUpdate();
            }
            else
                GameManager.Singleton.GetTribeScript(tribeIndex).ResourceTransaction(_zoneInfo.resourceNeededForUpgrade[_zoneLevel], resourceQuantNeeded, false);

            if (IsZoneMaxLevel == false)
                if (_zoneLevel + 1 >= _zoneInfo.maximumUnitsPerLevel.Length) IsZoneMaxLevel = true;

            MapManager.Singleton.SetGridZoneLevel(_indexesInGrid.Item1, _indexesInGrid.Item2, _zoneLevel);

            return true;
        }

        return false;
    }

    protected Transform GetAvailablePatrollingPosition()
    {
        // for (int e = 0; e < _unitPatrollingPositionsHolder.childCount; e++)
        // {
        //     _unitPatrollingPositions[e] = _unitPatrollingPositionsHolder.GetChild(e);   
        // }

        for (int i = 0; i < _unitPatrollingPositions.Length; i++)
            {
                if (_unitPatrollingPositions[i].childCount == 0)
                    return _unitPatrollingPositions[i];
            }

        return null;
    }

    public void UnitEnterZone(Unit unitScr)
    {
        if (_unitsMovingTowardsZone.Contains(unitScr))
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

    public void SetIndexesInGrid(int x, int z) => _indexesInGrid = (x, z);
    public virtual void SetOwnedByTribe(int tribeIndex)
    {
        _ownerTribeIndex = tribeIndex;
        if (_tribeColorUIDisplayIndex == -1)
        {
            _tribeColorUIDisplayIndex = WorldSpaceUIManager.Singleton.GetColorDisplayIndex(transform.position + (Vector3.up * 1.5f));
        }
        WorldSpaceUIManager.Singleton.ChangeTribeColorDisplay(_tribeColorUIDisplayIndex, _ownerTribeIndex);
    }

    public int GetUnitsCount() => _unitsInZone.Count;
    public List<Unit> GetUnitsInZone() => _unitsInZone;
    public (int, int) GetResourcesNeededForUpgrade() => (_zoneInfo.resourceNeededForUpgrade[_zoneLevel], _zoneInfo.resourceQuantNeededForUpgrade[_zoneLevel]);
    public int AvailableUnits() => _unitsInZone.Count - ((int)(_zoneInfo.maximumUnitsPerLevel[_zoneLevel] / 3));
}
