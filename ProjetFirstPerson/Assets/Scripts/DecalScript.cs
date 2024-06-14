using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalScript : MonoBehaviour
{
    [Header("Parameters")] 
    [SerializeField] private float timeBeforeFadeOut;
    [SerializeField] private float fadeOutDuration;

    [Header("References")] 
    private DecalProjector decal;
    
    
    private void Start()
    {
        decal = GetComponent<DecalProjector>();
        decal.material = new Material(decal.material);
        decal.material.SetFloat("_FadeOut0", 1);

        StartCoroutine(ManageFadeOut());
    }

    private IEnumerator ManageFadeOut()
    {
        yield return new WaitForSeconds(timeBeforeFadeOut);

        float timer = 0;

        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;

            decal.material.SetFloat("_FadeOut0", Mathf.Lerp(1, 0, timer / fadeOutDuration));
            
            yield return null;
        }
        
        Destroy(gameObject);
    }
}
