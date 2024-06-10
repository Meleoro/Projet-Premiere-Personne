using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SneakZone : MonoBehaviour
{
    public MeshRenderer _renderer;
    public bool isMaster;
    private void Start()
    {
        if(!isMaster)
            _renderer = GetComponentInChildren<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isMaster)
        {
            CharacterManager.Instance.isInSneakZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isMaster)
        {
            CharacterManager.Instance.isInSneakZone = false;
        }
    }
}
