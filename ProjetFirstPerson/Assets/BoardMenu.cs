using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BoardMenu : MonoBehaviour
{
    [Header("List Objects In Board")]
    public List<GameObject> TextAreaElement;
    [SerializeField] private GameObject MyBoard;

    [Header("Manipulation Variables")]
    public GameObject currentSelect;
    [SerializeField] private bool isRotating;
    [SerializeField] private Vector3 mousePos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Quand le joueur clic, on check si un élément est séléctionné dans l'Event system et on fait une variable
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(currentSelect != null)
            {
                if(currentSelect.transform.parent.parent.CompareTag("TextArea"))
                {
                    currentSelect.transform.parent.parent.GetComponent<AreaText>().OptionTextPanel.SetActive(true);
                }
                currentSelect = null;
                EventSystem.current.SetSelectedGameObject(null);
                return;
            }
            if(EventSystem.current.currentSelectedGameObject != null)
            {
                currentSelect = EventSystem.current.currentSelectedGameObject;
                return;
            }

            if(EventSystem.current.currentSelectedGameObject == null)
            {
                for(int i = 0; i < TextAreaElement.Count ; i++)
                {
                    Debug.Log("ok");
                    TextAreaElement[i].GetComponent<AreaText>().OptionTextPanel.SetActive(false);
                }
                return;
            }
        }

        // La rotation des objets
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            mousePos = Input.mousePosition;
        }
        if(Input.GetKey(KeyCode.Mouse1))
        {
            if(currentSelect != null)
            {
                Transform HisParent = currentSelect.GetComponent<ElementsOfBoard>().MyParent.transform;
                HisParent.localEulerAngles = new Vector3(0,0,Input.mousePosition.y);
                isRotating = true;
                Cursor.visible = false;
            }
        }
        if(Input.GetKeyUp(KeyCode.Mouse1))
        {
                isRotating = false;
                Cursor.visible = true;
                Mouse.current.WarpCursorPosition(mousePos);
        }

        // Si un élément de board est séléctionné, il suit le curseur de la souris
        if(currentSelect != null && currentSelect.CompareTag("MovingUI"))
        {
            Transform HisParent = currentSelect.GetComponent<ElementsOfBoard>().MyParent.transform;
            HisParent.localScale += ( new Vector3(0.1f,0.1f,0) * Input.mouseScrollDelta.y );
            Debug.Log(currentSelect);

            if(!isRotating)
            {
                HisParent.position = Input.mousePosition;
            }
            if(HisParent.localScale.x <= 0.1f)
            {
                HisParent.localScale = new Vector3(0.1f,0.1f,0);
            }
           else if(HisParent.localScale.x >= 3f)
            {
                HisParent.localScale = new Vector3(3f,3f,0);
            }    
        }
    }

    public void AddTextOnBoard(GameObject TextArea)
    {
        GameObject newTextArea = Instantiate(TextArea,new Vector3(Screen.width / 2, Screen.height / 2, 0),Quaternion.identity, MyBoard.transform);
        TextAreaElement.Add(newTextArea);
    }
}
