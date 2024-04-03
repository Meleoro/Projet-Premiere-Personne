using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : GenericSingletonClass<UIManager>
{
    [Header("References")]
    [SerializeField] private Image interactImage;
    [SerializeField] private CameraComponent cameraComponent;

    [Header("UI Variables")]
    [SerializeField] private GameObject Album;
    public bool isUIActive;
    public GameObject currentSelect;


    private void Start()
    {
        HideInteractIcon();
        Album.SetActive(false);
    }

    void Update()
    {
        // Quand le joueur clic, on check si un élément est séléctionné dans l'Event system et on fait une variable
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(currentSelect != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                currentSelect = null;
            }
            else
            {
                currentSelect = EventSystem.current.currentSelectedGameObject;
            }
        }
        // Si un élément de board est séléctionné, il suit le curseur de la souris
        if(currentSelect != null)
        {
            currentSelect.GetComponent<ElementsOfBoard>().MyParent.transform.position = Input.mousePosition;
        }

        // Test Ouvrir Album
        if(Input.GetKeyDown(KeyCode.Keypad3))
        {
             if (!Album.activeSelf)
            {
                cameraComponent.LockedCursor(false);
                Album.SetActive(true);
                isUIActive = true;
            }
            else
            {
                cameraComponent.LockedCursor(true);
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
