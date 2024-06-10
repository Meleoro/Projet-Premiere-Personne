using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SneakZone : MonoBehaviour
{
    public MeshRenderer _renderer;

    private void Start()
    {
        _renderer = GetComponentInChildren<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterManager.Instance.isInSneakZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterManager.Instance.isInSneakZone = false;
        }
    }
}
