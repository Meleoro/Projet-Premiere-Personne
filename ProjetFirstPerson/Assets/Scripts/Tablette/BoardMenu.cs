using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Unity.VisualScripting;
using System;
using System.Linq;

public class BoardMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CameraComponent cameraComponent;
    [SerializeField] private UIManager uIManager;
    [SerializeField] private SCR_UiDrag sCR_UiDrag;

    [Header("List Objects In Board")]
    public List<GameObject> listBoardElement;
    [SerializeField] private GameObject MyBoard;
    [SerializeField] private GameObject BoardPanel;

    [Header("Manipulation Variables")]
    public GameObject currentSelect;
    [SerializeField] private bool isRotating;
    [SerializeField] private Vector3 mousePos;

    [Header("Arrow Variable")]
    [SerializeField] private GameObject Arrow;
    [SerializeField] private int OffsetX, OffsetY;
    public bool isCreateArrow;

    [Header("Favorite Variable")]
    [SerializeField] public bool favCanBeOpen;
    [SerializeField] private Animator animator;
    [SerializeField] private bool isOpen;
    [SerializeField] private Image theFavImageSelected;
    [SerializeField] private Transform contentFavoritePhoto;

    
    // Update is called once per frame
    void Update()
    {
        // Desactive touts les options panel du board
        if(EventSystem.current.currentSelectedGameObject == MyBoard)
        {
            for(int i = 0; i < listBoardElement.Count ; i++)
                 {
                    listBoardElement[i].GetComponentInChildren<ElementsOfBoard>().OptionPanel.SetActive(false);
                 }
        }
        if(Input.GetKeyDown(KeyCode.Mouse0) && isCreateArrow)
        {
            Instantiate(Arrow, Input.mousePosition, Quaternion.identity, MyBoard.transform);
            isCreateArrow = false;
        }

        // Quand le joueur clic, on check si un élément est séléctionné dans l'Event system et on fait une variable
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {   
            if(EventSystem.current.currentSelectedGameObject == null)
            {
                listBoardElement.RemoveAll(item => item == null);
            }

            else
            {
                currentSelect = EventSystem.current.currentSelectedGameObject;
                if(currentSelect.CompareTag("MovingUI"))
                {
                    currentSelect.GetComponentInChildren<ElementsOfBoard>().isSelectedOneTime = true;
                }
                if(currentSelect.transform.parent.parent.name == "TextSpace(Clone)")
                {
                    currentSelect.GetComponentInChildren<ElementsOfBoard>().OptionPanel.SetActive(false);
                }
            }

            
        }
        if(Input.GetKeyUp(KeyCode.Mouse0) && currentSelect != null)
        {
            if(currentSelect.transform.parent.parent.name == "TextSpace(Clone)")
                {
                    currentSelect.GetComponentInChildren<ElementsOfBoard>().OptionPanel.SetActive(true);
                }
            currentSelect = null;
            EventSystem.current.SetSelectedGameObject(null);
        }

        // La rotation des objets
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            mousePos = Input.mousePosition;
        }
        if(Input.GetKeyUp(KeyCode.Mouse1) && UIManager.Instance.isUIActive)
        {
                isRotating = false;
                Cursor.visible = true;
                Mouse.current.WarpCursorPosition(mousePos);
        }

        // Si un élément de board est séléctionné, il suit le curseur de la souris
        if(currentSelect != null && currentSelect.CompareTag("MovingUI") && !currentSelect.GetComponent<ElementsOfBoard>().isFavorite)
        {
            Transform HisMovingObject = currentSelect.GetComponent<ElementsOfBoard>().MyMovingObject.transform;
            if(!isRotating)
            {
                HisMovingObject.position = Input.mousePosition;
            }
        }

        // OpenFavMenu
        if(Input.GetKeyDown(KeyCode.F) && favCanBeOpen )
        {
            OpenFavoriteMenu();
        }
    }

    public void AddElementOnBoard(GameObject element)
    {
        GameObject newElement = Instantiate(element,new Vector3(Screen.width / 2, Screen.height / 2, 0),Quaternion.identity, MyBoard.transform);
        listBoardElement.Add(newElement);
        for(int i = 0 ; i < listBoardElement.Count; i++)
        {
            if(!listBoardElement[i].GetComponentInChildren<ElementsOfBoard>().isSelectedOneTime)
            {
                OffsetX += 20;
                OffsetY += 20;
                listBoardElement[i].transform.position = new Vector3(Screen.width / 2 + OffsetX, Screen.height / 2 + OffsetY, 0);
            }
        }
        OffsetX = 0;
        OffsetY = 0;
        if(newElement.CompareTag("Slot"))
        {
            // Delete
            UnityAction UaDelete;
            UaDelete = new UnityAction(() => DeleteElement(newElement,gameObject.GetComponent<BoardMenu>()));
            newElement.GetComponentInChildren<ElementsOfBoard>().OptionPanel.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(UaDelete);
            // Favoris
            UnityAction UaFavorite;
            UaFavorite = new UnityAction(() => AddFavoritePhoto(newElement.transform,contentFavoritePhoto));
            newElement.GetComponentInChildren<ElementsOfBoard>().OptionPanel.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(UaFavorite);
        }
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
    public void DeleteElement(GameObject parent, BoardMenu boardMenu)
    {
        for(int i = 0 ; i < boardMenu.listBoardElement.Count; i++)
        {
            if(boardMenu.listBoardElement[i].gameObject == parent)
            {
                boardMenu.listBoardElement.RemoveAt(i);
            }
        }
        AudioManager.Instance.PlaySoundOneShot(1, 18, 0);
        Destroy(parent);
    }

    public void AddFavoritePhoto(Transform target, Transform parent)
    {
        Transform favElement = Instantiate(target,new Vector3(0,0,0), Quaternion.identity, parent);
        favElement.GetComponentInChildren<ElementsOfBoard>().isFavorite = true;
        favElement.GetComponentInChildren<Button>().onClick.AddListener(SelectPhotoToFavoriteMod);
    }
    private void OpenFavoriteMenu()
    {
        isOpen = !isOpen;
        if(isOpen)
        {
            animator.Play("OpenMenu");
            cameraComponent.LockedCursor(1);
        }
        else
        {
            animator.Play("CloseMenu");
            if(!uIManager.isUIActive)
            {
                cameraComponent.LockedCursor(2);
            }
        }
    }

    void SelectPhotoToFavoriteMod()
    {
        GameObject MyButton = EventSystem.current.currentSelectedGameObject;
        Image myImage = MyButton.transform.parent.GetComponent<SlotAlbum>().SlotImage;
        theFavImageSelected = GameObject.Find("FavImageSelected").GetComponent<Image>();
        theFavImageSelected.sprite = myImage.sprite;
    }

    public void SetZoomBoard()
    {
        float zoomValue = sCR_UiDrag.zoomSlider.value;
        MyBoard.transform.localScale = new Vector3(zoomValue, zoomValue, zoomValue);
    }
}
