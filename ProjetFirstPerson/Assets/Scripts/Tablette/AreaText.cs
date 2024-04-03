using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AreaText : MonoBehaviour, IPointerDownHandler, IPointerUpHandler 
{
    public Text textZone;
    public bool ButttonPressed;
    public string MyEvent;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(ButttonPressed)
        {
            Invoke(MyEvent,0);
            Debug.Log("ok");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
       ButttonPressed = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
       ButttonPressed = false;
    }

    // Si c'est un texte, tu peux augmenter et réduire la taille de la police
    public void UpTextSize()
        {
            textZone.GetComponent<Text>().fontSize += 1;
            if(textZone.GetComponent<Text>().fontSize >= 100)
            {
                textZone.GetComponent<Text>().fontSize = 100;
            }
        }

    public void DownTextSize()
        {
            textZone.GetComponent<Text>().fontSize -= 1;
            if(textZone.GetComponent<Text>().fontSize <= 6)
            {
                textZone.GetComponent<Text>().fontSize = 6;
            }
        }
}
