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
        symbol = 0;
    }

    private void OnMouseEnter()
    {
        meshRenderer.material = MaterialHighlighted;
    }

    private void OnMouseExit()
    {
        meshRenderer.material = MaterialOff;
    }

    private void OnMouseDown()
    {
        if ((int)symbol < 3)
        {
            if (!isMoving)
            {
                StartCoroutine(RotateCylinder((int)symbol*-90));
                symbol += 1;
            }
        }
        else
        {
            if (!isMoving)
            {
                StartCoroutine(RotateCylinder(90));
                symbol = 0;
            }
        }

        manager.CheckIfWin();
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
            currentAngle = new Vector3(0,-90,Mathf.LerpAngle(currentAngle.z, newAngle, timer / rotateDuration));
            
            transform.localEulerAngles = new Vector3(0,-90,currentAngle.z);
            yield return null;
        }
        transform.localEulerAngles = new Vector3(0, -90, newAngle);
        isMoving = false;
    }
}