using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    private void ApplyVolume()
    {
        if (currentStamina < staminaAmount * tiredVolumeRatioAppear)
        {
            float currentRatio = 1 - currentStamina / (staminaAmount * tiredVolumeRatioAppear);
            VolumeManager.Instance.staminaVolume.weight = currentRatio;
        }
        else
        {
            VolumeManager.Instance.staminaVolume.weight = 0;
        }
    }
    
    private void UseAdrenaline(ItemData staminaData)
    {
        
    }
}
