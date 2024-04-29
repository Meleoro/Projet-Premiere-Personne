using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogsMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject ContentLog;
    [SerializeField] private GameObject LogPrefab;

    public void AddLogsToContent(string info, string title)
    {
        GameObject NewLog = Instantiate(LogPrefab,Vector3.zero,Quaternion.Euler(0,0,0),ContentLog.transform);
        NewLog.GetComponent<LogsScripts>().MyInformation = info;
        NewLog.GetComponent<LogsScripts>().TitleArea.text = title;
    }

    void Update()
    {
        
    }
}
