using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerOpti : MonoBehaviour
{
    [Header("Gizmos Parameters")] 
    [SerializeField] private Color gizmosColor;

    [Header("Parameters")] [SerializeField]
    private List<GameObject> objectsToDeactivateOnStart = new List<GameObject>();
    
    [Header("References")] 
    [SerializeField] private List<GameObject> objectToActivate = new List<GameObject>();
    [SerializeField] private List<GameObject> objectToDeactivate = new List<GameObject>();


    private void Start()
    {
        for (int i = 0; i < objectsToDeactivateOnStart.Count; i++)
        {
            objectsToDeactivateOnStart[i].SetActive(false);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // True si le joueur suit le sens du jeu 
        bool goFront = transform.InverseTransformPoint(other.transform.position).z > 0;

        for (int i = 0; i < objectToActivate.Count; i++)
        {
            objectToActivate[i].SetActive(goFront);
        }
        for (int i = 0; i < objectToDeactivate.Count; i++)
        {
            objectToDeactivate[i].SetActive(!goFront);
        }
        
    }

    public void Activate()
    {
        bool goFront = true;
        for (int i = 0; i < objectToActivate.Count; i++)
        {
            objectToActivate[i].SetActive(goFront);
        }
        for (int i = 0; i < objectToDeactivate.Count; i++)
        {
            objectToDeactivate[i].SetActive(!goFront);
        }
    }
    
    public void Desactivate()
    {
        bool goFront = false;
        for (int i = 0; i < objectToActivate.Count; i++)
        {
            objectToActivate[i].SetActive(goFront);
        }
        for (int i = 0; i < objectToDeactivate.Count; i++)
        {
            objectToDeactivate[i].SetActive(!goFront);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmosColor;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

        Gizmos.DrawCube(Vector3.zero, transform.localScale);
    }
}
