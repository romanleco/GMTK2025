using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WorldSpaceUIManager : MonoSingleton<WorldSpaceUIManager>
{
    private List<Image> _zoneTribeColorDisplays = new List<Image>();
    [SerializeField] private Image _zoneTribeColorDisplayPrefab;

    public void ChangeTribeColorDisplay(int displayIndex, int tribeIndex)
    {
        _zoneTribeColorDisplays[displayIndex].color = GameManager.Singleton.GetTribeColor(tribeIndex);
    }

    public int GetColorDisplayIndex(Vector3 position)
    {
        Image newDisplay = Instantiate(_zoneTribeColorDisplayPrefab, position, Quaternion.identity);
        _zoneTribeColorDisplays.Add(newDisplay);
        newDisplay.transform.parent = transform;
        
        newDisplay.transform.position = position;

        return _zoneTribeColorDisplays.Count - 1;
    }
}
