using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;


namespace Puzzle
{
    public class PuzzleInteract : MonoBehaviour, IInteractible
    {
        [Header("Parameters")]
        [SerializeField] private GameObject cameraPos;
        [SerializeField] private Vector3 desiredRotation;

        [Header("Private Infos")]
        private bool isInRange;

        [Header("References")]
        private BoxCollider puzzleCollider;
        private MoveComponent characterMoveScript;
        private CameraComponent characterCameraScript;



        private void Start()
        {
            characterMoveScript = CharacterManager.Instance.GetComponent<MoveComponent>();
            characterCameraScript = CharacterManager.Instance.GetComponent<CameraComponent>();
            TryGetComponent<BoxCollider>(out puzzleCollider);
        }


        private void Update()
        {
            HideUI();

            if (isInRange)
            {
                if (VerifyLookingItem())
                {
                    DisplayUI();
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
                GetOutInteraction();
        }




        #region Get In / Out Interaction

        public void GetInInteraction()
        {
            CameraManager.Instance.transform.parent.transform.position = cameraPos.transform.position;
            CameraManager.Instance.transform.parent.transform.rotation = Quaternion.Euler(desiredRotation);

            characterMoveScript.canMove = false;
            characterMoveScript.rb.isKinematic = true;
            CharacterManager.Instance.capsule.gameObject.SetActive(false);

            UIManager.Instance.HideHUD();

            puzzleCollider.enabled = false;

            characterCameraScript.LockedCursor(0);
            characterCameraScript.doMoveFeel = false;
            characterCameraScript.doFOVEffect = false;
            characterCameraScript.canMove = false;
            characterCameraScript.canRotate = false;
        }


        public void GetOutInteraction()
        {
            CameraManager.Instance.transform.parent.transform.position = characterCameraScript.wantedCameraPos.position;
            CameraManager.Instance.transform.parent.transform.rotation = Quaternion.identity;

            characterMoveScript.canMove = true;
            characterMoveScript.rb.isKinematic = false;
            CharacterManager.Instance.capsule.gameObject.SetActive(true);

            UIManager.Instance.ShowHUD();

            puzzleCollider.enabled = true;

            characterCameraScript.LockedCursor(2);
            characterCameraScript.doMoveFeel = true;
            characterCameraScript.doFOVEffect = true;
            characterCameraScript.canMove = true;
            characterCameraScript.canRotate = true;
        }

        #endregion


        #region Interface Related

        public void Interact()
        {
            GetInInteraction();
        }


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
                CharacterManager.Instance.interactibleAtRange = this;
            }
        }


        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isInRange = false;
                CharacterManager.Instance.interactibleAtRange = null;
            }
        }

        #endregion
    }
}