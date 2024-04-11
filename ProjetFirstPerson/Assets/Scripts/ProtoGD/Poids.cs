using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poids : MonoBehaviour
{
    public BalanceManager manager;
    public int poids;
    public int currentSpot;
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
        if (currentSpot == 0)
        {
            if (manager.poid1Free)
            {
                transform.position = manager.poid1Position.transform.position;
                manager.poid1Free = false;
                currentSpot = 1;
                manager.registedPoids1 = poids;
                manager.CheckPoids();
            }
            
            else if (manager.poid2Free)
            {
                transform.position = manager.poid2Position.transform.position;
                manager.poid2Free = false;
                currentSpot = 2;               
                manager.registedPoids2 = poids;
                manager.CheckPoids();
            }
        }
        else
        {
            transform.position = originalPos;
            
            if(currentSpot == 1)
                manager.poid1Free = true;
            
            if(currentSpot == 2)
                manager.poid2Free = true;
            
            currentSpot = 0;
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
