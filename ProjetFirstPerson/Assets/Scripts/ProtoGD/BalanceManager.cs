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
    public Animator anim;
    

    public void CheckPoids()
    {
        if (poid1Free && poid2Free)
        {
            anim.SetBool("isGauche",false);
            anim.SetBool("isDroite",false);
        }
        
        if (poid1Free && !poid2Free)
        {
            anim.SetBool("isGauche",false);
            anim.SetBool("isDroite",true);
        }
        
        if (!poid1Free && poid2Free)
        {
            anim.SetBool("isGauche",true);
            anim.SetBool("isDroite",false);
        }
        
        if (!poid1Free && !poid2Free)
        {
            if (registedPoids1 > registedPoids2)
            {
                anim.SetBool("isGauche",true);
                anim.SetBool("isDroite",false);
                Debug.Log("gauche");
            }


            if (registedPoids2 > registedPoids1)
            {
                anim.SetBool("isGauche",false);
                anim.SetBool("isDroite",true);
                Debug.Log("droite");
            }
        }
    }
}
