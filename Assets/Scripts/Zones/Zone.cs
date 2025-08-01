using System.Collections.Generic;
using NUnit.Framework;
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

    protected virtual void OnLoopCompletedAction()
    {
        //If zone conquered
        //give resource to the corresponding tribe depending on level (by _zoneInfo)
    }

    public virtual void Select() //What happens when the zone gets selected
    {
        Debug.Log($"Selected | T: {Time.time}");
        UIManager.Singleton.DisplayZoneBaseInfo(_zoneInfo, _unitsInZone.Count, _zoneLevel, _ownerTribeIndex, this);
        ExtraUIInfoUpdate();
    }

    public virtual void ExtraUIInfoUpdate() { }

    public virtual bool UpgradeZone(int tribeIndex)
    {
        if (tribeIndex == GameManager.PLAYER_TRIBE_INDEX)
        {
            if (_zoneLevel + 1 >= _zoneInfo.maximumUnitsPerLevel.Length) return false; //Max Level

            int resourceQuantNeeded = _zoneInfo.resourceQuantNeededForUpgrade[_zoneLevel];
            if (GameManager.Singleton.GetPlayerScript().GetResources()[_zoneInfo.resourceNeededForUpgrade[_zoneLevel]] >= resourceQuantNeeded)
            {
                GameManager.Singleton.GetPlayerScript().SubtractToResource(_zoneInfo.resourceNeededForUpgrade[_zoneLevel], resourceQuantNeeded);
                _zoneLevel++;
                UIManager.Singleton.DisplayZoneBaseInfo(_zoneInfo, _unitsInZone.Count, _zoneLevel, _ownerTribeIndex, this);
                ExtraUIInfoUpdate();
                return true;
            }
        }
        else
        {
            //Tribe AI is trying to upgrade this
            //is the tribe owner of this zone??
        }

        return false;
    }

    public ZoneSO GetZoneInfo() => _zoneInfo;
}
