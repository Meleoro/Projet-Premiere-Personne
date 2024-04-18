using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;

public class BoardMenu : MonoBehaviour
{
    [Header("List Objects In Board")]
    public List<GameObject> listBoardElement;
    [SerializeField] private GameObject MyBoard;

    [Header("Manipulation Variables")]
    public GameObject currentSelect;
    [SerializeField] private bool isRotating;
    [SerializeField] private Vector3 mousePos;

    [Header("Arrow Variable")]
    [SerializeField] private GameObject Arrow;
    public bool isCreateArrow;
    
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0) && isCreateArrow)
        {
            Instantiate(Arrow, Input.mousePosition, Quaternion.identity, MyBoard.transform);
            isCreateArrow = false;
        }

        // Quand le joueur clic, on check si un élément est séléctionné dans l'Event system et on fait une variable
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameObject NewGameObject = EventSystem.current.currentSelectedGameObject;
            if(NewGameObject == currentSelect && NewGameObject != null && currentSelect.CompareTag("MovingUI"))
            {
                    currentSelect.GetComponent<ElementsOfBoard>().OptionPanel.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(null);
                    currentSelect = null;
            }
            
            else if(EventSystem.current.currentSelectedGameObject == null)
            {
                for(int i = 0; i < listBoardElement.Count ; i++)
                 {
                    listBoardElement[i].GetComponentInChildren<ElementsOfBoard>().OptionPanel.SetActive(false);
                 }
            }

            else
            {
                currentSelect = EventSystem.current.currentSelectedGameObject;
            }

            
        }
        if(Input.GetKeyUp(KeyCode.Mouse0))
        {
            currentSelect = null;
            EventSystem.current.SetSelectedGameObject(null);
        }

        // La rotation des objets
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            mousePos = Input.mousePosition;
        }
        if(Input.GetKey(KeyCode.Mouse1) && UIManager.Instance.isUIActive)
        {
            if(currentSelect != null)
            {
                Transform HisMovingObject = currentSelect.GetComponent<ElementsOfBoard>().MyMovingObject.transform;
                HisMovingObject.localEulerAngles = new Vector3(0,0,Input.mousePosition.y);
                isRotating = true;
                Cursor.visible = false;
            }
        }
        if(Input.GetKeyUp(KeyCode.Mouse1) && UIManager.Instance.isUIActive)
        {
                isRotating = false;
                Cursor.visible = true;
                Mouse.current.WarpCursorPosition(mousePos);
        }

        // Si un élément de board est séléctionné, il suit le curseur de la souris
        if(currentSelect != null && currentSelect.CompareTag("MovingUI"))
        {
            Transform HisMovingObject = currentSelect.GetComponent<ElementsOfBoard>().MyMovingObject.transform;
            HisMovingObject.localScale += ( new Vector3(0.1f,0.1f,0) * Input.mouseScrollDelta.y );
            Debug.Log(currentSelect);

            if(!isRotating)
            {
                HisMovingObject.position = Input.mousePosition;
            }
            if(HisMovingObject.localScale.x <= 0.1f)
            {
                HisMovingObject.localScale = new Vector3(0.1f,0.1f,0);
            }
           else if(HisMovingObject.localScale.x >= 3f)
            {
                HisMovingObject.localScale = new Vector3(3f,3f,0);
            }    
        }
    }

    public void AddElementOnBoard(GameObject element)
    {
        GameObject newElement = Instantiate(element,new Vector3(Screen.width / 2, Screen.height / 2, 0),Quaternion.identity, MyBoard.transform);
        listBoardElement.Add(newElement);
    }
    public void AddBackgroundOnBoard(GameObject background)
    {
        GameObject newElement = Instantiate(background,new Vector3(Screen.width / 2, Screen.height / 2, 0),Quaternion.identity, MyBoard.transform);
        newElement.transform.SetSiblingIndex(0);
        listBoardElement.Add(newElement);
    }
    public void AddArrowOnBoardMode()
    {
        isCreateArrow = true;
    }
    public void DeleteElement(GameObject parent)
    {
        Destroy(parent);
    }
}
