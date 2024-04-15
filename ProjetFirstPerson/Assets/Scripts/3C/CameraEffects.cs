using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraEffects : GenericSingletonClass<CameraEffects>
{
    [Header("Hide Effect Paramaters")] 
    [SerializeField] private float hideEffectLerpDuration;
    
    [Header("References")] 
    [SerializeField] private Volume hiddenVolume;


    private float hideTimer;
    public IEnumerator Hide(float wantedValue)
    {
        float save = hiddenVolume.weight;
        hideTimer = 0;
        
        while (hideTimer < hideEffectLerpDuration)
        {
            hideTimer += Time.deltaTime;

            hiddenVolume.weight = Mathf.Lerp(save, wantedValue, hideTimer / hideEffectLerpDuration);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        hiddenVolume.weight = wantedValue;
    }
}
