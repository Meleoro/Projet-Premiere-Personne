using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
 
public class SCR_UiDrag : MonoBehaviour
{
    public GameObject ui_canvas;
    GraphicRaycaster ui_raycaster;
    PointerEventData click_data;
    List<RaycastResult> click_results;
 
    List<GameObject> clicked_elements;
 
    bool dragging = false;
    GameObject drag_element;
 
    Vector3 mouse_position;
    Vector3 previous_mouse_position;
    Vector3 dragMousePosition;

    [Header("Clamp Values")]
    public float ClampX;
    public float ClampY;
    public GameObject board;
    private RectTransform RectBoard;
    public GameObject CenterMouse;
    [Header("Ui References")]
    [SerializeField] public Slider zoomSlider;
 
    void Start()
    {
        ui_raycaster = ui_canvas.GetComponent<GraphicRaycaster>();
        click_data = new PointerEventData(EventSystem.current);
        click_results = new List<RaycastResult>();
        clicked_elements = new List<GameObject>();

        RectBoard = board.GetComponent<RectTransform>();
        zoomSlider.value = 1;
    }
 
    void Update()
    {
        MouseDragUi();

        if(Input.mouseScrollDelta.y > 0)
        {
            zoomSlider.value = RectBoard.localScale.x;
            Vector3 currentScale = RectBoard.localScale;
            ScaleAround(board,CenterMouse.transform.localPosition / 2, currentScale += new Vector3(0.1f,0.1f,0) * Input.mouseScrollDelta.y);
        }
        if(Input.mouseScrollDelta.y < 0)
        {
            zoomSlider.value = RectBoard.localScale.x;
            Vector3 currentScale = RectBoard.localScale;
            ScaleAround(board,CenterMouse.transform.localPosition / 2, currentScale += new Vector3(0.1f,0.1f,0) * Input.mouseScrollDelta.y);
        }

        // Clamp des valeurs de position
            var pos =  RectBoard.transform.localPosition;

            pos.x =  Mathf.Clamp(pos.x, -ClampX, ClampX);
            pos.y =  Mathf.Clamp(pos.y, -ClampY, ClampY);

            RectBoard.transform.localPosition = pos;

             ClampX = 3000 * RectBoard.localScale.x;
             ClampY = 2000 * RectBoard.localScale.y;
             if(RectBoard.localScale.x <= 0.6f)
            {
                RectBoard.localScale = new Vector3(0.6f,0.6f,0);
            }
           else if(RectBoard.localScale.x >= 3f)
            {
                RectBoard.localScale = new Vector3(3f,3f,0);
            }

            // Fix Mouse1 return position bug
            if(Input.GetKeyUp(KeyCode.Mouse1))
            {
                StartCoroutine(SetMousePos());
            }
    }
     private IEnumerator SetMousePos()
    {
        yield return new WaitForSeconds(0.0001f);
        Mouse.current.WarpCursorPosition(dragMousePosition);
    }
 
    void MouseDragUi()
    {
        /** Houses the main mouse dragging logic. **/
 
        mouse_position = Mouse.current.position.ReadValue();
 
        if(Mouse.current.rightButton.wasPressedThisFrame)
        {
            DetectUi();
        }
 
        if(Mouse.current.rightButton.isPressed && dragging)
        {
            DragElement();
            dragMousePosition = Input.mousePosition;
        }
        else
        {
            dragging = false;
        }
 
        previous_mouse_position = mouse_position;
    }
 
    void DetectUi()
    {
        /** Detect if the mouse has been clicked on a UI element, and begin dragging **/
 
        GetUiElementsClicked();
 
        if(clicked_elements.Count > 0)
        {
            dragging = true;
            drag_element = clicked_elements[0];
        }
    }
 
    void GetUiElementsClicked()
    {
        /** Get all the UI elements clicked, using the current mouse position and raycasting. **/
 
        click_data.position = mouse_position;
        click_results.Clear();
        ui_raycaster.Raycast(click_data, click_results);
 
        // Optimised version
        //clicked_elements = (from result in click_results select result.gameObject).ToList();
 
        // Foreach version
        clicked_elements.Clear();
        foreach(RaycastResult result in click_results)
        {
            clicked_elements.Add(result.gameObject);
        }
 
    }
 
    void DragElement()
    {
        /** Drag a UI element across the screen based on the mouse movement. **/
 
        RectTransform element_rect = drag_element.GetComponent<RectTransform>();

        if(element_rect.CompareTag("Board"))
        {
            // Déplacement de l'objet
            Vector2 drag_movement = mouse_position - previous_mouse_position;
            element_rect.anchoredPosition = element_rect.anchoredPosition + drag_movement;
        }
    }

   public void ScaleAround(GameObject target, Vector3 pivot, Vector3 newScale)
   {
       if (newScale == target.transform.localScale) return; 
    
        Vector3 A = target.transform.localPosition;
        Vector3 B = pivot;

        Vector3 C = A - B; // diff from object pivot to desired pivot/origin

        float RS = newScale.x / target.transform.localScale.x; // relataive scale factor

        // calc final position post-scale
        Vector3 FP = B + C * RS;

        // finally, actually perform the scale/translation
        target.transform.localScale = newScale;
        target.transform.localPosition = FP;
    }
}