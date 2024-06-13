using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using static UnityEngine.GraphicsBuffer;


namespace Puzzle
{
    public class PuzzleInteract : MonoBehaviour, IInteractible
    {
        [Header("Parameters")]
        [SerializeField] private GameObject cameraPos;
        [SerializeField] private Vector3 desiredRotation;
        [SerializeField] private bool isTriangle;

        [Header("Private Infos")]
        private bool isInRange;
        private bool justInteracted;

        [Header("References")]
        private BoxCollider puzzleCollider;
        private MoveComponent characterMoveScript;
        private CameraComponent characterCameraScript;
        private BoardMenu boardMenu;



        private void Start()
        {
            cameraPos = transform.GetChild(0).gameObject;
            characterMoveScript = CharacterManager.Instance.GetComponent<MoveComponent>();
            characterCameraScript = CharacterManager.Instance.GetComponent<CameraComponent>();
            TryGetComponent<BoxCollider>(out puzzleCollider);

            boardMenu = GameObject.Find("TabletteManager").GetComponent<BoardMenu>();
            

            HideUI();
        }


        private void Update()
        {
            if (isInRange)
            {
                if (VerifyLookingItem())
                {
                    DisplayUI();
                    CharacterManager.Instance.interactibleAtRange = this;
                }
                else
                {
                    HideUI();
                    CharacterManager.Instance.interactibleAtRange = null;
                }
            }

            if (justInteracted)
            {
                justInteracted = false;
                return;
            }
            
            if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E)) && isInRange && CharacterManager.Instance.isInteracting)
                GetOutInteraction();
        }




        #region Get In / Out Interaction

        public void GetInInteraction()
        {
            justInteracted = true;
            
            CameraManager.Instance.transform.parent.transform.position = cameraPos.transform.position;
            CameraManager.Instance.transform.parent.transform.rotation = Quaternion.Euler(desiredRotation);

            characterMoveScript.canMove = false;
            characterMoveScript.rb.isKinematic = true;
            CharacterManager.Instance.capsule.GetComponent<MeshRenderer>().enabled = false;

            UIManager.Instance.HideHUD();
            UIManager.Instance.InteractHUD.gameObject.SetActive(true);

            puzzleCollider.enabled = false;

            characterCameraScript.LockedCursor(1);
            characterCameraScript.doMoveFeel = false;
            characterCameraScript.doFOVEffect = false;
            characterCameraScript.canMove = false;
            characterCameraScript.canRotate = false;

            boardMenu.favCanBeOpen = true;
            CharacterManager.Instance.isInteracting = true;
        }


        public void GetOutInteraction()
        {
            characterCameraScript.LockedCursor(2);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            CharacterManager.Instance.isInteracting = false;
            CameraManager.Instance.transform.parent.transform.position = characterCameraScript.wantedCameraPos.position;
            CameraManager.Instance.transform.parent.transform.rotation = Quaternion.identity;

            characterMoveScript.canMove = true;
            characterMoveScript.rb.isKinematic = false;
            CharacterManager.Instance.capsule.GetComponent<MeshRenderer>().enabled = true;

            UIManager.Instance.ShowHUD();
            UIManager.Instance.InteractHUD.gameObject.SetActive(false);

            puzzleCollider.enabled = true;

            if (UIManager.Instance.isUIActive)
            {
                StartCoroutine(UIManager.Instance.OpenMenu());
            }
           
            characterCameraScript.doMoveFeel = true;
            characterCameraScript.doFOVEffect = true;
            characterCameraScript.canMove = true;
            characterCameraScript.canRotate = true;

            isInRange = false;
            CharacterManager.Instance.interactibleAtRange = null;

            boardMenu.favCanBeOpen = false;

            HideUI();

            if (isTriangle)
            {
                TriangleManager triangle = transform.parent.GetComponent<TriangleManager>();
                if (triangle.selectedDalle1 is not null)
                {
                    DalleTriangle selectedDalle = triangle.selectedDalle1;
                    selectedDalle.GetComponent<MeshRenderer>().material = selectedDalle.MaterialOff;
                    triangle.selectedDalle1 = null;
                }
            }
        }

        #endregion


        #region Interface Related

        public void Interact()
        {
            GetInInteraction();
        }


        private bool VerifyLookingItem()
        {
            Vector3 dirCamItem = (transform.position - CameraManager.Instance.transform.position).normalized;
            Vector3 dirCamLook = CameraManager.Instance.transform.forward;

            float dotProduct = Vector3.Dot(dirCamItem, dirCamLook);

            if (dotProduct > 0.45f)
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
                CharacterManager.Instance.interactibleAtRange = this;
            }
        }


        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isInRange = false;
                CharacterManager.Instance.interactibleAtRange = null;

                HideUI();
            }
        }

        #endregion
    }
}
