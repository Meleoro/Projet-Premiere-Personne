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
    [HideInInspector] public bool isTraducted;

    [Header("Scripts Letter")]
    [SerializeField] private float typingSpeed = 0.05f;
    private bool isWriting;
    
    void Start()
    {
        logsMenu = GameObject.Find("TabletteManager").GetComponent<LogsMenu>();
        InformationArea = GameObject.Find("LogsInfoAreaText").GetComponent<TextMeshProUGUI>();
        TraductionButton = logsMenu.TraductionButton;

        if(isTraducted)
        {
            TraductionButton.gameObject.SetActive(false);
        }
    }
    public void ChangeTitle()
    {

    }

    public void InstantiateMyInfo()
    {
        
        logsMenu.currentLog = gameObject;

        if(!isTraducted)
        {
            InformationArea.text = codedInfo;
            TraductionButton.gameObject.SetActive(true);
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
            logsMenu.currentLog.GetComponent<LogsScripts>().isTraducted = true;
            TraductionButton.gameObject.SetActive(false);
    }
}
