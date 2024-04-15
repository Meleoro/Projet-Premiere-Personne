using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoveComponent), typeof(NoiseComponent))]
public class CrouchComponent : MonoBehaviour, ICharacterComponent
{
    [Header("Crouch Parameters")]
    [SerializeField] private float crouchSpeedModifier = 0.7f;
    [SerializeField] private float cameraPosYModifier = -0.4f;
    [SerializeField] private float crouchCameraMoveDuration = 0.3f;

    [Header("Private Infos")]
    [HideInInspector] public bool isCrouched;

    [Header("References")] 
    [SerializeField] private Collider normalCollider;
    [SerializeField] private Collider crouchedCollider;
    private Controls controls;
    private MoveComponent moveScript;
    private CameraComponent cameraScript;
    
    
    public void Awake()
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
    

    #region Interface Functions

    public void ComponentUpdate()
    {
        if (controls.Player.Crouch.WasPerformedThisFrame() && !CameraManager.Instance.isCrouching)
        {
            if (isCrouched) GetUp();
            else Crouch();
        }
    }

    public void ComponentFixedUpdate()
    {
    }

    public void ComponentLateUpdate()
    {
    }

    #endregion


    #region Main Functions

    private void Crouch()
    {
        isCrouched = true;

        normalCollider.enabled = false;
        crouchedCollider.enabled = true;
        
        moveScript.currentSpeedModifier = crouchSpeedModifier;
        StartCoroutine(cameraScript.Crouch(cameraPosYModifier, crouchCameraMoveDuration));
    }

    private void GetUp()
    {
        isCrouched = false;
        
        normalCollider.enabled = true;
        crouchedCollider.enabled = false;
        
        moveScript.currentSpeedModifier = 1;
        StartCoroutine(cameraScript.Crouch(0, crouchCameraMoveDuration));
    }

    #endregion
}
