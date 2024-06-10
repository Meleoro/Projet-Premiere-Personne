using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicationTouche : MonoBehaviour
{
    public CanvasGroup indicTouche;
    public float fadeSpeed;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            TextOnOff(true);
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
            TextOnOff(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && UIManager.Instance.canMenu)
        {
            TextOnOff(false);
            gameObject.SetActive(false);
        }
    }

    void TextOnOff(bool TurnOn)
    {
        if (TurnOn)
        {
            while (indicTouche.alpha < 1)
            {
                indicTouche.alpha += Time.deltaTime*fadeSpeed;
            }

            indicTouche.alpha = 1;
        }
        else
        {
            while (indicTouche.alpha > 0)
            {
                indicTouche.alpha -= Time.deltaTime*fadeSpeed;
            }  
            indicTouche.alpha = 0;
        }
        
    }
}
