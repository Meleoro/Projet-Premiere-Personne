using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    [SerializeField] public RectTransform InteractHUD;
    [SerializeField] private Image interactImage;
    [SerializeField] private Image eyeIconImageClosed;
    [SerializeField] private Image eyeIconImageOpened;
    [SerializeField] private CameraComponent cameraComponent;
    [SerializeField] private MoveComponent moveComponent;
    [SerializeField] public CameraTestEthan cam;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private CinematiqueFinale cinematiqueFinale;
    [SerializeField] private GameObject normalHUD;
    public Image fadeImage;

    [Header("References Menu Général Tablette")]
    [SerializeField] private HealthComponent playerHealth;
    [SerializeField] private TextMeshProUGUI textHealth;
    [SerializeField] private GameObject fullLifeBar;
    [SerializeField] private GameObject halfLifeBar;
    [SerializeField] private GameObject tabletteWorldFakeMenu;
    
    
    [Header("UI Variables")]
    [SerializeField] private GameObject GeneralMenu, BoardMenu, LogsMenu, SettingsMenu;
    private GameObject LastPanelOpen;
    [SerializeField] private TextMeshProUGUI schedule;
    [SerializeField] public bool isUIActive = false;
    public bool isFinalCinematic;
    public bool canMenu;
    private LogsMenu logsMenu;

    [Header("Cursor Variables")]
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    
    
    private void Start()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        HideInteractIcon();
        GeneralMenu.SetActive(false);
        logsMenu = GameObject.Find("TabletteManager").GetComponent<LogsMenu>();
    }

    void Update()
    {
        // Horaire du jeu
        EyeIconUpdate();
        //schedule.text = System.DateTime.Now + "";
        
        // Test Ouvrir Album
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            StartCoroutine(OpenMenu());
        } 
    }
    public void OpenMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public IEnumerator OpenMenu()
    {
        if (!cam.isAiming && !playerHealth.isDying && canMenu)
        {
            if(!isUIActive)
            {
                AudioManager.Instance.PlaySoundOneShot(1,21,0);
                if (playerHealth.isHurted)
                {
                    textHealth.text = "État de Santé : Critique";
                    fullLifeBar.SetActive(false);
                    halfLifeBar.SetActive(true);
                }
                else
                {
                    textHealth.text = "État de Santé : Bon";
                    fullLifeBar.SetActive(true);
                    halfLifeBar.SetActive(false);
                }
                cam.anim.SetBool("in",true);
                tabletteWorldFakeMenu.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                InteractHUD.gameObject.SetActive(false);
                normalHUD.SetActive(false);
                cameraComponent.canRotate = false;
                moveComponent.canMove = false;
                cameraComponent.LockedCursor(1);
                isUIActive = true;

                if(LastPanelOpen == null)
                {
                    LastPanelOpen = BoardMenu;
                    LastPanelOpen.SetActive(true);
                }
                else
                {
                    LastPanelOpen.SetActive(true);
                }
            }
            else
            {
                AudioManager.Instance.PlaySoundOneShot(1,22,0);
                tabletteWorldFakeMenu.SetActive(false);
                cam.anim.SetBool("in",false);
                if (!CharacterManager.Instance.isInteracting)
                {
                    normalHUD.SetActive(true);
                    cameraComponent.canRotate = true;
                    moveComponent.canMove = true;
                    cameraComponent.LockedCursor(2);
                }
                else InteractHUD.gameObject.SetActive(true);
            
                isUIActive = false;
                CloseAllPanel(false,false,false,false);
                logsMenu.ShutTraductionSound();
            }

            if (isFinalCinematic)
            {
                //cameraComponent.canRotate = false;
                //cameraComponent.canMove = false;
                moveComponent.canMove = false;
                cinematiqueFinale.lightEtape1.SetActive(false);
                StartCoroutine(cinematiqueFinale.DoSecondPart());
            }
        }
    }

    public IEnumerator OpenLogMenu()
    {
       StartCoroutine(OpenMenu());
       yield return new WaitForSeconds(0.5f);
       CloseAllPanel(false,false,false,true);
       isFinalCinematic = true;
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
    public void CloseAllPanel(bool GeneralBool, bool BoardBool, bool SettingsBool, bool LogsBool)
    {
        GeneralMenu.SetActive(GeneralBool);
        BoardMenu.SetActive(BoardBool);
        SettingsMenu.SetActive(SettingsBool);
        LogsMenu.SetActive(LogsBool);
    }
    public void OpenBoardMenu()
    {
        if (!BoardMenu.activeSelf)
            {
                CloseAllPanel(false,true,false,false);
                isUIActive = true;
                LastPanelOpen = BoardMenu;
            }
    }
    public void OpenSettingsMenu()
    {
        if (!SettingsMenu.activeSelf)
            {
                CloseAllPanel(false,false,true,false);
                isUIActive = true;
                LastPanelOpen = SettingsMenu;
            }
    }
    public void OpenLogsMenu()
    {
        if (!LogsMenu.activeSelf)
            {
                CloseAllPanel(false,false,false,true);
                isUIActive = true;
                LastPanelOpen = LogsMenu;
                logsMenu.RefreshLogs();
                logsMenu.OpenFirstUnreadLogs();
            }
    }
    #endregion
    
    
    #region SELECTION

    public void DisplayInteractIcon()
    {
        interactImage.gameObject.SetActive(true);
        interactText.enabled = true;
    }

    public void HideInteractIcon()
    {
        interactImage.gameObject.SetActive(false);
        interactText.enabled = false;
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

        eyeIconImageClosed.color = new Color(eyeIconImageClosed.color.r, eyeIconImageClosed.color.g,
            eyeIconImageClosed.color.b,
            Mathf.Lerp(1, 0, currentTimer)); 
        
        eyeIconImageOpened.color = new Color(eyeIconImageClosed.color.r, eyeIconImageClosed.color.g,
            eyeIconImageClosed.color.b,
            Mathf.Lerp(0, 1, currentTimer));

        //eyeIconImage.rectTransform.sizeDelta = new Vector2(eyeIconImage.rectTransform.sizeDelta.x, Mathf.Lerp(minYEyeIcon, maxYEyeIcon, currentTimer));
    }

    #endregion
}
