using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterManager : GenericSingletonClass<CharacterManager>
{
    [Header("Parameters")] 
    private List<ICharacterComponent> characterComponents = new List<ICharacterComponent>();

    [Header("Parameters Hideout")] 
    [SerializeField] private float distMaxFade;
    [SerializeField] private float distMinFade;
    [SerializeField] private Color colorNormal;
    [SerializeField] private Color colorFade;
    
    [Header("Public Infos")] 
    public bool isInteracting;
    public IInteractible interactibleAtRange;
    public NoiseType currentNoiseType;
    public bool isHidden;
    public bool isInSneakZone;

    [Header("Private Infos")] 
    private Coroutine hideEffectCoroutine;
    
    [Header("Actions")] 
    public Action<ItemData> UseAdrenaline;
    public Action enterSneakZone;
    public Action exitSneakZone;

    [Header("References")]
    public MeshRenderer capsule;
    private Controls controls;
    private CrouchComponent crouchComponent;
    public CameraComponent cameraComponent;


    public override void Awake()
    {
        base.Awake();
        
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
        characterComponents = GetComponents<ICharacterComponent>().ToList();
        crouchComponent = GetComponent<CrouchComponent>();
        cameraComponent = GetComponent<CameraComponent>();
    }
    
    

    private void Update()
    {
        // We execute the updates of all the components attached 
        for (int i = 0; i < characterComponents.Count; i++)
        {
            characterComponents[i].ComponentUpdate();
        }
        
        if (controls.Player.PickItem.WasPerformedThisFrame())
        {
            if(interactibleAtRange != null && !isInteracting)
                interactibleAtRange.Interact();
        }
        
        // Hide part
        SneakCharacterUpdate();
    }
    
    private void FixedUpdate()
    {
        // We execute the fixed updates of all the components attached 
        for (int i = 0; i < characterComponents.Count; i++)
        {
            characterComponents[i].ComponentFixedUpdate();
        }
    }
    
    private void LateUpdate()
    {
        // We execute the late updates of all the components attached 
        for (int i = 0; i < characterComponents.Count; i++)
        {
            characterComponents[i].ComponentLateUpdate();
        }
    }


    #region Hide Character

    private void SneakCharacterUpdate()
    {
        if (isInSneakZone)
        {
            if (crouchComponent.isCrouched && !isHidden)
            {
                Hide();
                enterSneakZone.Invoke();
            }
            else if(!crouchComponent.isCrouched && isHidden)
            {
                UnHide();
                exitSneakZone.Invoke();
            }
        }
        else
        {
            if (isHidden)
            {
                UnHide();
                exitSneakZone.Invoke();
            }
        }

        if (crouchComponent.isCrouched)
        {
            ApplyTransparency();
        }
    }
    
    private void Hide()
    {
        isHidden = true;
        
        if(hideEffectCoroutine != null)
            StopCoroutine(hideEffectCoroutine);
        
        hideEffectCoroutine = StartCoroutine(CameraEffects.Instance.Hide(1));
    }

    private void UnHide()
    {
        isHidden = false;
        
        if(hideEffectCoroutine != null)
            StopCoroutine(hideEffectCoroutine);
        
        hideEffectCoroutine = StartCoroutine(CameraEffects.Instance.Hide(0));
        
        RaycastHit[] hideColliders = Physics.SphereCastAll(transform.position, distMaxFade+  2, Vector3.up, 0);

        for (int i = 0; i < hideColliders.Length; i++)
        {
            if (hideColliders[i].collider.TryGetComponent<SneakZone>(out SneakZone sneakZone))
            {
                if (!hideColliders[i].collider.GetComponent<SneakZone>().isMaster)
                {
                    sneakZone._renderer.materials[0].SetColor("_alphaControl", colorNormal);
                    sneakZone._renderer.materials[1].SetColor("_alphaControl", colorNormal);
                }

            }
        }
    }

    private void ApplyTransparency()
    {
        RaycastHit[] hideColliders = Physics.SphereCastAll(transform.position, distMaxFade, Vector3.up, 0);

        for (int i = 0; i < hideColliders.Length; i++)
        {
            if (hideColliders[i].collider.TryGetComponent<SneakZone>(out SneakZone sneakZone))
            {
                if (!hideColliders[i].collider.GetComponent<SneakZone>().isMaster)
                {
                    float dist = Vector3.Distance(transform.position, hideColliders[i].collider.transform.position);
                    dist = Mathf.Clamp(dist - distMinFade, 0, distMaxFade - distMinFade);
                
                    float t = 1 - (dist / (distMaxFade - distMinFade));
                
                    sneakZone._renderer.materials[0].SetColor("_alphaControl", Color.Lerp(colorNormal, colorFade, t));
                    sneakZone._renderer.materials[1].SetColor("_alphaControl", Color.Lerp(colorNormal, colorFade, t));
                }
               
            }
        }
        
    }

    #endregion
}
