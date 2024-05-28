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
        if(!startInside)
            AudioManager.Instance.PlaySoundContinuous(3, 1, 2);

        else
            AudioManager.Instance.PlaySoundContinuous(3, 2, 3);

        isInside = startInside;
    }

    public void GoInside()
    {
        if (isInside) return;
        isInside = true;

        AudioManager.Instance.FadeOutAudioSource(2, 2);
        AudioManager.Instance.PlaySoundOneShot(3, 2, 3);
    }

    public void GoOutside()
    {
        if (!isInside) return;
        isInside = false;

        AudioManager.Instance.PlaySoundFadingIn(2, 3, 1, 2);
    }
}
