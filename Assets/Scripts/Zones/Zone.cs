using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    [SerializeField] protected ZoneSO _zoneInfo;
    [SerializeField] protected (int, int) _indexesInGrid;
    protected int _ownerTribeIndex;
    protected int _zoneLevel = 0;
    protected List<GameObject> _unitsInZone = new List<GameObject>();

    void OnEnable() => LoopTimerManager.OnLoopComplete += OnLoopCompletedAction;
    void OnDisable() => LoopTimerManager.OnLoopComplete -= OnLoopCompletedAction;

    private void OnLoopCompletedAction()
    {
        //If zone conquered
            //give resource to the corresponding tribe depending on level (by _zoneInfo)
    }

    public virtual void Select() //What happens when the zone gets selected
    {
        Debug.Log($"Selected | T: {Time.time}");
        UIManager.Singleton.DisplayZoneBaseInfo(_zoneInfo, _unitsInZone.Count, _zoneLevel, _ownerTribeIndex);
    }

    public ZoneSO GetZoneInfo() => _zoneInfo;
}
