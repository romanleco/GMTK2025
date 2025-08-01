using UnityEngine;

public class ZResourceGen : Zone
{
    [SerializeField] protected int _resourceToGenIndex;
    [SerializeField] private int[] _resourceQuantityGeneratedPerLevel = new int[3];

    public override void ExtraUIInfoUpdate()
    {
        UIManager.Singleton.DisplayResourceGeneratingZoneInfo(_resourceToGenIndex,
                                                              _resourceQuantityGeneratedPerLevel[_zoneLevel]);
    }

    protected override void OnLoopCompletedAction()
    {

    }
}
