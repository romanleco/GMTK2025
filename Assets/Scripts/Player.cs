using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int[] _resourcesAmount = new int[5];
    //0 = Wheat; 1 = Wood; 2 = Minerals; 3 = Units; 4 = Coins

    public void AddToResource(int resourceIndex, int valueToAdd)
    {
        _resourcesAmount[resourceIndex] += valueToAdd;
        UIManager.Singleton.UpdateResourceCount(resourceIndex, _resourcesAmount[resourceIndex]);
    }

    public void SubtractToResource(int resourceIndex, int valueToSubtract)
    {
        _resourcesAmount[resourceIndex] -= valueToSubtract;
        if (_resourcesAmount[resourceIndex] < 0)
        {
            Debug.Log("RESOURCE WENT INTO THE NEGATIVES");
            _resourcesAmount[resourceIndex] = 0;
        }
        UIManager.Singleton.UpdateResourceCount(resourceIndex, _resourcesAmount[resourceIndex]);
    }
}
