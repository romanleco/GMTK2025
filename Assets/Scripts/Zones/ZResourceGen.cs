using UnityEngine;

public class ZResourceGen : Zone
{
    [SerializeField] protected int _resourceToGenIndex;
    [SerializeField] private int[] _resourceQuantityGeneratedPerLevel = new int[3];

    public int GetResourceQuantGenPerLevel(int level)
    {
        if (level > _resourceQuantityGeneratedPerLevel.Length - 1) return -1;
        return _resourceQuantityGeneratedPerLevel[level];
    }

    public override void Select()
    {
        base.Select();
        UIManager.Singleton.DisplayResourceGeneratingZoneInfo(_resourceToGenIndex, _resourceQuantityGeneratedPerLevel[_zoneLevel]);
    }
}
