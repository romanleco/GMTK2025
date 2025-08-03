using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

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

    public Unit GenerateUnit(int tribeIndex)
    {
        Debug.Log("GU | Step1");
        if (IsZoneMaxLevel) return null; //If the zone has generated less than the maximum
        Debug.Log("GU | Step2");

        int wheatQuant;
        if (tribeIndex == GameManager.PLAYER_TRIBE_INDEX)
            wheatQuant = GameManager.Singleton.GetPlayerScript().GetResources()[0];
        else
            wheatQuant = GameManager.Singleton.GetTribeScript(tribeIndex).GetResources()[0];

        Debug.Log("GU | Step3");

        if (wheatQuant >= _unitGenerationCost)
        {
            Debug.Log("GU | Step4");
            Transform _aPP = GetAvailablePatrollingPosition();
            if (_aPP != null)
            {
                Debug.Log("GU | Step5");
                Unit newUnitScr = Instantiate(_unitPrefab, _aPP);
                newUnitScr.SetTribe(tribeIndex);
                newUnitScr.transform.localPosition = Vector3.zero;
                newUnitScr.SetZoneOfGeneration(this);
                newUnitScr.SetCurrentInZone(this);

                _unitsGenerated.Add(newUnitScr);
                _unitsInZone.Add(newUnitScr);

                if (tribeIndex == GameManager.PLAYER_TRIBE_INDEX)
                {
                    Debug.Log("GU | Step6");
                    Player playerScr = GameManager.Singleton.GetPlayerScript();

                    playerScr.SubtractToResource(0, _unitGenerationCost);
                    playerScr.AddToResource(3, 1); //subtracts to units from tribe
                    playerScr.AddToPlayerUnits(newUnitScr);

                    UIManager.Singleton.RefreshZoneUI();
                }
                else
                {
                    Debug.Log("GU | Step7");
                    Tribe tribeScr = GameManager.Singleton.GetTribeScript(tribeIndex);

                    tribeScr.ResourceTransaction(0, _unitGenerationCost, false);
                    tribeScr.ResourceTransaction(3, 1);
                    tribeScr.AddToTribeUnits(newUnitScr);
                }

                return newUnitScr;
            }
            else
                return null;
        }

        return null;
    }

    protected override void OnLoopCompletedAction()
    {
        int prevOwnerTribeIndex = _ownerTribeIndex;
        base.OnLoopCompletedAction();
        if (_ownerTribeIndex != prevOwnerTribeIndex)
            _flagMeshRenderer.material.color = GameManager.Singleton.GetTribeColor(_ownerTribeIndex);
    }

    public void RemoveUnitFromUnitsGenerated(Unit unitScr) => _unitsGenerated.Remove(unitScr);
    public bool IsFullOfUnits()
    {
        return _unitsGenerated.Count >= _maxUnitsToGeneratePerLevel[_zoneLevel];
    }

    public bool IsGettingFullOfUnits()
    {
        return _unitsGenerated.Count >= (_maxUnitsToGeneratePerLevel[_zoneLevel] / 2) + 1;
    }

    public int GetUnitsGeneratedCount() => _unitsGenerated.Count;

    public override void SetOwnedByTribe(int tribeIndex)
    {
        base.SetOwnedByTribe(tribeIndex);
        _flagMeshRenderer.material.color = GameManager.Singleton.GetTribeColor(_ownerTribeIndex);
    }
}
