using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace DmeoInfoGamerHubAssets
{
    public class DemoColorPicker : MonoBehaviour
    {
        public void SetColorBG(Color newColor)
        {
            GetComponent<Image>().color = newColor;  
        }
        public void SetColorText(Color newColor)
        {
            transform.GetChild(1).GetComponent<Text>().color = newColor;  
        }
    }
}