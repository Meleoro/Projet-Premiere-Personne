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

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider soundSlider;
    private GameObject player;
    private HealthComponent health;

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
        player = GameObject.Find("Player");
        health = player.GetComponent<HealthComponent>();

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

    }

    public void SetMasterVolume()
    {
        audioMixer.SetFloat("Master", (Mathf.Log10(masterSlider.value)*20));
    }
    
    public void SetMusicVolume()
    {
        audioMixer.SetFloat("Music", (Mathf.Log10(musicSlider.value)*20));
    }

    public void SetSoundVolume()
    {
        audioMixer.SetFloat("Effects", (Mathf.Log10(soundSlider.value)*20));
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
        AudioManager.Instance.PlaySoundOneShot(1, 16, 0);
    }

    public void TpToCheckpoint()
    {
        health.transform.position = health.lastCheckPoint;
    }

}

