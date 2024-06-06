using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlotDetection : MonoBehaviour, IPointerDownHandler
{
    public Transform myObject;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            myObject = transform;
        }
    }

    void Update() 
    {
        if(myObject != null && !GetComponent<ElementsOfBoard>().isFavorite)
        {
            Transform HisMovingObject = GetComponent<ElementsOfBoard>().MyMovingObject.transform;
            HisMovingObject.localEulerAngles = new Vector3(0,0,-Input.mousePosition.x);
            Cursor.visible = false;
        }      
        if(Input.GetKeyUp(KeyCode.Mouse1))
        {
            myObject = null;
            Cursor.visible = true;
        }
    }
}
