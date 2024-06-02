using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureAttackCollider : MonoBehaviour
{
    [Header("Private Infos")]
    private Vector3 saveOffset;



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (CharacterManager.Instance.gameObject.TryGetComponent<HealthComponent>(out HealthComponent healthScript))
            {
                RaycastHit hit;
                if (!Physics.Raycast(transform.parent.position, (CharacterManager.Instance.transform.position - transform.parent.position).normalized, out hit, 
                    Vector3.Distance(CharacterManager.Instance.transform.position, transform.parent.position), LayerManager.Instance.groundLayer))
                {
                    healthScript.TakeDamage();
                }
            }
        }
    }
}
