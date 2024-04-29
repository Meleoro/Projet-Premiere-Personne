using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteleScript : MonoBehaviour
{
    [Header("References")]
    private LogsMenu logsMenu;
    public bool isAlreadyInLogs;
    [SerializeField]  public string titleLogs;
    [SerializeField] [TextArea(5,10)] public string myInfo;
    
    void Start()
    {
        logsMenu = GameObject.Find("TabletteManager").GetComponent<LogsMenu>();
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player") && !isAlreadyInLogs)
        {
            isAlreadyInLogs = true;
            logsMenu.AddLogsToContent(myInfo,titleLogs);
        }
    }
}
