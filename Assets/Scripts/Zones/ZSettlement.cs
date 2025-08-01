using UnityEngine;
using System.Collections.Generic;

public class ZSettlement : Zone
{
    [SerializeField] private MeshRenderer _flagMeshRenderer;
    [SerializeField] private List<GameObject> _unitsGenerated = new List<GameObject>();
    [SerializeField] private int[] _maxUnitsToGeneratePerLevel = new int[3];
    public override void Select()
    {
        base.Select();
        UIManager.Singleton.DisplaySettlementZoneInfo(_unitsGenerated.Count, _maxUnitsToGeneratePerLevel[_zoneLevel]);
    }
}
