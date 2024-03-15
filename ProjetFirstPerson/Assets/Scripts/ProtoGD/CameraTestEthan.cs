using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraTestEthan : MonoBehaviour
{
    public bool isIn;
    public GameObject tablette;
    public GameObject UI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!isIn)
            {
                isIn = !isIn;
                tablette.GetComponent<Animator>().SetBool("in",true);
                StartCoroutine(WaitForUI());
            }
            else
            {
                isIn = !isIn;
                tablette.GetComponent<Animator>().SetBool("in",false);
                StartCoroutine(WaitForUI());
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
