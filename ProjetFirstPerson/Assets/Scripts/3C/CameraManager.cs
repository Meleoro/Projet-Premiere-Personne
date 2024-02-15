using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : GenericSingletonClass<CameraManager>
{
    [Header("Private Infos")] 
    private float originalYSave;

    [Header("Private Infos")] 
    private bool isDoingUpDownEffect;
    private float currentUpDownDuration;
    private float currentUpDownAmplitude;
    private Vector3 wantedLocalPosition;
    private bool isDoingLeftRightEffect;
    private float currentLeftRightDuration;
    private float currentLeftRightAmplitude;
    private Vector3 wantedLocalRotation;
    
    [Header("References")] 
    [SerializeField] private Transform parentTransform;


    private void Start()
    {
        originalYSave = transform.localPosition.y;
    }


    private void FixedUpdate()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, wantedLocalPosition, Time.fixedDeltaTime * 30);
        transform.localRotation = Quaternion.Euler(new Vector3(wantedLocalRotation.x, wantedLocalRotation.y, Mathf.Lerp(transform.localRotation.z, wantedLocalRotation.z, Time.fixedDeltaTime * 30)));
    }


    #region UP DOWN FUNCTIONS

    /// <summary>
    /// CALLED TO LAUNCH THE HEAD UP DOWN EFFECT MOVEMENT
    /// </summary>
    public void DoMoveHEadUpDownEffect(float duration, float amplitude)
    {
        // First we actualise the duration and the amplitude of the movement
        currentUpDownDuration = duration;
        currentUpDownAmplitude = amplitude;
        
        // Then if the head is not already moving we move it
        if (!isDoingUpDownEffect)
        {
            isDoingUpDownEffect = true;
            
            StartCoroutine(MoveHeadUpDownCoroutine());
        }
    }

    // COROUTINE WHICH DOES THE CURRENT MOVEMENT OF THE HEAD 
    private IEnumerator MoveHeadUpDownCoroutine()
    {
        float timer = 0;

        while (timer < currentUpDownDuration)
        {
            timer += Time.deltaTime;
            
            wantedLocalPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(originalYSave,
                originalYSave + currentUpDownAmplitude, timer / currentUpDownDuration), transform.localPosition.z);
            
            yield return new WaitForSeconds(Time.deltaTime);
        }
        
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            
            wantedLocalPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(originalYSave - currentUpDownAmplitude,
                originalYSave + currentUpDownAmplitude, timer / currentUpDownDuration), transform.localPosition.z);
            
            yield return new WaitForSeconds(Time.deltaTime);
        }
        
        while (timer < currentUpDownDuration)
        {
            timer += Time.deltaTime;
            
            wantedLocalPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(originalYSave - currentUpDownAmplitude,
                originalYSave, timer / currentUpDownDuration), transform.localPosition.z);
            
            yield return new WaitForSeconds(Time.deltaTime);
        }

        isDoingUpDownEffect = false;
    }

    #endregion


    #region LEFT RIGHT FUNCTIONS

    /// <summary>
    /// CALLED TO LAUNCH THE HEAD LEFT RIGHT EFFECT MOVEMENT 
    /// </summary>
    public void DoMoveHEadLeftRightEffect(float duration, float amplitude)
    {
        // First we actualise the duration and the amplitude of the movement
        currentLeftRightDuration = duration;
        currentLeftRightAmplitude = amplitude;
        
        // Then if the head is not already moving we move it
        if (!isDoingLeftRightEffect)
        {
            isDoingLeftRightEffect = true;
            
            StartCoroutine(MoveHeadLeftRightCoroutine());
        }
    }

    // COROUTINE WHICH DOES THE CURRENT MOVEMENT OF THE HEAD 
    private IEnumerator MoveHeadLeftRightCoroutine()
    {
        float timer = 0;

        while (timer < currentLeftRightDuration)
        {
            timer += Time.deltaTime * 2;
            
            wantedLocalRotation = new Vector3(transform.localRotation.eulerAngles.x, 
                transform.localRotation.eulerAngles.y, Mathf.Lerp(0, currentLeftRightAmplitude, timer / currentLeftRightDuration));
            
            yield return new WaitForSeconds(Time.deltaTime);
        }

        yield return new WaitForSeconds(currentLeftRightDuration * 0.1f);
        
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            
            wantedLocalRotation = new Vector3(transform.localRotation.eulerAngles.x, 
                transform.localRotation.eulerAngles.y, Mathf.Lerp(-currentLeftRightAmplitude, currentLeftRightAmplitude, timer / currentLeftRightDuration));
            
            yield return new WaitForSeconds(Time.deltaTime);
        }
        
        yield return new WaitForSeconds(currentLeftRightDuration * 0.1f);
        
        while (timer < currentLeftRightDuration)
        {
            timer += Time.deltaTime * 2;
            
            wantedLocalRotation = new Vector3(transform.localRotation.eulerAngles.x, 
                transform.localRotation.eulerAngles.y, Mathf.Lerp(-currentLeftRightAmplitude, 0, timer / currentLeftRightDuration));
            
            yield return new WaitForSeconds(Time.deltaTime);
        }

        isDoingLeftRightEffect = false;
    }

    #endregion
    
}
