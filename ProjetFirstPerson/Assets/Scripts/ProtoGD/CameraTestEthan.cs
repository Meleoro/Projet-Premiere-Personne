using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraTestEthan : MonoBehaviour
{
    public bool isIn;
    public bool isMenu;
    public bool canInOut;
    public Animator anim;
    public GameObject tablette;
    public GameObject tabletteScreen;
    public GameObject UI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (canInOut)
            {
                if (!isIn)
                {
                    isIn = !isIn;
                    anim.SetBool("in",true);
                    StartCoroutine(WaitForUI());
                }
                else
                {
                    isIn = !isIn;
                    anim.SetBool("in",false);
                    StartCoroutine(WaitForUI());
                }

            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isMenu)
            {
                canInOut = true;
                tabletteScreen.SetActive(false);
                isMenu = !isMenu;
                anim.SetBool("Menu",false);
                isIn = false;
            }
            else
            {
                canInOut = false;
                tabletteScreen.SetActive(true);
                isMenu = !isMenu;
                anim.SetBool("Menu",true);
                anim.SetBool("in",false);
                UI.SetActive(false);
                isIn = false;
            }
        }
    }

    public IEnumerator WaitForUI()
    {
        if (isIn)
        {
           yield return new WaitForSeconds(0.45f);
           UI.SetActive(true);
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            UI.SetActive(false);
        }
    }
}
