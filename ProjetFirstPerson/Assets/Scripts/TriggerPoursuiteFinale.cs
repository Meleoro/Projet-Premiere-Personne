using Creature;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPoursuiteFinale : MonoBehaviour
{
    [Header("Private Infos")]
    private List<Vector3> originalPosition = new List<Vector3>();

    [Header("References")]
    [SerializeField] private List<Transform> objectsToPutBack = new List<Transform>();
    [SerializeField] private CreatureTrigger triggerToReset;
    [SerializeField] private Transform creatureTPPos;
    [SerializeField] private CreatureWaypoints creatureToMove;
    [SerializeField] private List<GameObject> objectsToActivate = new List<GameObject>();

    private void Start()
    {
        for(int i = 0; i < objectsToPutBack.Count; i++)
        {
            originalPosition.Add(objectsToPutBack[i].position);
        }
    }


    public void ActivateTrigger()
    {
        // We put back the objects
        for (int i = 0; i < objectsToPutBack.Count; i++)
        {
            objectsToPutBack[i].GetComponent<Rigidbody>().isKinematic = true;
            objectsToPutBack[i].position = originalPosition[i];
            objectsToPutBack[i].GetComponent<Rigidbody>().isKinematic = false;
        }

        // We reactivate the trigger
        if(triggerToReset != null)
            triggerToReset.gameObject.SetActive(true);

        // We move the creature
        if (creatureToMove != null)
        {
            creatureToMove.transform.GetComponent<CreatureMover>().navMeshAgent.enabled = false;
            Vector3 moveDir = creatureTPPos.position - creatureToMove.transform.position;
            creatureToMove.transform.parent.transform.position += moveDir;

            creatureToMove.transform.GetComponent<CreatureMover>().tailIKScript.RebootTargets();

            creatureToMove.transform.GetComponent<CreatureMover>().navMeshAgent.enabled = true;
        }
       

        for (int i = 0; i < objectsToActivate.Count; i++)
        {
            objectsToActivate[i].SetActive(true);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.parent.GetComponent<HealthComponent>().currentTriggerPoursuiteF = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.parent.GetComponent<HealthComponent>().currentTriggerPoursuiteF = null;
        }
    }
}
