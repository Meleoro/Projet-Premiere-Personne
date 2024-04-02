using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AlbumSlot : MonoBehaviour
{
    public Transform currentSlotSelected;
    public bool isFollowCursor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(currentSlotSelected != null  && isFollowCursor)
        {
            MoveObject(currentSlotSelected);
        }
    }

    public void SelectSlot()
    {
        isFollowCursor = !isFollowCursor;
        currentSlotSelected = EventSystem.current.currentSelectedGameObject.transform.parent;
    }

    public void MoveObject(Transform MyCurrentSlotSelected)
    {
        MyCurrentSlotSelected.transform.position = Input.mousePosition;
    }
}
