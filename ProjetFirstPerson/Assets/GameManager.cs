using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public string sceneName;
    public GameObject SettingsMenu;
    public List<Button> buttons;
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    void Start()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }
    public void StartGame()
    {
        SceneManager.LoadScene(sceneName);
    }
    public void OpenSettings()
    {
        SettingsMenu.SetActive(true);
    }
    public void Quit()
    {
        Application.Quit();
    }

   /* void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(SettingsMenu.activeSelf)
            {
                SettingsMenu.SetActive(false);
                for(int i = 0; i < buttons.Count; i++)
                {
                    buttons[i].interactable = false;
                    buttons[i].interactable = true;
                }
            }
        }
    }*/

   public void QuitSettings()
   {
       SettingsMenu.SetActive(false);
   }
}
