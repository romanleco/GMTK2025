using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private int[] _resourcesAmount = new int[4];
    //0 = Wheat; 1 = Wood; 2 = Minerals; 3 = Units;
    [SerializeField] private LayerMask _zoneLM;
    private List<Unit> _playerUnits = new List<Unit>();
    private List<Unit> _selectedUnits = new List<Unit>();
    [SerializeField] private Button[] _selectedUnitButtons = new Button[8];

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

    public void SelectUnit(Unit selectedUnit)
    {
        if (_selectedUnits.Count < _selectedUnitButtons.Length)
        {
            if (_selectedUnits.Contains(selectedUnit))
            {
                _selectedUnits.Remove(selectedUnit);
                for (int i = 0; i < _selectedUnitButtons.Length; i++)
                {
                    if (_selectedUnitButtons[i].gameObject.activeSelf)
                    {
                        _selectedUnitButtons[i].onClick.RemoveAllListeners();
                        if (_selectedUnits.Count > 0)
                        {
                            if (i <= _selectedUnits.Count - 1)
                            {
                                _selectedUnitButtons[i].gameObject.SetActive(true);
                                _selectedUnitButtons[i].onClick.AddListener(() =>
                                {
                                    _selectedUnits.Remove(_selectedUnits[i]);
                                    _selectedUnitButtons[i].onClick.RemoveAllListeners();
                                    _selectedUnitButtons[i].gameObject.SetActive(false);
                                });
                            }
                            else
                                _selectedUnitButtons[i].gameObject.SetActive(false);
                        }
                        else
                            _selectedUnitButtons[i].gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                _selectedUnits.Add(selectedUnit);
                Debug.Log($"Selected Units Count: {_selectedUnits.Count}");
                _selectedUnitButtons[_selectedUnits.Count - 1].gameObject.SetActive(true);
                _selectedUnitButtons[_selectedUnits.Count - 1].onClick.AddListener(() =>
                {
                    _selectedUnits.Remove(selectedUnit);
                    _selectedUnitButtons[_selectedUnits.Count - 1].onClick.RemoveAllListeners();
                    _selectedUnitButtons[_selectedUnits.Count - 1].gameObject.SetActive(false);
                });
            }
        }
        else
            Debug.Log("Maximum Units Selected");

        //make the units that are already selected selected in the zone unitsInZone menu
    }

    public void AddToPlayerUnits(Unit unitScr) => _playerUnits.Add(unitScr);

    public int[] GetResources() => _resourcesAmount;
}
