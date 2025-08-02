using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class GameManager : MonoSingleton<GameManager>
{
    public const int PLAYER_TRIBE_INDEX = 1;
    public const int TRIBES_NUMBER = 4;
    [SerializeField] private Color[] _tribeColors = new Color[TRIBES_NUMBER + 1];
    [SerializeField] private Player _playerScr;
    [SerializeField] private Tribe[] _tribes = new Tribe[TRIBES_NUMBER - 1];
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
    public Player GetPlayerScript() => _playerScr;
    public Tribe GetTribeScript(int tribeIndex) => _tribes[tribeIndex - 2];

}
