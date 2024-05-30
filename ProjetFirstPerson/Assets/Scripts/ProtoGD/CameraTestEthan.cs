using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraTestEthan : MonoBehaviour
{
    [SerializeField] UIManager uIManager; 
    public bool isAiming;
    public bool isMenu;
    public bool canInOut;
    public Animator anim;
    public GameObject NormalHUD;
    public GameObject tabletteScreen;
    public GameObject UIPhotoPlayer;
    public GameObject UIPhotoTablette;
    public GameObject UIMenuGeneral;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && !uIManager.isUIActive)
        {
            if (canInOut)
            {
                uIManager.InteractHUD.gameObject.SetActive(false);
                UIPhotoTablette.SetActive(true);
                isAiming = !isAiming;
                anim.SetBool("in",true);
                StartCoroutine(WaitForUI());
            }
        }
        
        if (Input.GetKeyUp(KeyCode.Mouse1) && !uIManager.isUIActive)
        {
            if (canInOut)
            {
                if(CharacterManager.Instance.isInteracting)
                    uIManager.InteractHUD.gameObject.SetActive(true);
                UIPhotoPlayer.SetActive(false);
                UIPhotoTablette.SetActive(false);
                StopCoroutine(WaitForUI());
                isAiming = !isAiming;
                anim.SetBool("in",false);
                UIPhotoTablette.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isMenu)
            {
                NormalHUD.SetActive(true);
                canInOut = true;
                isMenu = !isMenu;
                anim.SetBool("in",false);
                UIMenuGeneral.SetActive(false);
                isAiming = false;
            }
            else
            {
                NormalHUD.SetActive(false);
                UIMenuGeneral.SetActive(true);
                UIPhotoTablette.SetActive(false);
                canInOut = false;
                tabletteScreen.SetActive(true);
                isMenu = !isMenu;
                anim.SetBool("in",true);
                isAiming = false;
            }
        }
    }

    public IEnumerator WaitForUI()
    {
        if (isAiming)
        {
           yield return new WaitForSeconds(0.5f);
           if (isAiming)
           {
               UIPhotoPlayer.SetActive(true);
                UIMenuGeneral.SetActive(false);
                //tablette.SetActive(false);
           }
        }
        else
        {
            UIPhotoPlayer.SetActive(false);
            UIMenuGeneral.SetActive(false);
            StopCoroutine(WaitForUI());
        }
    }
}
