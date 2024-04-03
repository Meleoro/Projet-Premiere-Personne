using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

public class MoveComponent : MonoBehaviour, ICharacterComponent
{
    [Header("Main Parameters")] 
    [SerializeField] private float walkSpeed;
    [SerializeField] private float walkAcceleration;

    [Header("Run Parameters")] 
    [SerializeField] private bool canRun;
    [ShowIf("canRun")] [SerializeField] private float runSpeed;
    [ShowIf("canRun")] [SerializeField] private float runAcceleration;

    [Header("Crouch Parameters")] 
    [SerializeField] private bool canCrouch;
    [ShowIf("canCrouch")] [SerializeField] private float crouchSpeed;
    [ShowIf("canCrouch")] [SerializeField] private float crouchAcceleration;

    [Header("Public Infos")] 
    public bool isRunning;
    public bool isCrouching;
    public Vector3 currentVelocity;
    
    [Header("Private Infos")]
    private Vector3 inputDirection;
    private float currentSpeed;
    private float currentAcceleration;
    private float addedSpeed;

    [Header("References")] 
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform wantedCamPos;
    private Controls controls;
    private CameraComponent cameraComponent;
    private StaminaComponent staminaComponent;


    private void Awake()
    {
        controls = new Controls();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }


    private void Start()
    {
        cameraComponent = GetComponent<CameraComponent>();
        staminaComponent = GetComponent<StaminaComponent>();
        
        GetComponent<CharacterManager>().UseAdrenaline += UseAdrenaline;
        
        if (!rb)
        {
            Debug.LogError("Il manque la référence du rigidbody sur le script de movement");
        }
    }


    public void ComponentUpdate()
    {
        ManageInputs();
        MoveCharacter();
    }

    public void ComponentFixedUpdate()
    {
        
    }

    public void ComponentLateUpdate()
    {
        
    }


    // ACTUALISE THE MOVE DIRECTION INPUT AND THE CURRENT SPEED ACCORDING TO IF THE PLAYER WANTS TO RUN OR NOT
    private void ManageInputs()
    {
        // First we take the wanted move direction and transform it according to the camera rotation 
        inputDirection = controls.Player.MoveAxis.ReadValue<Vector2>();
        inputDirection = wantedCamPos.TransformDirection(new Vector3(inputDirection.x, 0, inputDirection.y));
        
        // Then we look if the player can and wants to run to actualise the current speed
        currentAcceleration = walkAcceleration;
        currentSpeed = walkSpeed;

        if (staminaComponent)
        {
            if (!staminaComponent.hasStamina)
            {
                currentAcceleration = walkAcceleration;
                currentSpeed = walkSpeed;
                
                return;
            }
        }
        
        if (canRun)
        {
            if (controls.Player.Run.IsPressed())
            {
                currentAcceleration = runAcceleration;
                currentSpeed = runSpeed;
            }
        }
    }
    
    // MOVES THE THE PLAYER IN THE WORLD SPACE
    private void MoveCharacter()
    {
        rb.AddForce(inputDirection * (Time.deltaTime * currentAcceleration), ForceMode.Force);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, currentSpeed + addedSpeed);
        
        // We apply the feel to the camera according to our current speed
        cameraComponent.DoMoveFeel(rb.velocity.magnitude / (runSpeed + addedSpeed));
        cameraComponent.ModifyFOV(rb.velocity.magnitude / (runSpeed + addedSpeed));

        // Run bool for stamina script
        if (rb.velocity.magnitude >= runSpeed - 0.1f)
            isRunning = true;
        
        else
            isRunning = false;

        currentVelocity = rb.velocity;
    }

    
    private void UseAdrenaline(ItemData adrenalineData)
    {
        addedSpeed = adrenalineData.speedIncrease;
        StartCoroutine(AdrenalineCoroutine(adrenalineData.effectDuration));
    }

    private IEnumerator AdrenalineCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);

        addedSpeed = 0;
    }
}
