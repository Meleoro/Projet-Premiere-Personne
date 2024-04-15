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
    public NoiseType currentNoiseType;
    public bool isHidden;

    [Header("Actions")] 
    public Action<ItemData> UseAdrenaline;

    [Header("References")] 
    private Controls controls;


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
            if(interactibleAtRange != null)
                interactibleAtRange.Interact();
        }
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
}
