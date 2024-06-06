using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Random = UnityEngine.Random;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

public class MoveComponent : MonoBehaviour, ICharacterComponent
{
    [Header("Main Parameters")] 
    [SerializeField] private float walkSpeed;
    [SerializeField] private float walkAcceleration;
    public bool canMove;
    
    [Header("Run Parameters")] 
    [SerializeField] private bool canRun;
    [ShowIf("canRun")] [SerializeField] private float runSpeed;
    [ShowIf("canRun")] [SerializeField] private float runAcceleration;

    [Header("Gravity Parameters")] 
    [SerializeField] private float gravityStrength;
    [SerializeField] private float slopHelpStrength;
    [SerializeField] private float groundRaycastDist;

    [Header("Sound Parameters")]
    [SerializeField] private float crouchSoundDuration;
    [SerializeField] private float runSoundDuration;

    [Header("Public Infos")] 
    [HideInInspector] public bool isRunning;
    [HideInInspector] public bool disableRun;
    [HideInInspector] public float currentSpeedModifier;
    [HideInInspector] public Vector3 currentVelocity;
    
    [Header("Private Infos")]
    private Vector3 inputDirection;
    private float currentSpeed;
    private float currentAcceleration;
    private float addedSpeed;
    private bool isOnGround;
    private bool isInSlope;
    private float walkSoundTimer;
    private float walkSoundDuration;

    [Header("References")] 
    public Rigidbody rb;
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

        currentSpeedModifier = 1;
        
        GetComponent<CharacterManager>().UseAdrenaline += UseAdrenaline;
        
        if (!rb)
        {
            Debug.LogError("Il manque la référence du rigidbody sur le script de movement");
        }
    }

    
    #region Interface Functions

    public void ComponentUpdate()
    {
        ManageInputs();

        if (canMove)
        {
            MoveCharacter();

            ManageWalkSound();
        }
    }

    public void ComponentFixedUpdate()
    {
        GravityUpdate();
    }

    public void ComponentLateUpdate()
    {
        
    }

    #endregion

    
    #region Base Movement Functions

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
        
        if (canRun && !disableRun)
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
        rb.AddForce(inputDirection * (Time.deltaTime * currentAcceleration * currentSpeedModifier), ForceMode.Force);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, (currentSpeed + addedSpeed) * currentSpeedModifier);

        if(inputDirection == Vector3.zero && rb.velocity.magnitude > 0.1f)
            rb.AddForce(-rb.velocity.normalized * (Time.deltaTime * currentAcceleration * currentSpeedModifier), ForceMode.Force);

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

    #endregion


    #region Gravity Functions

    private void GravityUpdate()
    {
        VerifyCurrentState();
        
        if(!isOnGround) ApplyGravity();
        if(isInSlope) HelpInSlope();
    }

    private bool quitGround;
    private float YQuitGround;
    private void VerifyCurrentState()
    {
        RaycastHit hitInfo;

        isOnGround = Physics.Raycast(transform.position, Vector3.down, out hitInfo, groundRaycastDist,LayerManager.Instance.groundLayer);

        float dist = 0.23f;
        if(!isOnGround)
            isOnGround = Physics.Raycast(transform.position + new Vector3(dist, 0, 0), Vector3.down, out hitInfo, groundRaycastDist, LayerManager.Instance.groundLayer);

        if (!isOnGround)
            isOnGround = Physics.Raycast(transform.position + new Vector3(-dist, 0, 0), Vector3.down, out hitInfo, groundRaycastDist, LayerManager.Instance.groundLayer);

        if (!isOnGround)
            isOnGround = Physics.Raycast(transform.position + new Vector3(0, 0, dist), Vector3.down, out hitInfo, groundRaycastDist, LayerManager.Instance.groundLayer);

        if (!isOnGround)
            isOnGround = Physics.Raycast(transform.position + new Vector3(0, 0, -dist), Vector3.down, out hitInfo, groundRaycastDist, LayerManager.Instance.groundLayer);


        //isOnGround = Physics.BoxCast(transform.position, gf, );

        if (isOnGround)
        {
            if (quitGround)
            {
                quitGround = false;
                GetComponent<HealthComponent>().VerifyFall(Mathf.Abs(YQuitGround - transform.position.y));
            }

            currentGravity = 0;
            isInSlope = Vector3.Angle(hitInfo.normal, Vector3.up) > 20;
        }
        else
        {
            quitGround = true;
            YQuitGround = transform.position.y;
        }
    }

    private float currentGravity = 0;
    private void ApplyGravity()
    {
        currentGravity += Time.fixedDeltaTime * gravityStrength;

        rb.AddForce(Vector3.down * currentGravity, ForceMode.Acceleration);
    }

    private void HelpInSlope()
    {
        rb.AddForce(Vector3.up * (slopHelpStrength * Time.fixedDeltaTime), ForceMode.Acceleration);
    }

    #endregion


    private void ManageWalkSound()
    {
        walkSoundDuration = Mathf.Lerp(crouchSoundDuration, runSoundDuration, rb.velocity.magnitude / runSpeed);
        if (inputDirection == Vector3.zero)
            return;

        walkSoundTimer += Time.deltaTime;

        if(walkSoundTimer > walkSoundDuration)
        {
            walkSoundTimer = 0;
            AudioManager.Instance.PlaySoundOneShot(1, Random.Range(6, 16), 0);
        }
    }
}
