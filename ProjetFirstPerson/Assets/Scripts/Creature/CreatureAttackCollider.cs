using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureAttackCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (CharacterManager.Instance.gameObject.TryGetComponent<HealthComponent>(out HealthComponent healthScript))
            {
                healthScript.TakeDamage();
            }
        }
    }
}
