using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylindrePuzzle : MonoBehaviour
{
    public PorteCylindre.CylindreSymbols symbol;
    [SerializeField] private Material MaterialHighlighted;
    [SerializeField] private Material MaterialOff;
    [SerializeField] private float rotateDuration;
    public bool isMoving;
    
    [Header("References")]
    [HideInInspector] public MeshRenderer meshRenderer;
    private PorteCylindre manager;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        manager = GetComponentInParent<PorteCylindre>();
        //symbol = 0;
    }

    private void OnMouseEnter()
    {
        if (!UIManager.Instance.isUIActive && !UIManager.Instance.cam.isAiming)
            meshRenderer.material = MaterialHighlighted;
    }

    private void OnMouseExit()
    {
        if (!UIManager.Instance.isUIActive && !UIManager.Instance.cam.isAiming)
            meshRenderer.material = MaterialOff;
    }

    private void OnMouseDown()
    {
        if (!isMoving && !UIManager.Instance.isUIActive && !UIManager.Instance.cam.isAiming)
        {
            if ((int)symbol < 3) //tout sauf feuille
            {
                Debug.Log("normal");
                StartCoroutine(RotateCylinder(((int)symbol+1)*-80));
                symbol += 1;
            }
            else // feuille
            {
                Debug.Log("last");
                StartCoroutine(RotateCylinder(0));
                symbol = 0;
            }  
            manager.CheckIfWin();
            
            if(symbol == PorteCylindre.CylindreSymbols.Feuille)
                AudioManager.Instance.PlaySoundOneShot(2,4,0);
            if(symbol == PorteCylindre.CylindreSymbols.Pales)
                AudioManager.Instance.PlaySoundOneShot(2,5,0);
            if(symbol == PorteCylindre.CylindreSymbols.Fleur)
                AudioManager.Instance.PlaySoundOneShot(2,6,0);
            if(symbol == PorteCylindre.CylindreSymbols.Soleil)
                AudioManager.Instance.PlaySoundOneShot(2,7,0);
        }
    }

    IEnumerator RotateCylinder(int newAngle)
    {
        if(isMoving) yield break;
        isMoving = true;
        float timer = 0;

        Vector3 currentAngle = transform.localEulerAngles;

        while (timer < rotateDuration)
        {
            timer += Time.deltaTime;
            currentAngle = new Vector3(0,90,Mathf.LerpAngle(currentAngle.z, newAngle, timer / rotateDuration));
            
            transform.localEulerAngles = new Vector3(0,90,currentAngle.z);
            yield return null;
        }
        transform.localEulerAngles = new Vector3(0, 90, newAngle);
        isMoving = false;
    }
}
