using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string sceneName;
    public void StartGame()
    {
        SceneManager.LoadScene(sceneName);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
