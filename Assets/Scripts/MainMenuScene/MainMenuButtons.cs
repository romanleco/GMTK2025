using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButtons : MonoBehaviour
{
    public void Play()
    {
        SceneManager.Singleton.ChangeScene("MainScene");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
