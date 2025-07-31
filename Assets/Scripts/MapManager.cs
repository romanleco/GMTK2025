using UnityEngine;

public class MapManager : MonoSingleton<MapManager>
{
    [SerializeField] private GameObject _cellPositionVisualizer;
    public const int gridSize = 20;
    public const float gridCellSize = 2;
    private Vector3[,] _gridPositions = new Vector3[gridSize, gridSize]; //The position of each cell on the grid
    private int[,] _gridFactionControl = new int[gridSize, gridSize]; //What faction controls the tile; 0 = none

    void Start()
    {
        CalculateGridPositions();
        VisualizeCellPositions();
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
}
