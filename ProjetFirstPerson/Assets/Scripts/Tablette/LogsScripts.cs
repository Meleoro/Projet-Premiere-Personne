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
    private bool isTraducted;
    
    void Start()
    {
        logsMenu = GameObject.Find("TabletteManager").GetComponent<LogsMenu>();
        InformationArea = GameObject.Find("LogsInfoAreaText").GetComponent<TextMeshProUGUI>();
        TraductionButton = GameObject.Find("TraductionButton").GetComponent<Button>();
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
        }
        else
        {
            InformationArea.text = MyInformation;
        }
        TraductionButton.GetComponent<LogsScripts>().MyInformation = MyInformation;
    }
    public void Traduction()
    {
        InformationArea.text = MyInformation;
        if(logsMenu.currentLog != null)
        {
            logsMenu.currentLog.GetComponent<LogsScripts>().isTraducted = true;
        }
    }
}
