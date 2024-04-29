using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LogsScripts : MonoBehaviour
{
    [Header("References")]
    private TextMeshProUGUI InformationArea;
    public string MyInformation;
    
    void Start()
    {
        InformationArea = GameObject.Find("LogsInfoAreaText").GetComponent<TextMeshProUGUI>();
    }

    public void InstantiateMyInfo()
    {
        InformationArea.text = MyInformation;
    }
}
