using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractible
{
    [Header("Parameters")] 
    [SerializeField] private ItemData itemData;

    [Header("Private Infos")] 
    private bool isInRange;


    private void Update()
    {
        if (isInRange)
        {
            CharacterManager.Instance.interactibleAtRange = this;
        }
    }
    

    public void Interact()
    {
        if (InventoryManager.Instance.VerifyInventoryFull())
        {
            InventoryManager.Instance.AddItem(itemData);
        
            Destroy(gameObject);
        }
    }


    private void DisplayUI()
    {
        
    }

    private void HideUI()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
        }
    }
}
