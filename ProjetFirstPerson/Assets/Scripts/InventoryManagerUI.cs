using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManagerUI : MonoBehaviour
{
    [Header("Parameters")] 
    [Range(5f, 20f)] [SerializeField] private float slotOffsetScreenPercentage;

    [Header("Public Infos")] 
    [HideInInspector] public List<InventorySlot> inventorySlotsScripts = new();
    [HideInInspector] public InventorySlot selectedSlot;
    
    [Header("References")] 
    [SerializeField] private GameObject inventorySlot;
    [SerializeField] private RectTransform originalTransformSlotLocation;


    #region INITIALISATION

    private void Start()
    {
        InventoryManager.Instance.inventoryUIScript = this;
        
        GenerateInventorySlots();
    }
    

    private void GenerateInventorySlots()
    {
        int slotAmount = InventoryManager.Instance.inventorySlotsAmount;

        float addedOffset = slotOffsetScreenPercentage / 100 * Camera.main.scaledPixelWidth;
        float currentX = originalTransformSlotLocation.position.x - addedOffset * (slotAmount * 0.5f) + addedOffset * 0.5f;

        for (int i = 0; i < slotAmount; i++)
        {
            RectTransform newSlotRect = Instantiate(inventorySlot, transform).GetComponent<RectTransform>();

            newSlotRect.position = new Vector3(currentX, originalTransformSlotLocation.position.y,
                originalTransformSlotLocation.position.z);

            currentX += addedOffset;
            
            inventorySlotsScripts.Add(newSlotRect.GetComponent<InventorySlot>());
        }
    }

    #endregion
    
    
    public void ChangeSelectedSlot(int index)
    {
        if(selectedSlot)
            selectedSlot.UnselectSlot();

        selectedSlot = inventorySlotsScripts[index];
        
        selectedSlot.SelectSlot();
    }

}
