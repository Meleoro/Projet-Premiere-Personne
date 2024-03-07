using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : GenericSingletonClass<InventoryManager>
{
    [Header("Parameters")] 
    [Range(0, 10)] public int inventorySlotsAmount;

    [Header("Private Infos")] 
    private float currentScrollValue;
    private int selectedIndex;
    
    [Header("References")] 
    [HideInInspector] public InventoryManagerUI inventoryUIScript;
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


    private void Update()
    {
        if (controls.Player.MouseScroll.ReadValue<float>() > 1)
        {
            ChangeSelected(true);
        }
        else if (controls.Player.MouseScroll.ReadValue<float>() < -1)
        {
            ChangeSelected(false);
        }
    }

    
    private void ChangeSelected(bool goUp)
    {
        selectedIndex = goUp ? selectedIndex + 1 : selectedIndex - 1;

        if (selectedIndex >= inventorySlotsAmount)
            selectedIndex = 0;
        
        else if (selectedIndex < 0)
            selectedIndex = inventorySlotsAmount - 1;
        
        inventoryUIScript.ChangeSelectedSlot(selectedIndex);
    }
}
