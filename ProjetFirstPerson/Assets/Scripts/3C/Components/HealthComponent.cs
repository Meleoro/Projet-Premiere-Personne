using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour, ICharacterComponent
{
    [Header("Parameters")] 
    [SerializeField] private int startHealth;

    [Header("Private Infos")] 
    private int currentHealth;


    private void Start()
    {
        currentHealth = startHealth;
    }


    public void ComponentUpdate()
    {
        
    }

    public void ComponentFixedUpdate()
    {
    }

    public void ComponentLateUpdate()
    {
    }


    public void TakeDamage()
    {
        currentHealth -= 1;
        
        if(currentHealth <= 0)
            Debug.Log("Dead");

        StartCoroutine(CameraEffects.Instance.TakeDamage(0.8f));
    }
}
