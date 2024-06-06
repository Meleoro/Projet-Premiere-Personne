using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LogsMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject ContentLog;
    [SerializeField] private GameObject LogPrefab;
    [SerializeField] public GameObject currentLog;
    [SerializeField] public Button TraductionButton;
    public Animation logPopUpAnim;
    public List<GameObject> logsList;
    public List<GameObject> logIconUI;

    [Header("Lorem ipsum")]
    [SerializeField] string characters = "abcdefghijklmnopqrstuvwyz0123456789";
    [SerializeField] private int loremIpsumLenghtValue = 150;
    [SerializeField] private string myRandomString;
    
    public int unreadLogs;

    public void AddLogsToContent(string info, string title, bool isTriggered)
    {
        TraductionButton.gameObject.SetActive(true);
        GameObject NewLog = Instantiate(LogPrefab,Vector3.zero,Quaternion.Euler(0,0,0),ContentLog.transform);
        NewLog.GetComponent<LogsScripts>().TitleArea.text = title;
        unreadLogs += 1;
        for (int i = 0; i < logIconUI.Count; i++)
        {
            logIconUI[i].SetActive(true);
        }
        for(int i = 0; i < loremIpsumLenghtValue; i++)
        {
            myRandomString += characters[Random.Range(0, characters.Length)];
        }
        if(isTriggered)
        {
            NewLog.GetComponent<LogsScripts>().codedInfo = info;
            NewLog.GetComponent<LogsScripts>().MyInformation = info;
            NewLog.GetComponent<LogsScripts>().isTraducted = true;
        }
        else
        {
            NewLog.GetComponent<LogsScripts>().codedInfo = myRandomString;
            myRandomString = null;
            NewLog.GetComponent<LogsScripts>().MyInformation = info;
        }
        logsList.Add(NewLog);
    }

   
}
