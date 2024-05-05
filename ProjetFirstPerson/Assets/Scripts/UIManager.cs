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
    [SerializeField] private GameObject GeneralMenu, BoardMenu, LogsMenu, MapMenu;
    [SerializeField] public bool isUIActive = false;

    
    
    private void Start()
    {

        HideInteractIcon();
        GeneralMenu.SetActive(false);
    }

    void Update()
    {
        EyeIconUpdate();
        
        // Test Ouvrir Album
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(!isUIActive)
            {
                cameraComponent.canRotate = false;
                cameraComponent.LockedCursor(1);
                isUIActive = true;
                CloseAllPanel(true,false,false,false);
            }
          else
            {
                cameraComponent.canRotate = true;
                cameraComponent.LockedCursor(2);
                isUIActive = false;
                CloseAllPanel(false,false,false,false);
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
    #region Button
    public void CloseAllPanel(bool GeneralBool, bool BoardBool, bool MapBool, bool LogsBool)
    {
        GeneralMenu.SetActive(GeneralBool);
        BoardMenu.SetActive(BoardBool);
        MapMenu.SetActive(MapBool);
        LogsMenu.SetActive(LogsBool);
    }
    public void OpenBoardMenu()
    {
        if (!BoardMenu.activeSelf)
            {
                CloseAllPanel(false,true,false,false);
                isUIActive = true;
            }
    }
    public void OpenMapMenu()
    {
        if (!MapMenu.activeSelf)
            {
                CloseAllPanel(false,false,true,false);
                isUIActive = true;
            }
    }
    public void OpenLogsMenu()
    {
        if (!LogsMenu.activeSelf)
            {
                CloseAllPanel(false,false,false,true);
                isUIActive = true;
            }
    }
    #endregion
    
    
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
