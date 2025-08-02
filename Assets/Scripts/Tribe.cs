using System.Collections.Generic;
using UnityEngine;

public class Tribe : MonoBehaviour
{
    [SerializeField] private int _tribeIndex;
    private int[] _resources = new int[4];
    private List<Unit> _tribeUnits = new List<Unit>();
    private List<Unit> _tribeSelectedUnits = new List<Unit>();

    public int GetTribeIndex() => _tribeIndex;
    public int[] GetResources() => _resources;


    public void ResourceTransaction(int resourceIndex, int resourceCount, bool add = true)
    {
        if (add)
        {
            _resources[resourceIndex] += resourceCount;
        }
        else
        {
            _resources[resourceIndex] -= resourceCount;
            if (_resources[resourceIndex] < 0) _resources[resourceIndex] = 0;
        }
    }

    public void AddToTribeUnits(Unit unitScr) => _tribeUnits.Add(unitScr);
}
