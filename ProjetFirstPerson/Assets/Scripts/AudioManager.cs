using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class AudioManager : GenericSingletonClass<AudioManager>
{
    [Header("Mixer")] 
    [SerializeField] private AudioMixer mixer;

    [Header("Effects")] 
    [SerializeField] private Vector2 minMaxLowPass;
    [SerializeField] private float lowPassEffectDuration = 1f;
    [SerializeField] private AnimationCurve lowPassCurve;
    
    [Header("Sounds")] 
    [SerializeField] private List<SoundCategory> soundList;
    [SerializeField] private int musicCategoryID;
    
    [Header("Audio Sources")] 
    [SerializeField] private List<AudioSource> audioSources;
    
    [Header("Volume")]
    [Range(0f, 1f)] [SerializeField] private float masterVolume;
    [Range(0f, 1f)] [SerializeField] private float musicVolume;
    [Range(0f, 1f)] [SerializeField] private float sfxVolume;

    private void Start()
    {
        CharacterManager.Instance.enterSneakZone += ApplySneakAudioEffect;
        CharacterManager.Instance.exitSneakZone += RemoveSneakAudioEffect;
    }

    private void ApplySneakAudioEffect()
    {
        mixer.DOSetFloat("_LowPass2", minMaxLowPass.x, lowPassEffectDuration).SetEase(lowPassCurve);
    }
    
    private void RemoveSneakAudioEffect()
    {
        mixer.DOSetFloat("_LowPass2", minMaxLowPass.y, lowPassEffectDuration).SetEase(lowPassCurve);
    }

    public void SetAudioSource(int index, AudioSource audioSource)
    {
        if (index > audioSources.Count)
            audioSources.Add(audioSource);

        else
            audioSources[index] = audioSource;
    }
    
    
    #region Play Sounds Functions

    /// <summary>
    /// WILL THE PLAY THE SOUND ONLY ONE TIME WITH NORMAL PITCH
    /// </summary>
    public void PlaySoundOneShot(int categoryId, int soundId, int audioSourceId = 0, AudioSource audioSource = null)
    {
        AudioSource currentAudioSource = audioSource is not null ? audioSource : audioSources[audioSourceId];
        currentAudioSource.PlayOneShot(soundList[categoryId].listSoundIdentities[soundId].audioClip, 
            soundList[categoryId].listSoundIdentities[soundId].volume * masterVolume * sfxVolume);
        
        currentAudioSource.volume = soundList[categoryId].listSoundIdentities[soundId].volume * masterVolume * sfxVolume;
    }
    
    /// <summary>
    /// WILL PLAY THE SOUND ENDLESSLY (used to play music)
    /// </summary>
    public void PlaySoundContinuous(int categoryId, int soundId, int audioSourceId = 0, AudioSource audioSource = null)
    {
        AudioSource currentAudioSource = audioSource is not null ? audioSource : audioSources[audioSourceId];
        currentAudioSource.clip = soundList[categoryId].listSoundIdentities[soundId].audioClip;
        
        if(musicCategoryID == categoryId)
            currentAudioSource.volume = soundList[categoryId].listSoundIdentities[soundId].volume * masterVolume * musicVolume;
        
        else
            currentAudioSource.volume = soundList[categoryId].listSoundIdentities[soundId].volume * masterVolume * sfxVolume;
        
        currentAudioSource.Play();
    }

    /// <summary>
    /// WILL PLAY THE SOUND ONE TIME AND WITH A RANDOM PITCH VARIATION
    /// </summary>
    public void PlaySoundOneShotRandomPitch(float pitchRangeMin, float pitchRangeMax, int soundId, int categoryId = 0, int audioSourceId = 0, AudioSource audioSource = null)
    {
        AudioSource currentAudioSource = audioSource is not null ? audioSource : audioSources[audioSourceId];
        currentAudioSource.pitch = Random.Range(pitchRangeMin, pitchRangeMax);
        currentAudioSource.clip = soundList[categoryId].listSoundIdentities[soundId].audioClip;
        currentAudioSource.pitch = 1;
    }


    public void PlaySoundFadingIn(float timeToFade, int categoryId, int soundId, int audioSourceId = 0, AudioSource audioSource = null)
    {
        PlaySoundContinuous(categoryId, soundId, audioSourceId, audioSource);
        
        AudioSource currentAudioSource = audioSource is not null ? audioSource : audioSources[audioSourceId];
        float wantedVolume = currentAudioSource.volume;
        currentAudioSource.volume = 0;
        StartCoroutine(FadeValue(0, wantedVolume, timeToFade, currentAudioSource));
    }
    
    public void FadeOutAudioSource(float timeToFade, int audioSourceId, AudioSource audioSource = null)
    {
        AudioSource currentAudioSource = audioSource is not null ? audioSource : audioSources[audioSourceId];
        StartCoroutine(FadeValue(currentAudioSource.volume, 0, timeToFade, currentAudioSource));
    }

    private IEnumerator FadeValue(float start, float wantedValue, float timeToFade, AudioSource audioSource)
    {
        float timer = 0;

        while (timer < timeToFade)
        {
            timer += Time.deltaTime;

            audioSource.volume = Mathf.Lerp(start, wantedValue, timer / timeToFade);
            
            yield return null;
        }
    }

    #endregion


    #region Volumes Functions

    public void SetMasterVolume(float newMasterVolume)
    {
        masterVolume = newMasterVolume;
    }
    
    public void SetSfxVolume(float newSfxVolume)
    {
        sfxVolume = newSfxVolume;
    }
    
    public void SetMusicVolume(float newMusicVolume)
    {
        musicVolume = newMusicVolume;
    }

    #endregion
    
}

[Serializable]
public class SoundCategory
{
    public List<SoundIdentity> listSoundIdentities;
}

[Serializable]
public class SoundIdentity
{
    public AudioClip audioClip;
    [Range(0, 5)] public float volume = 1;
}
