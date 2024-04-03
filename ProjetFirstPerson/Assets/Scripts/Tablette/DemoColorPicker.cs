using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace DmeoInfoGamerHubAssets
{
    public class DemoColorPicker : MonoBehaviour
    {
        public void SetColor(Color newColor)
        {
            GetComponent<Image>().color = newColor;
        }
    }
}