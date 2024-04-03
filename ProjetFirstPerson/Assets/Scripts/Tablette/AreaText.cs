using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AreaText : MonoBehaviour
{
    public Text textZone;
    public bool ButttonPressed;
    [SerializeField] private GameObject ButtonParent;
    [SerializeField] private bool isFontStyle;
    [SerializeField] public string SizeTextFunctionName;
    // Start is called before the first frame update
    void Start()
    {
        ButtonParent.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(ButttonPressed)
        {
            Invoke(SizeTextFunctionName,0);
        }
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


        public void OpenFontSizePanel()
        {
            isFontStyle = !isFontStyle;
            if(!isFontStyle)
            {
                ButtonParent.SetActive(true);
            }
            else
            {
                ButtonParent.SetActive(false);
            }
        }
        public void BoldText(int fontStyleValue)
        {
            if(fontStyleValue == 0)
            {
                textZone.GetComponent<Text>().fontStyle = FontStyle.Normal;
            }
           else if(fontStyleValue == 1)
            {
                textZone.GetComponent<Text>().fontStyle = FontStyle.Bold;
            }
           else if(fontStyleValue == 2)
            {
                textZone.GetComponent<Text>().fontStyle = FontStyle.Italic;
            }
        }
}
