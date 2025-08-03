using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tribe : MonoBehaviour
{
    //Shared with Player
    [SerializeField] private int _tribeIndex;
    private int[] _resources = new int[4];
    private List<Unit> _tribeUnits = new List<Unit>();
    private List<Unit> _tribeSelectedUnits = new List<Unit>();

    public int GetTribeIndex() => _tribeIndex;
    public int[] GetResources() => _resources;

    //Not shared with Player
    private int[] _zonesOwnedQuant = new int[4];
    private List<(int, int)> _ownedZones = new List<(int, int)>();
    private int _wantsToConquerZoneWIndex = 0;
    private int risk = 0;
    private int _riskIncreasePerLoopMin = 4;
    private int _riskIncreasePerLoopMax = 8;
    private int _unitGenerationCost = 3;

    void OnEnable() => LoopTimerManager.OnLoopComplete += OnLoopCompleteBehaviour;
    void OnDisable() => LoopTimerManager.OnLoopComplete -= OnLoopCompleteBehaviour;

    public void InitializeTribe(int x, int z)
    {
        ResourceTransaction(0, 12);
        AddToOwnedZones(x, z);
        MapManager.Singleton.SetGridTribeControl(x, z, _tribeIndex);
        ZSettlement settlementScr = MapManager.Singleton.GridZonesScr[x, z].GetComponent<ZSettlement>();
        if (settlementScr != null)
        {
            settlementScr.SetOwnedByTribe(_tribeIndex);
            for (int i = 0; i < 3; i++)
            {
                settlementScr.GenerateUnit(_tribeIndex);
            }
        }
        else
            Debug.LogError("Tribe Starter Settlement Null");
    }

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
    public void AddToOwnedZones(int gridIndexesItem1, int gridIndexesItem2)
    {
        if (_ownedZones.Contains((gridIndexesItem1, gridIndexesItem2)) == false)
            _ownedZones.Add((gridIndexesItem1, gridIndexesItem2));
    }

    public void RemoveFromOwnedZones(int gridIndexesItem1, int gridIndexesItem2)
    {
        if (_ownedZones.Contains((gridIndexesItem1, gridIndexesItem2)))
            _ownedZones.Remove((gridIndexesItem1, gridIndexesItem2));
    }


    private void OnLoopCompleteBehaviour()
    {
        CalculateZonesOwnedQuant();
        LogicTree();
    }

    public void InformUnitsSentToOwnedZone()
    {
        //Affect this by risk

        //What to do when an owned zone gets attacked
    }

    private bool CheckForEmptyOwnedTiles()
    {
        //Affect this by risk

        //Check for tiles that are owned but don't have any units in them
        //if attacks returns true
        return false;
    }

    private void CalculateZonesOwnedQuant()
    {
        for (int zT = 0; zT < _zonesOwnedQuant.Length; zT++) //Reset zones owned
        {
            _zonesOwnedQuant[zT] = 0;
        }

        for (int i = 0; i < MapManager.gridSize; i++)
        {
            for (int e = 0; e < MapManager.gridSize; e++)
            {
                if (MapManager.Singleton.GridZones[i, e] >= 0)
                {
                    if (MapManager.Singleton.GridTribeControl[i, e] == _tribeIndex)
                    {
                        _zonesOwnedQuant[MapManager.Singleton.GridZones[i, e]]++;
                    }
                }
            }
        }
    }

    private void LogicTree()
    {
        risk += Random.Range(_riskIncreasePerLoopMin, _riskIncreasePerLoopMax);

        _wantsToConquerZoneWIndex = -1;
        if (4 > Random.Range(0, 101)) //very low chance of wanting to conquer a random zone
        {
            _wantsToConquerZoneWIndex = Random.Range(0, _resources.Length);
            Conquer();
            return;
        }

        if (4 > Random.Range(0, 101)) //very low chance of trying to upgrade a random zone
        {
            UpgradeZonesOfIndex(Random.Range(0, _resources.Length));
        }

        bool randomWheatSkip = Random.Range(0, 101) < 15;
        if (_zonesOwnedQuant[0] < 2) randomWheatSkip = false;

        // if the wheat produced is less than number of units + 4
        if (GetWheatProducedPerLoop() < _tribeUnits.Count + 4 && randomWheatSkip == false)// 4 is the quantity produced by a level 1 farm
        {
            int rand = Random.Range(0, 101);

            if (_zonesOwnedQuant[0] < 2)
            {
                //conquer farm
                _wantsToConquerZoneWIndex = 0;
            }
            else if (_zonesOwnedQuant[0] <= 5)
            {
                //medium chance of conquering
                int randNum = Random.Range(0, 101);
                if (randNum > 65 || risk > Random.Range(20, 101))
                {
                    if (randNum <= 65) risk = 0;
                    _wantsToConquerZoneWIndex = 0;
                }
                else
                {
                    UpgradeFarmOrSeekResources();
                }
            }
            else
            {
                //low chance of conquering unless high risk
                int randNum2 = Random.Range(0, 101);
                if (randNum2 > 90 || risk > Random.Range(40, 101))
                {
                    if (randNum2 <= 90) risk = 0;
                    _wantsToConquerZoneWIndex = 0;
                }
                else
                {
                    UpgradeFarmOrSeekResources();
                }
            }
        }
        else
        {
            //build new units if possible
            int unitsGenerated = GenerateUnits();
            if (unitsGenerated < 1)
                _wantsToConquerZoneWIndex = 3;
            //otherwise wants to conquer a settlement

            for (int i = 0; i < _ownedZones.Count; i++)
            {
                if (MapManager.Singleton.GridZones[_ownedZones[i].Item1, _ownedZones[i].Item2] == 3)
                {
                    //if there are settlements with more than half + 1 units generated
                    if (SettlementGettingFull(_ownedZones[i].Item1, _ownedZones[i].Item2))
                    {
                        _wantsToConquerZoneWIndex = 3;
                        break;
                        // wants to conquer a settlement
                    }
                }
            }

            if (_wantsToConquerZoneWIndex == -1) //if wants to conquer nothing
            {
                //random chance to want to conquer a forest or mine
                int randomForestOrMineConquer = Random.Range(0, 101);
                if (randomForestOrMineConquer > 85)
                    _wantsToConquerZoneWIndex = Random.Range(1, 3);
            }

        }

        Conquer();
    }

    private void UpgradeFarmOrSeekResources()
    {
        //if has the necessary resources to upgrade do it
        int upgradedFarms = UpgradeZonesOfIndex(0);
        if (upgradedFarms > 0)
        {
            //else look for them
            int indexToConquer = Random.Range(1, 3); //forest or mine;
            _wantsToConquerZoneWIndex = indexToConquer;
        }
        else if (upgradedFarms == -1) //no farm to upgrade, let's conquer one or upgrade other type
        {
            if (Random.Range(0, 101) > 45)
                _wantsToConquerZoneWIndex = 0;
            else
            {
                int indexToUpgrade = Random.Range(1, 3);
                int upgradedZones = UpgradeZonesOfIndex(indexToUpgrade);
                if (upgradedZones == -1) //if every zone of index upgraded conquer one
                    _wantsToConquerZoneWIndex = indexToUpgrade;
            }

        }
    }

    private int GetWheatProducedPerLoop()
    {
        int res = 0;

        ZResourceGen zoneResourceGen;
        for (int i = 0; i < _ownedZones.Count; i++)
        {
            if (MapManager.Singleton.GridZones[_ownedZones[i].Item1, _ownedZones[i].Item2] == 0)
            {
                zoneResourceGen = MapManager.Singleton.GridZonesScr[_ownedZones[i].Item1, _ownedZones[i].Item2].GetComponent<ZResourceGen>();
                if (zoneResourceGen != null)
                {
                    res += zoneResourceGen.GetResourceOutput();
                }
            }
        }

        return res;
    }

    private int GetUpgradeResourceNeededIndex()
    {
        int res = 0;

        return res;
    }

    private int GenerateUnits()
    {
        int res = 0;

        int unitsToGen = (int)(_resources[0] / _unitGenerationCost);
        if (unitsToGen > 0)
        {
            ZSettlement suitableSettlement = GetSuitableSettlementToGen();
            if (suitableSettlement != null)
            {
                for (int i = 0; i < unitsToGen; i++)
                {
                    suitableSettlement.GenerateUnit(_tribeIndex);
                    if (suitableSettlement.IsFullOfUnits())
                    {
                        suitableSettlement = GetSuitableSettlementToGen();
                        if (suitableSettlement == null) break;
                    }
                }
            }
        }

        return res;
    }

    private ZSettlement GetSuitableSettlementToGen()
    {
        ZSettlement zSettlement = null;
        for (int i = 0; i < _ownedZones.Count; i++)
        {
            if (MapManager.Singleton.GridZones[_ownedZones[i].Item1, _ownedZones[i].Item2] == 3)
            {
                ZSettlement settlementScr = MapManager.Singleton.GridZonesScr[_ownedZones[i].Item1, _ownedZones[i].Item2].GetComponent<ZSettlement>();
                if (settlementScr != null)
                {
                    if (zSettlement == null)
                    {
                        if (settlementScr.IsFullOfUnits() == false)
                            zSettlement = settlementScr;
                    }
                    else
                    {
                        if (settlementScr.GetUnitsGeneratedCount() < zSettlement.GetUnitsGeneratedCount())
                            zSettlement = settlementScr;
                    }
                }
            }
        }

        return zSettlement;
    }

    private bool SettlementGettingFull(int x, int z)
    {
        ZSettlement zSettlement = null;
        for (int i = 0; i < _ownedZones.Count; i++)
        {
            if (MapManager.Singleton.GridZones[_ownedZones[i].Item1, _ownedZones[i].Item2] == 3)
            {
                zSettlement = MapManager.Singleton.GridZonesScr[_ownedZones[i].Item1, _ownedZones[i].Item2].GetComponent<ZSettlement>();
                if (zSettlement != null)
                {
                    if (zSettlement.IsGettingFullOfUnits()) return true;
                }
            }
        }

        return false;
    }

    private int UpgradeZonesOfIndex(int zoneIndex)
    {
        //if it returns -1 it means every zone to be upgraded is to the max level
        int res = -1;

        Zone zoneScr = null;
        for (int i = 0; i < _ownedZones.Count; i++)
        {
            if (MapManager.Singleton.GridZones[_ownedZones[i].Item1, _ownedZones[i].Item2] == zoneIndex)
            {
                zoneScr = MapManager.Singleton.GridZonesScr[_ownedZones[i].Item1, _ownedZones[i].Item2];
                if (zoneScr != null)
                {
                    if (zoneScr.IsZoneMaxLevel == false)
                    {
                        if (res == -1) res = 0;
                        (int, int) resourcesNeededForUpgrade = zoneScr.GetResourcesNeededForUpgrade();
                        if (_resources[resourcesNeededForUpgrade.Item1] >= resourcesNeededForUpgrade.Item2)
                        {
                            zoneScr.UpgradeZone(_tribeIndex);
                            res++;
                        }
                    }
                }
            }
        }

        return res;
    }

    private void Conquer()
    {
        if (_wantsToConquerZoneWIndex == -1) return;

        bool mustConquer = false;

        //if it doesn't have available units and has resources to make them
        availableUnitsList = GetAvailableUnits();
        if (!(availableUnitsList.Count > 0))
        {
            int unitsToGenerate = (_resources[0] / _unitGenerationCost) - 1;
            if (unitsToGenerate < 1 && _resources[0] >= _unitGenerationCost) unitsToGenerate = 1;

            //generate unit and add it to the available list
            ZSettlement settlementScr = GetSuitableSettlementToGen();
            if (settlementScr != null)
            {
                Unit newUnitScr;
                for (int i = 0; i < unitsToGenerate; i++)
                {
                    newUnitScr = settlementScr.GenerateUnit(_tribeIndex);
                    if (newUnitScr != null)
                        availableUnitsList.Add(newUnitScr);
                    else
                    {
                        settlementScr = GetSuitableSettlementToGen();
                        if (settlementScr == null)
                            break;
                    }
                }
            }
            else
            {
                //try to take one unit per zone unless zone only has one
                Zone zoneScr = null;
                for (int i = 0; i < _ownedZones.Count; i++)
                {
                    if (MapManager.Singleton.GridZones[_ownedZones[i].Item1, _ownedZones[i].Item2] == 3)
                    {
                        zoneScr = MapManager.Singleton.GridZonesScr[_ownedZones[i].Item1, _ownedZones[i].Item2];
                        if (zoneScr != null)
                        {
                            if (zoneScr.GetUnitsCount() > 1)
                                availableUnitsList.Add(zoneScr.GetUnitsInZone()[0]);
                        }
                    }
                }

                if (availableUnitsList.Count < 1) availableUnitsList.Add(_tribeUnits[Random.Range(0, _tribeUnits.Count)]);
                _wantsToConquerZoneWIndex = 3;
                mustConquer = true;
            }
        }

        if (mustConquer)
        {
            Zone zoneToConquer = GetBestZoneToConquer(_wantsToConquerZoneWIndex);
            for (int i = 0; i < availableUnitsList.Count; i++)
            {
                if (zoneToConquer != null)
                {
                    if (availableUnitsList[i] != null)
                        availableUnitsList[i].SetUnitDestination(zoneToConquer);
                    else
                        availableUnitsList.Remove(availableUnitsList[i]);
                }
            }
        }
        else
        {
            if (availableUnitsList.Count > 0)
            {
                Zone zoneToConquer = GetBestZoneToConquer(_wantsToConquerZoneWIndex);
                if (zoneToConquer != null)
                {
                    int rand = Random.Range(15, 101);
                    if (zoneToConquer.GetUnitsCount() < availableUnitsList.Count || risk > rand)
                    {
                        if (zoneToConquer.GetUnitsCount() >= availableUnitsList.Count) risk = 0;
                        for (int i = 0; i < availableUnitsList.Count; i++)
                        {
                            availableUnitsList[i].SetUnitDestination(zoneToConquer);
                        }
                    }
                    else
                    {
                        Zone zoneWithLessUnits = GetOwnedZoneWithLessUnits();
                        if (zoneWithLessUnits != null)
                        {
                            for (int i = 0; i < availableUnitsList.Count; i++)
                            {
                                availableUnitsList[i].SetUnitDestination(zoneWithLessUnits);
                            }
                        }
                    }

                }
                else
                {
                    Zone zoneWithLessUnits = GetOwnedZoneWithLessUnits();
                    if (zoneWithLessUnits != null)
                    {
                        for (int i = 0; i < availableUnitsList.Count; i++)
                        {
                            availableUnitsList[i].SetUnitDestination(zoneWithLessUnits);
                        }
                    }
                }
            }
        }

        availableUnitsList.Clear();
    }

    private List<Unit> availableUnitsList = new List<Unit>();

    private Zone GetBestZoneToConquer(int zoneIndex)
    {
        Zone zoneScr = null;
        (int, int) closestZoneIndex = (0, 0);
        Vector2 dist1;
        Vector2 dist2;
        float lowestDistance = 1000;

        for (int i = 0; i < MapManager.gridSize; i++)
        {
            for (int e = 0; e < MapManager.gridSize; e++)
            {
                if (MapManager.Singleton.GridZones[i, e] == zoneIndex && MapManager.Singleton.GridTribeControl[i, e] == 0)
                {
                    if (zoneScr == null)
                    {
                        zoneScr = MapManager.Singleton.GridZonesScr[i, e];
                        closestZoneIndex = (i, e);

                        dist1.x = i;
                        dist1.y = e;

                        for (int x = 0; x < _ownedZones.Count; x++)
                        {
                            dist2.x = _ownedZones[x].Item1;
                            dist2.y = _ownedZones[x].Item2;

                            if (lowestDistance > Vector2.Distance(dist1, dist2))
                                lowestDistance = Vector2.Distance(dist1, dist2);
                        }
                    }
                    else
                    {
                        //Get the closest to an owned zone
                        dist1.x = closestZoneIndex.Item1;
                        dist1.y = closestZoneIndex.Item2;

                        for (int x = 0; x < _ownedZones.Count; x++)
                        {
                            dist2.x = i;
                            dist2.y = e;

                            if (Vector2.Distance(dist1, dist2) < lowestDistance)
                            {
                                lowestDistance = Vector2.Distance(dist1, dist2);
                                closestZoneIndex = (i, e);
                            }
                        }

                        zoneScr = MapManager.Singleton.GridZonesScr[closestZoneIndex.Item1, closestZoneIndex.Item2];
                    }
                }
            }
        }

        if (zoneScr == null)
        {
            List<(int, int)> zonesWithLowestUnitCount = new List<(int, int)>();
            int lowestUnitCount = 10;
            Zone currentZone;

            for (int i = 0; i < MapManager.gridSize; i++)
            {
                for (int e = 0; e < MapManager.gridSize; e++)
                {
                    if (MapManager.Singleton.GridZones[i, e] == zoneIndex)
                    {
                        currentZone = MapManager.Singleton.GridZonesScr[i, e];
                        if (currentZone != null)
                        {
                            if (currentZone.GetUnitsCount() == lowestUnitCount)
                                zonesWithLowestUnitCount.Add((i, e));
                            else if (currentZone.GetUnitsCount() < lowestUnitCount)
                            {
                                lowestUnitCount = currentZone.GetUnitsCount();
                                zonesWithLowestUnitCount.Clear();
                                zonesWithLowestUnitCount.Add((i, e));
                            }
                        }
                    }
                }
            }

            if (zonesWithLowestUnitCount.Count > 0)
            {
                if (zonesWithLowestUnitCount.Count == 1)
                    zoneScr = MapManager.Singleton.GridZonesScr[zonesWithLowestUnitCount[0].Item1, zonesWithLowestUnitCount[0].Item2];
                else
                {
                    //get the closest zone in zones with lowest unit count
                    lowestDistance = 1000;
                    closestZoneIndex = (0, 0);

                    for (int i = 0; i < zonesWithLowestUnitCount.Count; i++)
                    {
                        dist1.x = zonesWithLowestUnitCount[i].Item1;
                        dist1.y = zonesWithLowestUnitCount[i].Item2;

                        for (int oZ = 0; oZ < _ownedZones.Count; oZ++)
                        {
                            dist2.x = _ownedZones[oZ].Item1;
                            dist2.y = _ownedZones[oZ].Item2;

                            if (Vector2.Distance(dist1, dist2) > lowestDistance)
                            {
                                lowestDistance = Vector2.Distance(dist1, dist2);
                                closestZoneIndex = (zonesWithLowestUnitCount[i].Item1, zonesWithLowestUnitCount[i].Item2);
                            }
                        }
                    }

                    if (closestZoneIndex != (0, 0))
                    {
                        zoneScr = MapManager.Singleton.GridZonesScr[closestZoneIndex.Item1, closestZoneIndex.Item2];
                    }
                }
            }
        }

        return zoneScr;
    }

    private Zone GetOwnedZoneWithLessUnits()
    {
        Zone zoneScr = null;
        for (int i = 0; i < _ownedZones.Count; i++)
        {
            if (zoneScr == null) zoneScr = MapManager.Singleton.GridZonesScr[_ownedZones[i].Item1, _ownedZones[i].Item2];
            else
                if (zoneScr.GetUnitsCount() < MapManager.Singleton.GridZonesScr[_ownedZones[i].Item1, _ownedZones[i].Item2].GetUnitsCount())
                    zoneScr = MapManager.Singleton.GridZonesScr[_ownedZones[i].Item1, _ownedZones[i].Item2];
        }

        return zoneScr;
    }

    private List<Unit> GetAvailableUnits()
    {
        Zone zoneScr = null;
        for (int i = 0; i < _ownedZones.Count; i++)
        {
            zoneScr = MapManager.Singleton.GridZonesScr[_ownedZones[i].Item1, _ownedZones[i].Item2];
            if (zoneScr != null)
            {
                if (zoneScr.AvailableUnits() > 0)
                {
                    List<Unit> unitsInZone = zoneScr.GetUnitsInZone();
                    for (int e = 0; e < zoneScr.AvailableUnits(); e++)
                    {
                        availableUnitsList.Add(unitsInZone[e]);
                    }
                }
            }
        }

        List<Unit> res = availableUnitsList;
        availableUnitsList.Clear();
        return res;
    }
}
