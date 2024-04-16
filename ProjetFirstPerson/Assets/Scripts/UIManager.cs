using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UIManager : GenericSingletonClass<UIManager>
{
    [Header("References")]
    [SerializeField] private Image interactImage;
    [SerializeField] private CameraComponent cameraComponent;

    [Header("UI Variables")]
    [SerializeField] private GameObject Album;
    public bool isUIActive;
    private void Start()
    {
        HideInteractIcon();
        Album.SetActive(false);
    }

    void Update()
    {

        // Test Ouvrir Album
        if(Input.GetKeyDown(KeyCode.Tab))
        {
             if (!Album.activeSelf)
            {
                cameraComponent.LockedCursor(1);
                Album.SetActive(true);
                isUIActive = true;
            }
            else
            {
                cameraComponent.LockedCursor(2);
                Album.SetActive(false);
                isUIActive = false;
            }
        } 
    }
    
    

    #region SELECTION

    public void DisplayInteractIcon()
    {
        interactImage.gameObject.SetActive(true);
    }

    public void HideInteractIcon()
    {
        interactImage.gameObject.SetActive(false);
    }

    #endregion
}
