using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematiqueFinale : MonoBehaviour
{
    public GameObject light;
    public Animation doorAnim;
    public bool isOpen;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorAnim.clip = doorAnim["FermeturePorte"].clip;
            doorAnim.Play();
        }
    }

    public void DoSecondPart()
    {
        light.SetActive(true);
        doorAnim.clip = doorAnim["OuverturePorteFinale"].clip;
        doorAnim.Play();
    }
}
