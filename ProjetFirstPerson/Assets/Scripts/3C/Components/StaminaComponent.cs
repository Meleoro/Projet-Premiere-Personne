using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

[RequireComponent(typeof(MoveComponent), typeof(CharacterManager))]
public class StaminaComponent : MonoBehaviour, ICharacterComponent
{
    [Header("Parameters")] 
    [SerializeField] private float staminaAmount;
    [SerializeField] private float staminaLoseSpeed;
    [SerializeField] private float staminaRegainSpeed;
    [SerializeField] private float timeBeforeRegain;
    [Range(0f, 1f)] [SerializeField] private float tiredVolumeRatioAppear;      // From what percentage of the stamina amount the tired volume starts to appear

    [Header("Public Infos")]
    public bool hasStamina;
    
    [Header("Private Infos")] 
    private float currentStamina;
    private float regainTimer;
    private bool wasRunning;

    [Header("References")] 
    private MoveComponent moveScript;


    private void Start()
    {
        moveScript = GetComponent<MoveComponent>();
        GetComponent<CharacterManager>().UseAdrenaline += UseAdrenaline;

        currentStamina = staminaAmount;
    }


    public void ComponentUpdate()
    {
        if (moveScript.isRunning)
        {
            wasRunning = true;
            currentStamina -= Time.deltaTime * staminaLoseSpeed;
        }
        else
        {
            if (wasRunning)
            {
                wasRunning = false;
                regainTimer = timeBeforeRegain;
            }

            if (regainTimer <= 0)
            {
                if (currentStamina < staminaAmount)
                    currentStamina += Time.deltaTime * staminaRegainSpeed;
            }
            else
            {
                regainTimer -= Time.deltaTime;
            }
        }
        
        if (currentStamina > 0) hasStamina = true;
        else hasStamina = false;
        
        ApplyVolume();
    }

    public void ComponentFixedUpdate()
    {
        
    }

    public void ComponentLateUpdate()
    {
        
    }


    private float staminaLerpValue;
    private void ApplyVolume()
    {
        if (currentStamina < staminaAmount * tiredVolumeRatioAppear)
        {
            float currentRatio = 1 - currentStamina / (staminaAmount * tiredVolumeRatioAppear);

            staminaLerpValue = Mathf.Lerp(staminaLerpValue, currentRatio, Time.deltaTime * 5);
            VolumeManager.Instance.staminaVolume.weight = staminaLerpValue;
        }
        else
        {
            staminaLerpValue = Mathf.Lerp(staminaLerpValue, 0, Time.deltaTime * 5);
            VolumeManager.Instance.staminaVolume.weight = 0;
        }
    }
    
    private void UseAdrenaline(ItemData staminaData)
    {
        if (staminaData.rechargeStamina)
            currentStamina = staminaAmount;
    }
}
