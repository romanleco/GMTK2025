using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public const int TRIBES_NUMBER = 4;
    [SerializeField] private Color[] _tribeColors = new Color[TRIBES_NUMBER + 1];
    void Start()
    {
        DataContainer loadedData = SaveManager.Singleton.Load();
        if (loadedData != null)
        {
            Debug.Log("Save file found");
        }
        else
        {
            Debug.Log("No save file detected, creating a new save file");
            SaveManager.Singleton.Save(100, 100, true, true);
        }
    }

    public Color GetTribeColor(int index) => _tribeColors[index];
}
