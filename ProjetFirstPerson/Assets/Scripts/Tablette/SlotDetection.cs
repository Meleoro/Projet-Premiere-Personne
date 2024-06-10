using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlotDetection : MonoBehaviour, IPointerDownHandler
{
    private EventSystem m_EventSystem;
    public Transform myObject;
    private Vector3 LastPos;
    public float speedRotating;
    void Start()
    {
        m_EventSystem = EventSystem.current;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            myObject = transform.parent;
        }
    }

    void Update() 
    {
        if(Input.GetKey(KeyCode.Mouse1))
        {
            if(myObject != null && m_EventSystem.currentSelectedGameObject != gameObject)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            
            float xValue = Input.GetAxis("Mouse X");
            if(Input.GetAxis("Mouse X") != 0)
            {
                print("Mouse moved left");
                myObject.localEulerAngles += (new Vector3(0,0,-xValue)) * speedRotating;
            }
            }
        } 

        if(Input.GetKeyUp(KeyCode.Mouse1))
        {
            myObject = null;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
