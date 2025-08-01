using UnityEngine;

[CreateAssetMenu(fileName = "ZoneSO", menuName = "Scriptable Objects/ZoneSO")]
public class ZoneSO : ScriptableObject
{
    public Sprite zoneIconSprite;
    public string zoneName;
    public int[] maximumUnitsPerLevel = new int[3];
    public int[] resourceNeededForUpgrade = new int[3];
    public int[] resourceQuantNeededForUpgrade = new int[3];
}
