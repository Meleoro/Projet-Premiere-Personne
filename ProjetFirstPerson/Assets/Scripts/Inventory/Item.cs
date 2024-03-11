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
        HideUI();
        
        if (isInRange)
        {
            if (VerifyLookingItem())
            {
                DisplayUI();
                
                CharacterManager.Instance.interactibleAtRange = this;
            }
        }
    }
    

    public void Interact()
    {
        if (!InventoryManager.Instance.VerifyInventoryFull())
        {
            InventoryManager.Instance.AddItem(itemData);
            HideUI();
        
            Destroy(gameObject);
        }
    }


    private bool VerifyLookingItem()
    {
        Vector3 dirCamItem = transform.position - CameraManager.Instance.transform.position;
        Vector3 dirCamLook = CameraManager.Instance.transform.forward;

        Vector3 crossProduct = Vector3.Cross(dirCamItem, dirCamLook);

        if (crossProduct.sqrMagnitude < 0.3f)
            return true;

        else
            return false;
    }


    private void DisplayUI()
    {
        UIManager.Instance.DisplayInteractIcon();
    }

    private void HideUI()
    {
        UIManager.Instance.HideInteractIcon();
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
