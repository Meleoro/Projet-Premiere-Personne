using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(MoveComponent))]
public class TiltComponent : MonoBehaviour, ICharacterComponent
{
    [Header("Parameters")] 
    [SerializeField] private float maxTilt;
    [SerializeField] private float maxTiltRot;
    [SerializeField] private float sensibilityX;
    [SerializeField] private float sensibilityY;
    
    [Header("Private Infos")] 
    private Vector3 saveOriginalPos;
    private Vector3 currentTiltValue;
    private Vector3 currentTiltRotValue;
    private Vector3 inputDir;
    private Vector3 localInputDir;
    private bool isTilting;
    
    [Header("References")] 
    private Controls controls;
    private MoveComponent moveScript;
    private CameraComponent cameraScript;


    private void Awake()
    {
        controls = new Controls();
        moveScript = GetComponent<MoveComponent>();
        cameraScript = GetComponent<CameraComponent>();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
    


    public void ComponentUpdate()
    {
        if (controls.Player.Tilt.IsPressed())
        {
            if(!isTilting) StartTilt();
            Tilt();
        }

        else
        {
            if (isTilting) EndTilt();
        }
    }

    public void ComponentFixedUpdate()
    {
    }

    public void ComponentLateUpdate()
    {
    }


    private void StartTilt()
    {
        isTilting = true;
        cameraScript.StartTilting();
    }

    private void EndTilt()
    {
        isTilting = false;
        cameraScript.StopTilting();

        currentTiltValue = Vector3.zero;
        currentTiltRotValue = Vector3.zero;
        
        cameraScript.tiltPosAddedPos = currentTiltValue;
        cameraScript.tiltRotAddedPos = currentTiltRotValue;
    }
    
    
    private void Tilt()
    {
        inputDir = new Vector2(Mouse.current.delta.x.ReadValue() * sensibilityX * 0.01f, Mouse.current.delta.y.ReadValue() * sensibilityY * 0.01f);
        localInputDir = cameraScript.characterCamera.TransformDirection(inputDir);

        currentTiltValue += localInputDir;
        currentTiltValue = Vector3.ClampMagnitude(currentTiltValue, maxTilt);
        
        currentTiltRotValue = new Vector3(0, 0, Mathf.Lerp(maxTiltRot, -maxTiltRot, 0.5f + cameraScript.characterCamera.InverseTransformDirection(currentTiltValue).x / maxTilt));
        
        cameraScript.tiltPosAddedPos = currentTiltValue;
        cameraScript.tiltRotAddedPos = currentTiltRotValue;
    }
}