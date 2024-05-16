using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogsMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject ContentLog;
    [SerializeField] private GameObject LogPrefab;
    [SerializeField] public GameObject currentLog;

    [Header("Lorem ipsum")]
    [SerializeField] string characters = "abcdefghijklmnopqrstuvwyz0123456789";
    [SerializeField] private int loremIpsumLenghtValue = 150;
    [SerializeField] private string myRandomString;

    public void AddLogsToContent(string info, string title)
    {
        GameObject NewLog = Instantiate(LogPrefab,Vector3.zero,Quaternion.Euler(0,0,0),ContentLog.transform);
        NewLog.GetComponent<LogsScripts>().TitleArea.text = title;

        for(int i = 0; i < loremIpsumLenghtValue; i++)
        {
            myRandomString += characters[Random.Range(0, characters.Length)];
        }
        NewLog.GetComponent<LogsScripts>().codedInfo = myRandomString;
        myRandomString = null;
        NewLog.GetComponent<LogsScripts>().MyInformation = info;
    }

    void Update()
    {
        
    }
}
