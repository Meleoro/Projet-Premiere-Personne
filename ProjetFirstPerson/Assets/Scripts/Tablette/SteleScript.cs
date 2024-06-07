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
    public bool isFinalStele;
    
    [Header("Parameters Gizmos")] 
    [SerializeField] private bool showGizmosOnlyOnSelected;
    [SerializeField] private Color gizmosColor;
    
    void Start()
    {
        logsMenu = GameObject.Find("TabletteManager").GetComponent<LogsMenu>();
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player") && !isAlreadyInLogs)
        {
            logsMenu.logPopUpAnim.clip = logsMenu.logPopUpAnim["NewLogAnim"].clip;
            logsMenu.logPopUpAnim.Play();
            AudioManager.Instance.PlaySoundOneShot(1, 17, 0);
            isAlreadyInLogs = true;
            logsMenu.AddLogsToContent(myInfo,titleLogs,true);
        }
    }
    
    private void OnDrawGizmos()
    {
        if (!showGizmosOnlyOnSelected)
        {
            Gizmos.color = gizmosColor;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            
            Gizmos.DrawCube(Vector3.zero, transform.localScale);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (showGizmosOnlyOnSelected)
        {
            Gizmos.color = gizmosColor;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            
            Gizmos.DrawCube(Vector3.zero, transform.localScale);
        }
    }
}
