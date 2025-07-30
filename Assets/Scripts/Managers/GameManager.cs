using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    void Start()
    {
        DataContainer loadedData = SaveManager.Instance.Load();
        if(loadedData != null)
        {
            Debug.Log("Save file found");
        }
        else
        {
            Debug.Log("No save file detected, creating a new save file");
            SaveManager.Instance.Save(100, 100, true, true);
        }
    }

    public Vector3 playerPosition {get; private set;}

    public void SetPlayerPosition(Vector3 position)
    {
        playerPosition = position;
    } 
}
