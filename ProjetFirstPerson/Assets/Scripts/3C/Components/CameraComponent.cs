using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraComponent : MonoBehaviour, ICharacterComponent
{
    [Header("Main Parameters")] 
    [SerializeField] private float sensibilityX;
    [SerializeField] private float sensibilityY;
    [SerializeField] private float cameraYLimit = 90;          // Used to avoid that the camera flips vertically
    
    [Header("Lerp Parameters")]
    [SerializeField] private bool lerpCameraRotation;         // If true, the camera rotation when the camera rotates it will not be immediate
    [ShowIf("lerpCameraRotation")] [SerializeField] private float lerpSpeed = 10;
    
    [Header("Move Up Down Feel Parameters")]
    public bool doMoveFeel;
    [ShowIf("doMoveFeel")] [SerializeField] private bool doUpDownFeel;          // If true, the camera moves up and down when the character moves
    [ShowIf(EConditionOperator.And, "doMoveFeel", "doUpDownFeel")] [SerializeField] private float upDownFeelAmplitude = 0.05f;        // Contains how much the head will move
    [ShowIf(EConditionOperator.And, "doMoveFeel", "doUpDownFeel")] [SerializeField] private float upDownFeelDuration = 1;          // Contains how fast the head will move
    [ShowIf("doMoveFeel")] [SerializeField] private bool doLeftRightFeel;       // If true, the camera rotate slightly on the left and the right when the character moves
    [ShowIf(EConditionOperator.And, "doMoveFeel", "doLeftRightFeel")] [SerializeField] private float leftRightFeelDuration = 10;        // Contains how much the head will rotate
    [ShowIf(EConditionOperator.And, "doMoveFeel", "doLeftRightFeel")] [SerializeField] private float leftRightFeelAmplitude = 0.05f;       // Contains how fast the head will rotate

    [Header("FOV Parameters")] 
    public bool doFOVEffect;
    [ShowIf("doFOVEffect")] [SerializeField] private float addedFOVMax = 5;
    [ShowIf("doFOVEffect")] [SerializeField] private float FOVLerpSpeed = 6;

    [Header("Flags")] 
    public bool canRotate;
    public bool canMove;

    [Header("Public infos")] 
    [HideInInspector] public bool isCrouching;
    
    [Header("Private Infos")] 
    private Vector2 inputDirection;
    private Vector2 currentRotation;
    private Vector2 lerpedRotation;
    private float moveFeelTimer;
    private bool moveFeelGoDown;
    private Coroutine upDownCoroutine;
    private float crouchModifierY;
    
    [Header("References")] 
    public Transform wantedCameraPos;
    [SerializeField] private Transform characterCamera;
    
    

    private void Start()
    {
        LockedCursor(2);
    }

    public void LockedCursor(int value)
    {
        if(value == 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if(value == 1)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else if(value == 2)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void ComponentUpdate()
    {
        //Application.targetFrameRate = 60;
    }

    public void ComponentFixedUpdate()
    {
        /*MoveCamera();
        RotateCamera();*/
    }

    public void ComponentLateUpdate()
    {
        ActualiseInputs();
        
        if(canMove)
            MoveCamera();
        
        if(canRotate)
            RotateCamera();
    }


    #region MAIN FUNCTIONS
    
    // MOVES THE CAMERA
    private void MoveCamera()
    {
        characterCamera.position = Vector3.Lerp(characterCamera.position, wantedCameraPos.position + new Vector3(0, crouchModifierY, 0), Time.deltaTime * 50);
    }

    // ROTATES THE CAMERA
    private void RotateCamera()
    {
        if (lerpCameraRotation)
        {
            lerpedRotation = Vector2.Lerp(lerpedRotation, currentRotation, lerpSpeed * Time.deltaTime);
            
            characterCamera.rotation = Quaternion.Euler(lerpedRotation.x, lerpedRotation.y, 0);
            wantedCameraPos.rotation = Quaternion.Euler(0, lerpedRotation.y, 0);
        }
        else
        {
            characterCamera.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0);
            wantedCameraPos.rotation = Quaternion.Euler(0, lerpedRotation.y, 0);
        }
    }
    
    
    // CALLED FOR THE ACTUALISE THE PLAYER INPUTS
    private void ActualiseInputs()
    {
        inputDirection = new Vector2(Mouse.current.delta.x.ReadValue() * sensibilityX, Mouse.current.delta.y.ReadValue() * sensibilityY);
        
        currentRotation.x -= inputDirection.y;
        currentRotation.y += inputDirection.x;

        currentRotation.x = Mathf.Clamp(currentRotation.x, -cameraYLimit, cameraYLimit);     // Avoids that the camera flips vertically
    }

    #endregion

    
    #region FEEL FUNCTIONS

    /// <summary>
    /// CALLED TO DO COOL EFFECTS WHEN THE CHARACTER MOVES, THIS FUNCTION IS CALLED FROM THE MOVEMENT SCRIPT
    /// </summary>
    /// <param name="currentModifier"> MODIFIER THAT DEPENDS OF THE CURRENT SPEED OF THE PLAYER </param>
    public void DoMoveFeel(float currentModifier)
    {
        if (doMoveFeel)
        {
            if (doUpDownFeel)
            {
                if (currentModifier > 0.2f)
                {
                    CameraManager.Instance.DoMoveHEadUpDownEffect(upDownFeelDuration * (1.5f - currentModifier), upDownFeelAmplitude * currentModifier);
                }
            }

            if (doLeftRightFeel)
            {
                if (currentModifier > 0.2f)
                {
                    CameraManager.Instance.DoMoveHEadLeftRightEffect(leftRightFeelDuration * (1.5f - currentModifier), leftRightFeelAmplitude * currentModifier);
                }
            }
        }
    }

    /// <summary>
    /// CALLED TO MODIFY THE FOV OF THE CAMERA FROM THE MOVEMENT SPEED
    /// </summary>
    /// <param name="currentModifier"> MODIFIER THAT DEPENDS OF THE CURRENT SPEED OF THE PLAYER </param>
    public void ModifyFOV(float currentModifier)
    {
        if (doFOVEffect)
        {
            CameraManager.Instance.ChangeFOV(addedFOVMax * currentModifier, FOVLerpSpeed);
        }
    }
    
    #endregion
    
    
    #region Crouch Functions

    private float crouchTimer;
    public IEnumerator Crouch(float YModifier, float crouchDuration)
    {
        crouchTimer = 0;
        isCrouching = true;
        float save = crouchModifierY;
        
        while (crouchTimer < crouchDuration)
        {
            crouchTimer += Time.deltaTime;

            crouchModifierY = Mathf.Lerp(save, YModifier, crouchTimer / crouchDuration);
            
            yield return new WaitForSeconds(Time.deltaTime);
        }
        
        isCrouching = false;
    }
    #endregion
}
