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

    private void Update()
    {
        if (currentSpot == 1)
        {
            transform.position = manager.poid1Position.transform.position;
        }
        
        if (currentSpot == 2)
        {
            transform.position = manager.poid2Position.transform.position;
        }
    }

    public void OnMouseDown()
    {
        if (!UIManager.Instance.isUIActive && !UIManager.Instance.cam.isAiming)
        {
            if (currentSpot == 0)
            {
                if (manager.poid1Free)
                {
                    //transform.position = manager.poid1Position.transform.position;
                    manager.poid1Free = false;
                    currentSpot = 1;
                    manager.registedPoids1 = poids;
                    manager.CheckPoids();
                }
            
                else if (manager.poid2Free)
                {
                    //transform.position = manager.poid2Position.transform.position;
                    manager.poid2Free = false;
                    currentSpot = 2;               
                    manager.registedPoids2 = poids;
                    manager.CheckPoids();
                }
                AudioManager.Instance.PlaySoundOneShot(2,1,0);
            }
            else
            {
                transform.position = originalPos;
            
                if(currentSpot == 1)
                    manager.poid1Free = true;
            
                if(currentSpot == 2)
                    manager.poid2Free = true;
            
                currentSpot = 0;
                manager.CheckPoids();
                AudioManager.Instance.PlaySoundOneShot(2,0,0);
            }
        }
    }

    public void OnMouseEnter()
    {
        if(!UIManager.Instance.isUIActive && !UIManager.Instance.cam.isAiming)
            mesh.material = MaterialOn;
    }

    public void OnMouseExit()
    { 
        if(!UIManager.Instance.isUIActive && !UIManager.Instance.cam.isAiming)
            mesh.material = MaterialOff;
    }
}
