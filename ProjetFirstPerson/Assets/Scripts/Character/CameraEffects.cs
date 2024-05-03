using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraEffects : GenericSingletonClass<CameraEffects>
{
    [Header("Hide Effect Paramaters")] 
    [SerializeField] private float hideEffectLerpDuration;
    
    [Header("Damage Effect Paramaters")] 
    [SerializeField] private float damageEffectDuration;
    
    [Header("References")] 
    [SerializeField] private Volume hiddenVolume;
    [SerializeField] private Volume healthVolume;


    private float hideTimer;
    public IEnumerator Hide(float wantedValue)
    {
        float save = hiddenVolume.weight;
        hideTimer = 0;
        
        while (hideTimer < hideEffectLerpDuration)
        {
            hideTimer += Time.deltaTime;

            hiddenVolume.weight = Mathf.Lerp(save, wantedValue, hideTimer / hideEffectLerpDuration);
            yield return null;
        }

        hiddenVolume.weight = wantedValue;
    }
    
    
    private float healthTimer;
    public IEnumerator TakeDamage(float wantedValue)
    {
        healthTimer = 0;
        while (healthTimer < damageEffectDuration * 0.2f)
        {
            healthTimer += Time.deltaTime;

            healthVolume.weight = Mathf.Lerp(0, wantedValue, healthTimer / (damageEffectDuration * 0.2f));
            yield return null;
        }
        
        healthVolume.weight = wantedValue;
        
        healthTimer = 0;
        while (healthTimer < damageEffectDuration)
        {
            healthTimer += Time.deltaTime;

            healthVolume.weight = Mathf.Lerp(wantedValue, 0, healthTimer / damageEffectDuration);
            yield return null;
        }

        healthVolume.weight = 0;
    }

    private float fadeScreenTimer;
    public IEnumerator FadeScreen(float fadeDuration, float wantedValue)
    {
        fadeScreenTimer = 0;
        float saveFadeValue = UIManager.Instance.fadeImage.color.a;

        while (fadeScreenTimer < fadeDuration)
        {
            fadeScreenTimer += Time.deltaTime;

            UIManager.Instance.fadeImage.color = new Color(UIManager.Instance.fadeImage.color.r,
                UIManager.Instance.fadeImage.color.g, UIManager.Instance.fadeImage.color.b, 
                Mathf.Lerp(saveFadeValue, wantedValue, fadeScreenTimer / fadeDuration));

            yield return null;
        }
    }
}
