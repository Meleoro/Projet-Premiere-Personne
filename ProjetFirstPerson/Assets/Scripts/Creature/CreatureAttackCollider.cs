using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureAttackCollider : MonoBehaviour
{
    [Header("Private Infos")]
    private Vector3 saveOffset;

    [Header("References")]
    [SerializeField] private Transform creatureTrRef;


    private void Start()
    {
        saveOffset = creatureTrRef.InverseTransformPoint(transform.position);
    }

    private void Update()
    {
        transform.position = creatureTrRef.TransformPoint(saveOffset);
        transform.rotation = creatureTrRef.rotation;
    }



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
