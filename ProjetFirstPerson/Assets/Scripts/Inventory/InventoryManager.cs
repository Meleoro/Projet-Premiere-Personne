using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : GenericSingletonClass<InventoryManager>
{
    [Header("Parameters")] 
    [Range(0, 10)] public int inventorySlotsAmount;

    [Header("Public Infos")]
    public ItemData selectedItem;
    
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
            ChangeSelected(true);
        
        else if (controls.Player.MouseScroll.ReadValue<float>() < -1)
            ChangeSelected(false);
        

        if (controls.Player.UseItem.WasPerformedThisFrame())
            UseItem();
        
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

    private void UseItem()
    {
        selectedItem = inventoryUIScript.GetSelectedItemData();
        
        if (selectedItem)
        {
            switch (selectedItem.function)
            {
                case ObjectFunction.heal :
                    Debug.Log("You healed" + selectedItem.HPHealed + "PVs");
                    break;
            }
            
            RemoveItem();
        }
    }


    public bool VerifyInventoryFull()
    {
        for (int i = 0; i < inventoryUIScript.inventorySlotsScripts.Count; i++)
        {
            if (!inventoryUIScript.inventorySlotsScripts[i].currentSlotItem)
            {
                return false;
            }
        }

        return true;
    }
    
    public void AddItem(ItemData addedItemData)
    {
        inventoryUIScript.AddItem(addedItemData);
    }

    private void RemoveItem()
    {
        inventoryUIScript.RemoveItem(selectedIndex);
    }
}
