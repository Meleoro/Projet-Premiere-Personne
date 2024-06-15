using System.Collections;
using System.Collections.Generic;
using ArthurUtilities;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraEffects : GenericSingletonClass<CameraEffects>
{
    [Header("Hide Effect Paramaters")] 
    [SerializeField] private float hideEffectLerpDuration;
    
    [Header("Damage Effect Paramaters")] 
    [SerializeField] private float damageEffectDuration;
    
    [Header("Hurt Effect Paramaters")] 
    [SerializeField] private float flickerSpeed;
    [SerializeField] [Range(0f, 1f)] private float flickerMinValue;
    
    [Header("References")]
    public Volume hiddenVolume;
    [SerializeField] private Volume healthVolume;
    public Volume hurtVolume;


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


    private float hurtTimer;
    private float secondaryTimer;
    private bool goUp;
    public IEnumerator HurtEffect(float duration)
    {
        hurtVolume.weight = 1;
        
        hurtTimer = 0;
        secondaryTimer = 1;
        goUp = false;
        while(hurtTimer < duration * 0.9f)
        {
            hurtTimer += Time.deltaTime;
            secondaryTimer += goUp ? Time.deltaTime * flickerSpeed : -Time.deltaTime * flickerSpeed;

            hurtVolume.weight = Mathf.Lerp(flickerMinValue, 1, secondaryTimer);

            if (secondaryTimer < 0)
                goUp = true;
            
            else if (secondaryTimer > 1)
                goUp = false;

            yield return null;
        }

        float saveWeight = hurtVolume.weight;
        hurtTimer = 0;
        while(hurtTimer < duration * 0.1f)
        {
            hurtTimer += Time.deltaTime;

            hurtVolume.weight = Mathf.Lerp(saveWeight, 0, hurtTimer / (duration * 0.1f));

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
