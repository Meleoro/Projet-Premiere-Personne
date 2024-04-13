using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoPuzzleInteract : MonoBehaviour, IInteractible
{
    public GameObject cameraPos;
    public Vector3 desiredRotation;
    
    [Header("Références")]
    public GameObject associatedDoor;
    public GameObject hud;
    public CameraComponent cam;
    public MoveComponent move;
  

    [Header("Private Infos")] 
    public bool isInRange;
    private MeshRenderer playerCapsule;
    private BoxCollider collider;

    private void Start()
    {
        collider = GetComponent<BoxCollider>();
        playerCapsule = move.transform.GetChild(0).GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        HideUI();
        
        if (isInRange)
        {
            if (VerifyLookingItem())
            {
                DisplayUI();
                
                CharacterManager.Instance.interactibleAtRange = this;
            }
        }
        
        if(Input.GetKeyDown(KeyCode.Escape))
            GetOutInteraction();
    }
    
    public void Interact()
    {
        GetInInteraction();
    }
    
    public void GetInInteraction()
    {
        CameraManager.Instance.transform.parent.transform.position = cameraPos.transform.position;
        CameraManager.Instance.transform.parent.transform.rotation = Quaternion.Euler(desiredRotation);
        hud.SetActive(false);
        collider.enabled = false;
        cam.LockedCursor(0);

        move.canMove = false;
        playerCapsule.enabled = false;
        
        cam.doMoveFeel = false;
        cam.doFOVEffect = false;
        cam.canMove = false;
        cam.canRotate = false;
    }
    
    public void GetOutInteraction()
    {
        CameraManager.Instance.transform.parent.transform.position = cam.wantedCameraPos.position;
        CameraManager.Instance.transform.parent.transform.rotation = Quaternion.identity;
        hud.SetActive(true);
        cam.LockedCursor(2);
        collider.enabled = true;

        move.canMove = true;
        playerCapsule.enabled = true;

        
        cam.doMoveFeel = true;
        cam.doFOVEffect = true;
        cam.canMove = true;
        cam.canRotate = true;
    }

    public void OpenDoor()
    {
        associatedDoor.SetActive(false);
    }
    
    #region Interaction Functions
   
   
    
    private bool VerifyLookingItem()
    {
        Vector3 dirCamItem = transform.position - CameraManager.Instance.transform.position;
        Vector3 dirCamLook = CameraManager.Instance.transform.forward;

        Vector3 crossProduct = Vector3.Cross(dirCamItem, dirCamLook);

        if (crossProduct.sqrMagnitude < 0.3f)
            return true;

        else
            return false;
    }
    
    private void DisplayUI()
    {
        UIManager.Instance.DisplayInteractIcon();
    }

    private void HideUI()
    {
        UIManager.Instance.HideInteractIcon();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
        }
    }
    #endregion
}

    