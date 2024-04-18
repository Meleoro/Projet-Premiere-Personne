using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceManager : MonoBehaviour
{
    public bool poid1Free;
    public bool poid2Free;
    public GameObject poid1Position;
    public GameObject poid2Position;
    public int registedPoids1;
    public int registedPoids2;
    //public Animation anim;

    /*private void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animation>();
    }*/

    public void CheckPoids()
    {
        if (!poid1Free && !poid2Free)
        {
            if (registedPoids1 > registedPoids2)
            {
                //anim.clip = anim["Gauche"].clip;
                //anim.Play();
                Debug.Log("gauche");
            }


            if (registedPoids2 > registedPoids1)
            {
                //anim.clip = anim["Droite"].clip;
                //anim.Play();
                Debug.Log("droite");
            }
        }
    }
}
