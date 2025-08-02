using UnityEngine;
using System.Collections.Generic;

public class ZSettlement : Zone
{
    [SerializeField] private Unit _unitPrefab;
    [SerializeField] private int _unitGenerationCost = 3;
    [SerializeField] private MeshRenderer _flagMeshRenderer;
    [SerializeField] private List<Unit> _unitsGenerated = new List<Unit>();
    [SerializeField] private int[] _maxUnitsToGeneratePerLevel = new int[3];

    public override void ExtraUIInfoUpdate()
    {
        UIManager.Singleton.DisplaySettlementZoneInfo(_unitsGenerated.Count, _maxUnitsToGeneratePerLevel[_zoneLevel]);
    }

    public bool GenerateUnit(int tribeIndex)
    {
        if (tribeIndex == GameManager.PLAYER_TRIBE_INDEX)
        {
            if (_maxUnitsToGeneratePerLevel[_zoneLevel] <= _unitsGenerated.Count) return false; //If the zone has generated less than the maximum

            if (GameManager.Singleton.GetPlayerScript().GetResources()[0] >= _unitGenerationCost)
            {
                Transform _aPP = GetAvailablePatrollingPosition();
                if (_aPP != null)
                {
                    Unit newUnitScr = Instantiate(_unitPrefab, _aPP);
                    newUnitScr.SetTribe(tribeIndex);
                    newUnitScr.transform.localPosition = Vector3.zero;
                    newUnitScr.SetZoneOfGeneration(this);
                    newUnitScr.SetCurrentInZone(this);

                    _unitsGenerated.Add(newUnitScr);
                    _unitsInZone.Add(newUnitScr);
                    GameManager.Singleton.GetPlayerScript().SubtractToResource(0, _unitGenerationCost);
                    GameManager.Singleton.GetPlayerScript().AddToResource(3, 1); //subtracts to units from tribe
                    GameManager.Singleton.GetPlayerScript().AddToPlayerUnits(newUnitScr);

                    UIManager.Singleton.RefreshZoneUI();

                    return true;
                }
            }
        }
        else
        {

        }

        return false;
    }

    protected override void OnLoopCompletedAction()
    {
        int prevOwnerTribeIndex = _ownerTribeIndex;
        base.OnLoopCompletedAction();
        if (_ownerTribeIndex != prevOwnerTribeIndex)
            _flagMeshRenderer.material.color = GameManager.Singleton.GetTribeColor(_ownerTribeIndex);
    }

    public void RemoveUnitFromUnitsGenerated(Unit unitScr) => _unitsGenerated.Remove(unitScr);
}
