using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SneakZone : MonoBehaviour
{
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
