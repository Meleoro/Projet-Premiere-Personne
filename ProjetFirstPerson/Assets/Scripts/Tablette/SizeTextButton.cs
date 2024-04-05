using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SizeTextButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler 
{
    [Header("References")]
    [SerializeField] private AreaText areaText;
    [SerializeField] private string functionName;
    public void OnPointerDown(PointerEventData UpSizeButton)
    {
       areaText.ButttonPressed = true;
       areaText.SizeTextFunctionName = functionName;
    }
    public void OnPointerUp(PointerEventData UpSizeButton)
    {
       areaText.ButttonPressed = false;
    }
}
