using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SneakZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterManager.Instance.isHidden = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterManager.Instance.isHidden = false;
        }
    }
}
