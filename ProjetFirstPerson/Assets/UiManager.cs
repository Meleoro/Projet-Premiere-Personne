using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CameraComponent cameraComponent;

    [Header("UI Variables")]
    [SerializeField] private GameObject Album;
    public bool isUIActive;

    GUIStyle style = new GUIStyle();

    public GameObject currentSelect;
    // Start is called before the first frame update
    void Start()
    {
       style.alignment = TextAnchor.MiddleCenter;
       Album.SetActive(false);
    }
    void OnGUI()
    {
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "This is a box", style);
    }

    // Update is called once per frame
    void Update()
    {
        // Quand le joueur clic, on check si un élément est séléctionné dans l'Event system et on fait une variable
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(currentSelect != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                currentSelect = null;
            }
            else
            {
                currentSelect = EventSystem.current.currentSelectedGameObject;
            }
        }
        // Si un élément de board est séléctionné, il suit le curseur de la souris
        if(currentSelect != null)
        {
            currentSelect.GetComponent<ElementsOfBoard>().MyParent.transform.position = Input.mousePosition;
        }

        // Test Ouvrir Album
        if(Input.GetKeyDown(KeyCode.Keypad3))
        {
             if (!Album.activeSelf)
            {
                cameraComponent.LockedCursor(false);
                Album.SetActive(true);
                isUIActive = true;
            }
            else
            {
                cameraComponent.LockedCursor(true);
                Album.SetActive(false);
                isUIActive = false;
            }
        } 
    }
}
