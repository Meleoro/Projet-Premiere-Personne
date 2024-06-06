using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LogsScripts : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LogsMenu logsMenu;
    [SerializeField] private TextMeshProUGUI InformationArea;
    [SerializeField] public TextMeshProUGUI TitleArea;
    public string codedInfo;
    public string MyInformation;
    [SerializeField] private Button TraductionButton;
    [HideInInspector] public bool isRead;
    [HideInInspector] public bool isTraducted;

    [Header("Scripts Letter")]
    [SerializeField] private float typingSpeed = 0.05f;
    [HideInInspector] public bool isWriting;
    public bool isActive;
    
    void Start()
    {
        isRead = false;
        logsMenu = GameObject.Find("TabletteManager").GetComponent<LogsMenu>();
        InformationArea = GameObject.Find("LogsInfoAreaText").GetComponent<TextMeshProUGUI>();
        TraductionButton = logsMenu.TraductionButton;

        if(isTraducted)
        {
            TraductionButton.gameObject.SetActive(false);
        }
    }
    public void LogsListFunction()
    {
        List<GameObject> newLogsList = logsMenu.logsList;
        for(int i = 0; i < newLogsList.Count ; i++)
        {
            newLogsList[i].SetActive(false);
            newLogsList[i].SetActive(true);
        }
    }
    

    public void InstantiateMyInfo()
    {
        logsMenu.currentLog = gameObject;
        LogsListFunction();
        
        if (!isRead)
        {
            transform.GetChild(1).gameObject.SetActive(false);
            logsMenu.unreadLogs -= 1;
            if (logsMenu.unreadLogs == 0)
            {
                for (int i = 0; i < logsMenu.logIconUI.Count; i++)
                {
                    logsMenu.logIconUI[i].SetActive(false);
                }
                logsMenu.logPopUpAnim.clip = logsMenu.logPopUpAnim["NewLogAnimOut"].clip;
                logsMenu.logPopUpAnim.Play();
            }
        }
      

        if(!isTraducted)
        {
            InformationArea.text = codedInfo;
            TraductionButton.gameObject.SetActive(true);
            TraductionButton.interactable = true;
        }
        else
        {
            InformationArea.text = MyInformation;
            TraductionButton.gameObject.SetActive(false);
        }
        TraductionButton.GetComponent<LogsScripts>().MyInformation = MyInformation;
    }
    public void Traduction()
    {
      //  InformationArea.text = MyInformation;
        StartCoroutine(TypeText(MyInformation));
        TraductionButton.interactable = false;
    }
    
    public void PlayUISound()
    {
        AudioManager.Instance.PlaySoundOneShot(1, 16, 1);
    }

     private IEnumerator TypeText(string text)
    {
        logsMenu.currentLog.GetComponent<LogsScripts>().isTraducted = true;
        isWriting = true;
        InformationArea.text = "";
          foreach(char letter in text.ToCharArray())
            {
                if(isWriting)
                {
                    InformationArea.text += letter;
                    yield return new WaitForSeconds(typingSpeed);
                }
            }  
            TraductionButton.gameObject.SetActive(false);
    }
}
