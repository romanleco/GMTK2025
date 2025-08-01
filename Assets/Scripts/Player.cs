using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    [SerializeField] private int[] _resourcesAmount = new int[4];
    //0 = Wheat; 1 = Wood; 2 = Minerals; 3 = Units;
    [SerializeField] private LayerMask _zoneLM;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            SelectZone();
    }

    private void SelectZone()
    {
        Ray raySelector = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;

        if (Physics.Raycast(raySelector, out raycastHit, Mathf.Infinity, _zoneLM))
        {
            Zone zoneScr = raycastHit.transform.GetComponent<Zone>();
            if (zoneScr != null)
            {
                zoneScr.Select();
            }
        }
        else if (EventSystem.current.IsPointerOverGameObject() == false)
            UIManager.Singleton.CloseZoneMenu();
    }

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

    public int[] GetResources() => _resourcesAmount;
}
