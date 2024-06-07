using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ElementsOfBoard : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] public GameObject MyMovingObject;
    [SerializeField] public GameObject OptionPanel;
    [SerializeField] public bool isSelectedOneTime = false;
    [SerializeField] public bool isFavorite;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && transform.parent.CompareTag("Slot"))
        {
            OptionPanel.SetActive(false);
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && transform.parent.CompareTag("Slot"))
        {
            OptionPanel.SetActive(true);
        }
    }
}
