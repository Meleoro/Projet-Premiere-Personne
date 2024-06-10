using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterManager : GenericSingletonClass<CharacterManager>
{
    [Header("Parameters")] 
    private List<ICharacterComponent> characterComponents = new List<ICharacterComponent>();

    [Header("Public Infos")] 
    public IInteractible interactibleAtRange;
    public bool isInteracting;
    public NoiseType currentNoiseType;
    public bool isHidden;
    public bool isInSneakZone;

    [Header("Actions")] 
    public Action<ItemData> UseAdrenaline;

    [Header("References")]
    public MeshRenderer capsule;
    private Controls controls;
    private CrouchComponent crouchComponent;


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
            }
            else if(!crouchComponent.isCrouched && isHidden)
            {
                UnHide();
            }
        }
        else
        {
            if (isHidden)
            {
                UnHide();
            }
        }
    }
    
    private void Hide()
    {
        isHidden = true;
        
        StartCoroutine(CameraEffects.Instance.Hide(1));
    }

    private void UnHide()
    {
        isHidden = false;
        
        StartCoroutine(CameraEffects.Instance.Hide(0));
    }

    #endregion
}
