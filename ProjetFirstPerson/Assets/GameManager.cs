using System.Collections;
using System.Collections.Generic;
using ArthurUtilities;
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

    public float startFadeDuration;
    public Image fadeImage;
    
    void Start()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }
    public void StartGame()
    {
        StartCoroutine(WaitStartGame());
        AudioManager.Instance.FadeOutAudioSource(2f,1);
    }

    public IEnumerator WaitStartGame()
    {
        StartCoroutine(FadeScreen(startFadeDuration,1));
        yield return new WaitForSeconds(startFadeDuration);
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
    

   public void QuitSettings()
   {
       SettingsMenu.SetActive(false);
   }
   
   private float fadeScreenTimer;
   public IEnumerator FadeScreen(float fadeDuration, float wantedValue)
   {
       fadeScreenTimer = 0;
       float saveFadeValue = fadeImage.color.a;

       while (fadeScreenTimer < fadeDuration)
       {
           fadeScreenTimer += Time.deltaTime;

           fadeImage.color = new Color(fadeImage.color.r,
               fadeImage.color.g, fadeImage.color.b, 
               Mathf.Lerp(saveFadeValue, wantedValue, fadeScreenTimer / fadeDuration));

           yield return null;
       }
   }
}
