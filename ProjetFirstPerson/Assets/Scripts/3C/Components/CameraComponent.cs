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
    [SerializeField] private bool doMoveFeel;
    [ShowIf("doMoveFeel")] [SerializeField] private bool doUpDownFeel;          // If true, the camera moves up and down when the character moves
    [ShowIf(EConditionOperator.And, "doMoveFeel", "doUpDownFeel")] [SerializeField] private float upDownFeelAmplitude = 0.05f;        // Contains how much the head will move
    [ShowIf(EConditionOperator.And, "doMoveFeel", "doUpDownFeel")] [SerializeField] private float upDownFeelDuration = 1;          // Contains how fast the head will move
    [ShowIf("doMoveFeel")] [SerializeField] private bool doLeftRightFeel;       // If true, the camera rotate slightly on the left and the right when the character moves
    [ShowIf(EConditionOperator.And, "doMoveFeel", "doLeftRightFeel")] [SerializeField] private float leftRightFeelDuration = 10;        // Contains how much the head will rotate
    [ShowIf(EConditionOperator.And, "doMoveFeel", "doLeftRightFeel")] [SerializeField] private float leftRightFeelAmplitude = 0.05f;       // Contains how fast the head will rotate
    
    [Header("Private Infos")] 
    private Vector2 inputDirection;
    private Vector2 currentRotation;
    private Vector2 lerpedRotation;
    private float moveFeelTimer;
    private bool moveFeelGoDown;
    private Coroutine upDownCoroutine;

    [Header("References")] 
    [SerializeField] private Transform wantedCameraPos;
    [SerializeField] private Transform characterCamera;

    

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    public void ComponentUpdate() { }

    public void ComponentFixedUpdate()
    {
        MoveCamera();
        OrientateCamera();
    }

    
    #region MAIN FUNCTIONS

    // MOVES THE CAMERA
    private void MoveCamera()
    {
        characterCamera.position = wantedCameraPos.position;
    }
    
    // CALLED FOR THE ORIENTATION OF THE CAMERA ACCORDING TO INPUTS AND SENSIBILITY
    private void OrientateCamera()
    {
        // First we actualise the rotation variables 
        inputDirection = new Vector2(Input.GetAxis("Mouse X") * Time.deltaTime * sensibilityX, Input.GetAxis("Mouse Y") * Time.deltaTime * sensibilityY);
        
        currentRotation.x -= inputDirection.y;
        currentRotation.y += inputDirection.x;

        currentRotation.x = Mathf.Clamp(currentRotation.x, -cameraYLimit, cameraYLimit);     // Avoids that the camera flips vertically

        // Then we apply these variables
        if (lerpCameraRotation)
        {
            lerpedRotation = Vector2.Lerp(lerpedRotation, currentRotation, lerpSpeed * Time.deltaTime);
            
            characterCamera.rotation = Quaternion.Euler(lerpedRotation.x, lerpedRotation.y, 0);
        }
        else
        {
            characterCamera.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0);
        }
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
    #endregion
    
}
