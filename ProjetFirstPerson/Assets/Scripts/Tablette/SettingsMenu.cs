using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public TMP_Dropdown resolutionDropdown;

    Resolution[] resolutions;

    public Slider musicSlider;
    public Slider soundSlider;
    private GameObject player;

    public Toggle FullScreenToggle;

    public static SettingsMenu instance;

    private void Awake()
    {
        if (instance != null)
            {
              Destroy(gameObject);
              return;
            }
            instance = this;

            FullScreenToggle.isOn = (PlayerPrefs.GetInt("FullScreen", 1) == 1);
    }

    public void Start()
    {
        // Note à moi même : L'audio mixer est désactivé pour l'instant
        player = GameObject.Find("Player");
      /*  audioMixer.GetFloat("Music", out float musicValueForSlider);
        musicSlider.value = musicValueForSlider;

        audioMixer.GetFloat("Sound", out float soundValueForSlider);
        soundSlider.value = soundValueForSlider; */

        resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if(resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = PlayerPrefs.GetInt("resolution", 21);
        resolutionDropdown.RefreshShownValue();
        Debug.Log(resolutionDropdown.value);

    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Music", volume);
    }

    public void SetSoundVolume(float volume)
    {
        audioMixer.SetFloat("Sound", volume);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("resolution", resolutionIndex);
        Debug.Log(resolutionIndex);
    }

    public void ClearSavedData()
    {
        PlayerPrefs.DeleteAll();
    }

    // Player Prefs
    public void TheFullScreenToggle() 
    {
    // Get the current state of our toggle button.
    int enable = FullScreenToggle.isOn ? 1 : 0;
   // Debug.Log(enable);
    // Set the PlayerPrefs equal to our current state.
    PlayerPrefs.SetInt("FullScreen", enable);
    }

    public void PlayUISound()
    {
        AudioManager.Instance.PlaySoundOneShot(1, 16, 1);
    }

}

