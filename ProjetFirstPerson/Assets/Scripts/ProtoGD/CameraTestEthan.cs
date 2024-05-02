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
    public GameObject tablette;
    public GameObject tabletteScreen;
    public GameObject UI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && !uIManager.isUIActive)
        {
            if (canInOut)
            {
                isAiming = !isAiming;
                anim.SetBool("in",true);
                StartCoroutine(WaitForUI());
            }
        }
        
        if (Input.GetKeyUp(KeyCode.Mouse1) && !uIManager.isUIActive)
        {
            if (canInOut)
            {
                UI.SetActive(false);
                StopCoroutine(WaitForUI());
                //tablette.SetActive(true);
                isAiming = !isAiming;
                anim.SetBool("in",false);
               // tablette.SetActive(true);
                UI.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isMenu)
            {
                canInOut = true;
                //tabletteScreen.SetActive(false);
                isMenu = !isMenu;
                anim.SetBool("Menu",false);
                isAiming = false;
            }
            else
            {
                canInOut = false;
                tabletteScreen.SetActive(true);
                isMenu = !isMenu;
                anim.SetBool("Menu",true);
                anim.SetBool("in",false);
                //tablette.SetActive(true);
                UI.SetActive(false);
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
            UI.SetActive(true);
            //tablette.SetActive(false);
           }
        }
        else
        {
            StopCoroutine(WaitForUI());
            //tablette.SetActive(true);
            UI.SetActive(false);
        }
    }
}
