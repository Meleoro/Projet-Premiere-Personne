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
    public GameObject currentSelect;

    [Header("Manipulation Variables")]
    [SerializeField] private bool isRotating;
    [SerializeField] private Vector3 mousePos;
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

        // La rotation des objets
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            mousePos = Input.mousePosition;
        }
        if(Input.GetKey(KeyCode.Mouse1))
        {
            if(currentSelect != null)
            {
                Transform HisParent = currentSelect.GetComponent<ElementsOfBoard>().MyParent.transform;
                HisParent.localEulerAngles = new Vector3(0,0,Input.mousePosition.y);
                isRotating = true;
                Cursor.visible = false;
            }
        }
        if(Input.GetKeyUp(KeyCode.Mouse1))
        {
                isRotating = false;
                Cursor.visible = true;
                Mouse.current.WarpCursorPosition(mousePos);
        }

        // Si un élément de board est séléctionné, il suit le curseur de la souris
        if(currentSelect != null && currentSelect.CompareTag("MovingUI"))
        {
            Transform HisParent = currentSelect.GetComponent<ElementsOfBoard>().MyParent.transform;
            HisParent.localScale += ( new Vector3(0.1f,0.1f,0) * Input.mouseScrollDelta.y );

            if(!isRotating)
            {
                HisParent.position = Input.mousePosition;
            }
            if(HisParent.localScale.x <= 0.1f)
            {
                HisParent.localScale = new Vector3(0.1f,0.1f,0);
            }
           else if(HisParent.localScale.x >= 3f)
            {
                HisParent.localScale = new Vector3(3f,3f,0);
            }    
        }

        // Test Ouvrir Album
        if(Input.GetKeyDown(KeyCode.Keypad3))
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
