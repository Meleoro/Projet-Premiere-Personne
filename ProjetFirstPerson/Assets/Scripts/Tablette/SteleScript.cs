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
    public float fadeDuration;
    public MeshRenderer mesh;
    [HideInInspector] public ParticleSystem activationVFX;
    [HideInInspector] public GameObject fumeVFX;
    [HideInInspector] public Material material;
    
    [Header("Parameters Gizmos")] 
    [SerializeField] private bool showGizmosOnlyOnSelected;
    [SerializeField] private Color gizmosColor;
    
    void Start()
    {
        logsMenu = GameObject.Find("TabletteManager").GetComponent<LogsMenu>();
        if (transform.childCount > 0)
        {
            material = mesh.material;
            activationVFX = transform.GetChild(0).GetComponent<ParticleSystem>();
            fumeVFX = transform.GetChild(1).gameObject;
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player") && !isAlreadyInLogs)
        {
            logsMenu.logPopUpAnim.clip = logsMenu.logPopUpAnim["NewLogAnim"].clip;
            logsMenu.logPopUpAnim.Play();
            StartCoroutine(WaitAndLoopPopUp());
            AudioManager.Instance.PlaySoundOneShot(1, 17, 0);
            isAlreadyInLogs = true;
            logsMenu.AddLogsToContent(myInfo,titleLogs,true);
        }
    }

    IEnumerator WaitAndLoopPopUp()
    {
        yield return new WaitForSeconds(logsMenu.logPopUpAnim.clip.length);
        logsMenu.logPopUpAnim.clip = logsMenu.logPopUpAnim["PopUpLogIdle"].clip;
        logsMenu.logPopUpAnim.Play();
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

    public IEnumerator ChangeShader()
    {
        float value = 0;
        float time = 0;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            value = Mathf.Lerp(value, 1, time / fadeDuration);
            material.SetFloat("_ActivationTransition",value);
            yield return null;
        }

        value = 1;
        material.SetFloat("_ActivationTransition",value);
    }
}
