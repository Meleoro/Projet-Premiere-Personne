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
    [Header("Eye Icon Parameters")] 
    [SerializeField] private float maxYEyeIcon;
    [SerializeField] private float minYEyeIcon;
    
    
    [Header("Public Infos")] 
    [HideInInspector] public bool isInCreatureView;

    [Header("Private Infos")]
    private float currentTimer;

    [Header("References")]
    [SerializeField] private RectTransform HUDParent;
    [SerializeField] private Image interactImage;
    [SerializeField] private Image eyeIconImage;
    [SerializeField] private CameraComponent cameraComponent;
    public Image fadeImage;

    [Header("UI Variables")]
    [SerializeField] private GameObject Album;
    [SerializeField] private GameObject LogPanel;
    public bool isUIActive;

    
    
    private void Start()
    {
        LogPanel = GameObject.Find("LogsPanel");

        HideInteractIcon();
        Album.SetActive(false);
        LogPanel.SetActive(false);
    }

    void Update()
    {
        EyeIconUpdate();
        
        // Test Ouvrir Album
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if (!Album.activeSelf)
            {
                cameraComponent.canRotate = false;
                cameraComponent.LockedCursor(1);
                Album.SetActive(true);
                isUIActive = true;
            }
            else
            {
                cameraComponent.canRotate = true;
                cameraComponent.LockedCursor(2);
                Album.SetActive(false);
                isUIActive = false;
            }
        } 

        if(Input.GetKeyDown(KeyCode.Keypad0))
        {
            if (!LogPanel.activeSelf)
            {
                cameraComponent.LockedCursor(1);
                LogPanel.SetActive(true);
                isUIActive = true;
            }
            else
            {
                cameraComponent.LockedCursor(2);
                LogPanel.SetActive(false);
                isUIActive = false;
            }
        }
    }


    public void HideHUD()
    {
        HUDParent.gameObject.SetActive(false);
    }

    public void ShowHUD()
    {
        HUDParent.gameObject.SetActive(true);
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

    #region EYE ICON

    private void EyeIconUpdate()
    {
        if (isInCreatureView)
        {
            currentTimer += Time.deltaTime * 4;
        }
        else
        {
            currentTimer -= Time.deltaTime * 3;
        }
        
        currentTimer = Mathf.Clamp(currentTimer, 0, 1);
        eyeIconImage.rectTransform.sizeDelta = new Vector2(eyeIconImage.rectTransform.sizeDelta.x, Mathf.Lerp(minYEyeIcon, maxYEyeIcon, currentTimer));
    }

    #endregion
}
