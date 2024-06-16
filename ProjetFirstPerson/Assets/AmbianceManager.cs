using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbianceManager : GenericSingletonClass<AmbianceManager>
{
    [Header("Parameters")]
    [SerializeField] private bool startInside;

    [Header("Private Infos")]
    private bool isInside;
    


    private void Start()
    {
        if (!startInside)
        {
            AudioManager.Instance.PlaySoundContinuous(3, 1, 2);
            AudioManager.Instance.PlaySoundContinuous(3, 5, 4);
        }

        else
        {
            AudioManager.Instance.PlaySoundContinuous(3, 2, 3);
            AudioManager.Instance.PlaySoundContinuous(3, 6, 5);
        }

        isInside = startInside;
    }

    public void GoInside()
    {
        if (isInside) return;
        isInside = true;

        AudioManager.Instance.FadeOutAudioSource(2, 2);
        AudioManager.Instance.PlaySoundOneShot(3, 2, 3);
        
        AudioManager.Instance.FadeOutAudioSource(2, 4);
        AudioManager.Instance.FadeOutAudioSource(2, 6);
        AudioManager.Instance.PlaySoundFadingIn(2, 3, 6, 5);
    }

    public void GoOutside()
    {
        if (!isInside) return;
        isInside = false;

        AudioManager.Instance.PlaySoundFadingIn(2, 3, 1, 2);
        
        AudioManager.Instance.FadeOutAudioSource(2, 5);
        AudioManager.Instance.FadeOutAudioSource(2, 6);
        AudioManager.Instance.PlaySoundFadingIn(2, 3, 5, 4);
    }

    public void StartEndPoursuite()
    {
        AudioManager.Instance.FadeOutAudioSource(1, 4);
        AudioManager.Instance.FadeOutAudioSource(1, 5);
        
        AudioManager.Instance.PlaySoundFadingIn(2, 3, 7, 6);
    }

    public void PlayCredits()
    {
        AudioManager.Instance.FadeOutAudioSource(1, 4);
        AudioManager.Instance.FadeOutAudioSource(1, 5);
        AudioManager.Instance.FadeOutAudioSource(1, 6);
        
        AudioManager.Instance.PlaySoundFadingIn(2, 3, 8, 7);
    }
}
