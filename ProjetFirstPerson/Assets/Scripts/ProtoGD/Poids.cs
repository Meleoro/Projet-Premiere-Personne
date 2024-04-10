using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poids : MonoBehaviour
{
    public BalanceManager manager;
    public int poids;
    public bool isOnSpot;
    public Material MaterialOn;
    public Material MaterialOff;
    private MeshRenderer mesh;
    private Vector3 originalPos;

    void Start()
    {
        originalPos = transform.position;
        mesh = GetComponent<MeshRenderer>();
    }

    public void OnMouseDown()
    {
        if (!isOnSpot)
        {
            if (manager.poid1Free)
            {
                transform.position = manager.poid1Position.transform.position;
                manager.poid1Free = false;
                isOnSpot = true;
                manager.registedPoids1 = poids;
                manager.CheckPoids();
            }
            
            else if (manager.poid2Free)
            {
                transform.position = manager.poid2Position.transform.position;
                manager.poid2Free = false;
                isOnSpot = true;
                manager.registedPoids2 = poids;
                manager.CheckPoids();
            }
        }
        else
        {
            transform.position = originalPos;
            manager.poid1Free = true;
            manager.poid2Free = true;
            isOnSpot = false;
        }
    }

    public void OnMouseEnter()
    {
        mesh.material = MaterialOn;
    }

    public void OnMouseExit()
    {
        mesh.material = MaterialOff;
    }
}
