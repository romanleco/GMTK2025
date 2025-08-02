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

        if (Input.GetMouseButtonDown(1))
            SendUnitsToZone();
            
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

    private List<Unit> _unitsCleaner = new List<Unit>();
    private void SendUnitsToZone()
    {
        if (_selectedUnits.Count <= 0) return;

        Ray raySelector = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;

        if (Physics.Raycast(raySelector, out raycastHit, Mathf.Infinity, _zoneLM))
        {
            Zone zoneScr = raycastHit.transform.GetComponent<Zone>();
            if (zoneScr != null)
            {
                int unitsToSend = _selectedUnits.Count;
                if (zoneScr.GetPossibleNumberOfUnitsToSend(GameManager.PLAYER_TRIBE_INDEX) < unitsToSend)
                    unitsToSend = zoneScr.GetPossibleNumberOfUnitsToSend(GameManager.PLAYER_TRIBE_INDEX);

                if (unitsToSend >= 1)
                {
                    for (int i = 0; i < unitsToSend; i++)
                    {
                        _selectedUnits[i].SetUnitDestination(zoneScr);
                        zoneScr.AddUnitMovingToZone(_selectedUnits[i]);
                        _unitsCleaner.Add(_selectedUnits[i]);
                    }

                    for (int e = 0; e < _unitsCleaner.Count; e++)
                    {
                        if (_selectedUnits.Contains(_unitsCleaner[e]))
                            _selectedUnits.Remove(_unitsCleaner[e]);
                    }
                    _unitsCleaner.Clear();

                    ResetSelectedUnitButtons();
                    UIManager.Singleton.RefreshZoneUI();
                }
            }
        }
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
        if (_selectedUnits.Contains(selectedUnit))
        {
            _selectedUnits.Remove(selectedUnit);
            ResetSelectedUnitButtons();
        }
        else
        {
            if (_selectedUnits.Count < _selectedUnitButtons.Length)
            {
                _selectedUnits.Add(selectedUnit);
                ResetSelectedUnitButtons();
            }
            else
                Debug.Log("Maximum Units Selected");
        }

        //make the units that are already selected selected in the zone unitsInZone menu
    }

    private void ResetSelectedUnitButtons()
    {
        for (int i = 0; i < _selectedUnitButtons.Length; i++)
        {
            if (_selectedUnits.Count > 0) //Is there any selected units?
            {
                if (i <= _selectedUnits.Count - 1)
                {
                    _selectedUnitButtons[i].gameObject.SetActive(true);
                    _selectedUnitButtons[i].onClick.RemoveAllListeners();
                    int assignementInt = i;
                    _selectedUnitButtons[i].onClick.AddListener(() =>
                    {
                        // Debug.Log($"SELECTED UNITS COUNT: {_selectedUnits.Count}| INDEX: {i}");
                        _selectedUnits.Remove(_selectedUnits[assignementInt]);
                        ResetSelectedUnitButtons();
                    });
                }
                else
                {
                    _selectedUnitButtons[i].onClick.RemoveAllListeners();
                    _selectedUnitButtons[i].gameObject.SetActive(false);
                }
            }
            else
            {
                _selectedUnitButtons[i].onClick.RemoveAllListeners();
                _selectedUnitButtons[i].gameObject.SetActive(false);
            }
            
        }
    }

    public void AddToPlayerUnits(Unit unitScr) => _playerUnits.Add(unitScr);

    public int[] GetResources() => _resourcesAmount;
}
