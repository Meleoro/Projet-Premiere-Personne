using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AreaText : MonoBehaviour
{
    public Text textZone;
    public bool ButttonPressed;
    [SerializeField] public GameObject OptionTextPanel;
    [SerializeField] private GameObject ButtonParentFontStyle, ButtonParentTextAlignement, ColorChartPanelBG, ColorChartPanelText;
    [SerializeField] private bool isFontStyle, isAlignement, isColoring;
    [HideInInspector] public string SizeTextFunctionName;
    // Start is called before the first frame update
    void Start()
    {
        ButtonParentTextAlignement.SetActive(false);
        ButtonParentFontStyle.SetActive(false);
        ColorChartPanelBG.SetActive(false);
        ColorChartPanelText.SetActive(false);
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
            if(isFontStyle)
            {
                ButtonParentFontStyle.SetActive(true);
            }
            else
            {
                ButtonParentFontStyle.SetActive(false);
            }
        }
        
        public void FontStyleText(int fontStyleValue)
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

        // Text Alignement
        public void OpenAlignementPanel()
        {
            isAlignement = !isAlignement;
            if(isAlignement)
            {
                ButtonParentTextAlignement.SetActive(true);
            }
            else
            {
                ButtonParentTextAlignement.SetActive(false);
            }
        }

        public void AlignementText(int AlignementValue)
        {
            if(AlignementValue == 0)
            {
                textZone.GetComponent<Text>().alignment = TextAnchor.UpperLeft;
            }
           else if(AlignementValue == 1)
            {
                textZone.GetComponent<Text>().alignment = TextAnchor.UpperCenter;
            }
           else if(AlignementValue == 2)
            {
                textZone.GetComponent<Text>().alignment = TextAnchor.UpperRight;
            }
        }

        // Couleur des textes
        public void OpenColorPanel(int value)
        {
            isColoring = !isColoring;
            if(isColoring)
            {
              if(value == 0)
              {
                ColorChartPanelBG.SetActive(true);
                ColorChartPanelText.SetActive(false);
              }
              if(value == 1)
              {
                ColorChartPanelBG.SetActive(false);
                ColorChartPanelText.SetActive(true);
              }
            }

            else
            {
                ColorChartPanelBG.SetActive(false);
                ColorChartPanelText.SetActive(false);
            }
        }
}
