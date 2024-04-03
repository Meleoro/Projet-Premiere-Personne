using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MoveComponent))]
public class NoiseComponent : MonoBehaviour, ICharacterComponent
{
    [Header("References")] 
    private MoveComponent moveScript;
    private CharacterManager mainScript;


    private void Start()
    {
        mainScript = GetComponent<CharacterManager>();
        moveScript = GetComponent<MoveComponent>();
    }


    public void ComponentUpdate()
    {
        if (moveScript.currentVelocity.magnitude < 1)
        {
            mainScript.currentNoiseType = NoiseType.Quiet;
        }
        
        else if (moveScript.isRunning)
        {
            mainScript.currentNoiseType = NoiseType.Loud;
        }
        
        else if (moveScript.isCrouching)
        {
            mainScript.currentNoiseType = NoiseType.Quiet;
        }

        else
        {
            mainScript.currentNoiseType = NoiseType.Normal;
        }
    }

    public void ComponentFixedUpdate()
    {
    }

    public void ComponentLateUpdate()
    {
    }
}


public enum NoiseType
{
    Loud,
    Normal,
    Quiet
}
