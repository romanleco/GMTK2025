using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoSingleton<MapManager>
{
    [Header("Zones")]
    [SerializeField] private GameObject _farmZonePrefab;
    [SerializeField] private GameObject _forestZonePrefab;
    [SerializeField] private GameObject _mineZonePrefab;
    [SerializeField] private GameObject _settlementZonePrefab;
    [SerializeField] private int _nOfFarms = 25; // 1/16
    [SerializeField] private int _nOfForests = 50; // 1/8
    [SerializeField] private int _nOfMines = 25; // 1/16
    [SerializeField] private int _nOfSettlements = 20; // 1/20

    [Header("Tile Visualization")]
    [SerializeField] private GameObject _cellPositionVisualizer;
    public const int gridSize = 20;
    public const float gridCellSize = 2.25f;
    private Vector3[,] _gridPositions = new Vector3[gridSize, gridSize]; //The position of each cell on the grid
    private int[,] _gridTribeControl = new int[gridSize, gridSize]; //What faction controls the tile; 0 = none
    private int[,] _gridZones = new int[gridSize, gridSize];

    void Start()
    {
        InitializeGridZonesArr();
        CalculateGridPositions();
        // VisualizeCellPositions();
        ZonesSpawning();
    }

    private void CalculateGridPositions()
    {
        Vector3 assignmentVector = Vector3.zero;
        assignmentVector.y = 1;

        for (int i = 0; i < gridSize; i++)
        {
            for (int e = 0; e < gridSize; e++)
            {
                assignmentVector.x = (gridCellSize / 2) + (gridCellSize * i);
                assignmentVector.z = -((gridCellSize / 2) + (gridCellSize * e));
                _gridPositions[i, e] = assignmentVector;
            }
        }
    }

    private void ZonesSpawning()
    {
        int farmN = _nOfFarms;
        int forestN = _nOfForests;
        int minesN = _nOfMines;
        int settlementsN = _nOfSettlements;
        int totalZones = farmN + forestN + minesN + settlementsN;

        List<(int, int)> cellsList = new(); // Make and fill List with all the cells in the grid (posX, posZ)
        for (int i = 0; i < gridSize; i++)
        {
            for (int e = 0; e < gridSize; e++)
            {
                cellsList.Add((i, e));
            }
        }

        InitializePossibleZones();
        int randIndex;
        int zoneIndex;
        while (farmN > 0 || forestN > 0 || minesN > 0 || settlementsN > 0)
        {
            randIndex = Random.Range(0, cellsList.Count);
            zoneIndex = PickZoneToSpawn(farmN, forestN, minesN, settlementsN);
            if (zoneIndex == -1)
            {
                Debug.LogError("No available zones to spawn");
                break;
            }

            switch (zoneIndex)
            {
                case 0:
                    Instantiate(_farmZonePrefab,
                                _gridPositions[cellsList[randIndex].Item1, cellsList[randIndex].Item2],
                                Quaternion.identity);
                    _gridZones[cellsList[randIndex].Item1, cellsList[randIndex].Item2] = 0;
                    farmN--;
                    break;

                case 1:
                    Instantiate(_forestZonePrefab,
                                _gridPositions[cellsList[randIndex].Item1, cellsList[randIndex].Item2],
                                Quaternion.identity);
                    _gridZones[cellsList[randIndex].Item1, cellsList[randIndex].Item2] = 1;
                    forestN--;
                    break;

                case 2:
                    Instantiate(_mineZonePrefab,
                                _gridPositions[cellsList[randIndex].Item1, cellsList[randIndex].Item2],
                                Quaternion.identity);
                    _gridZones[cellsList[randIndex].Item1, cellsList[randIndex].Item2] = 2;
                    minesN--;
                    break;

                case 3:
                    Instantiate(_settlementZonePrefab,
                                _gridPositions[cellsList[randIndex].Item1, cellsList[randIndex].Item2],
                                Quaternion.identity);
                    _gridZones[cellsList[randIndex].Item1, cellsList[randIndex].Item2] = 3;
                    settlementsN--;
                    break;

                default:
                    Debug.LogError("Invalid or unrecognized Zone Index");
                    break;
            }
            cellsList.RemoveAt(randIndex);

            if (totalZones <= 0) //In case there is a problem with the spawning break the loop
            {
                Debug.LogError($"Error in spawning | TotalZones: {totalZones}");
                break;
            }
            totalZones--;
        }
    }

    List<int> possibleZones = new List<int>();
    private void InitializePossibleZones() { for (int i = 0; i < 4; i++) possibleZones.Add(i); }
    private int PickZoneToSpawn(int farmN, int forestN, int minesN, int settlementsN)
    {
        if (farmN <= 0) possibleZones.Remove(0);
        if (forestN <= 0) possibleZones.Remove(1);
        if (minesN <= 0) possibleZones.Remove(2);
        if (settlementsN <= 0) possibleZones.Remove(3);

        if (possibleZones.Count < 1) return -1;
        int rand = Random.Range(0, possibleZones.Count);

        return possibleZones[rand];
    }

    private void VisualizeCellPositions()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int e = 0; e < gridSize; e++)
            {
                Instantiate(_cellPositionVisualizer, _gridPositions[i, e], Quaternion.identity);
            }
        }
    }

    private void InitializeGridZonesArr()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int e = 0; e < gridSize; e++)
            {
                _gridZones[i, e] = -1;
            }
        }
    }
}
