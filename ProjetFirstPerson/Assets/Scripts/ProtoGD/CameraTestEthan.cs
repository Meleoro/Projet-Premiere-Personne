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
                AudioManager.Instance.PlaySoundOneShot(1,21,0);
                uIManager.InteractHUD.gameObject.SetActive(false);
                UIPhotoTablette.SetActive(true);
                anim.SetBool("in",true);
                StartCoroutine(WaitForUI());
            }
        }
        
        if (Input.GetKeyUp(KeyCode.Mouse1) && !uIManager.isUIActive)
        {
            if (canInOut)
            {
                AudioManager.Instance.PlaySoundOneShot(1,22,0);
                isAiming = false;
                if(CharacterManager.Instance.isInteracting)
                    uIManager.InteractHUD.gameObject.SetActive(true);
                UIPhotoPlayer.SetActive(false);
                UIPhotoTablette.SetActive(false);
                StopCoroutine(WaitForUI());
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
        }
    }

    public IEnumerator WaitForUI()
    {
        yield return new WaitForSeconds(0.5f);
        isAiming = true;
        
           if (isAiming && Input.GetKey(KeyCode.Mouse1))
           {
                UIPhotoPlayer.SetActive(true);
                UIMenuGeneral.SetActive(false);
           }
           else
           {
               isAiming = false;
               UIPhotoPlayer.SetActive(false);
                UIMenuGeneral.SetActive(false);
                StopCoroutine(WaitForUI());
           }
    }

    public void AutoQuitPhoto()
    {
        AudioManager.Instance.PlaySoundOneShot(1,22,0);
        isAiming = false;
        if(CharacterManager.Instance.isInteracting)
            uIManager.InteractHUD.gameObject.SetActive(true);
        UIPhotoPlayer.SetActive(false);
        UIPhotoTablette.SetActive(false);
        StopCoroutine(WaitForUI());
        anim.SetBool("in",false);
        UIPhotoTablette.SetActive(false);
    }
}
