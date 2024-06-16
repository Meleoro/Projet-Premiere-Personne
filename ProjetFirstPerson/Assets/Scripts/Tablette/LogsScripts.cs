using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


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
    public bool isTraducted;
    public Coroutine traduction;
    [SerializeField] private Sprite unselectedLogs, selectedLogs;
    [Header("Scripts Letter")]
    [SerializeField] private float typingSpeed = 0.05f;
    [HideInInspector] public bool isWriting;
    public bool isActive;

    [Header("Police")]
    [SerializeField] private TMP_FontAsset normalPolice;
    [SerializeField] private TMP_FontAsset codedPolice;

   // public string myWord;
    public List<string> colorWorld;
    public List<int> colorInt;
    
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

            if(newLogsList[i] == gameObject)
            {
                newLogsList[i].GetComponent<LogsScripts>().isActive = true;
                newLogsList[i].GetComponent<Image>().sprite = selectedLogs;
            }
            else
            {
                newLogsList[i].GetComponent<LogsScripts>().isActive = false;
                newLogsList[i].GetComponent<Image>().sprite = unselectedLogs;
            }
        }
    }
    

    public void InstantiateMyInfo()
    {
        StopTraduction();
        logsMenu.currentLog = gameObject;
        
        LogsListFunction();
        
        if (!isRead)
        {
            isRead = true;
            transform.GetChild(1).gameObject.SetActive(false);
            logsMenu.unreadLogs -= 1;
            logsMenu.unreadCounter.text = logsMenu.unreadLogs + "";
            if (logsMenu.unreadLogs == 0)
            {
                logsMenu.bouttonToLog.SetActive(false);
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
            InformationArea.font = codedPolice;
        }
        else
        {
            InformationArea.font = normalPolice;
            TraductionButton.gameObject.SetActive(false);

            InformationArea.text = "";
            int currentValue = 0;

        for(int i = 0; i < MyInformation.Length ; i++)
        {
            if(currentValue < colorInt.Count)
            {
                if(i >= colorInt[currentValue] && i < colorInt[currentValue + 1])
            {
                string newLetter = MyInformation[i].ToString();
                InformationArea.text += "<color=orange>" + newLetter + "</color>";
                if(i == colorInt[currentValue + 1] - 1)
                {
                    currentValue += 2;
                }
            }
            else
            {
                InformationArea.text += MyInformation[i];
            }
            }
            else
            {
                InformationArea.text += MyInformation[i];
            }
        }
        }
        TraductionButton.GetComponent<LogsScripts>().MyInformation = MyInformation;

        TraductionButton.GetComponent<LogsScripts>().colorInt = colorInt;
        TraductionButton.GetComponent<LogsScripts>().colorWorld = colorWorld;
    }
    public void Traduction()
    {
      //  InformationArea.text = MyInformation;
        InformationArea.font = normalPolice;
        traduction = StartCoroutine(TypeText(MyInformation));
        TraductionButton.interactable = false;
    }

    public void StopTraduction()
    {
        if (logsMenu.currentLog != null)
        {
                isWriting = false;
                TraductionButton.gameObject.SetActive(false);  
                AudioManager.Instance.FadeOutAudioSource(0.5f,8);
        }
        AudioManager.Instance.FadeOutAudioSource(0.5f,8);
    }

   
    
    public void PlayUISound()
    {
        AudioManager.Instance.PlaySoundOneShot(1, 16, 0);
    }

     private IEnumerator TypeText(string text)
    {
        AudioManager.Instance.PlaySoundOneShot(1, 23, 8);
        logsMenu.currentLog.GetComponent<LogsScripts>().isTraducted = true;
        isWriting = true;
        InformationArea.text = "";
        int currentValue = 0;

        if (colorWorld.Count > 0)
        {
            for(int i = 0; i < colorWorld.Count ; i++)
            {
                int newIndex = text.IndexOf(colorWorld[i]);
                int largeNewIndex = newIndex + colorWorld[i].Length;
                colorInt.Add(newIndex);
                colorInt.Add(largeNewIndex);
            } 
        }
      

        for(int i = 0; i < text.Length ; i++)
        {
            if(currentValue < colorInt.Count)
            {
                if(i >= colorInt[currentValue] && i < colorInt[currentValue + 1])
            {
                string newLetter = text[i].ToString();
                InformationArea.text += "<color=orange>" + newLetter + "</color>";
                if(i == colorInt[currentValue + 1] - 1)
                {
                    currentValue += 2;
                }
            }
            else
            {
                InformationArea.text += text[i];
            }
            }
            else
            {
                InformationArea.text += text[i];
            }
            yield return new WaitForSeconds(typingSpeed);
        }
        AudioManager.Instance.FadeOutAudioSource(0.5f,8);
            TraductionButton.gameObject.SetActive(false);
    }
}
