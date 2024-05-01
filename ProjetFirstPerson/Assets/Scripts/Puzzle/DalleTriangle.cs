using Puzzle;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DalleTriangle : MonoBehaviour
{
    [Header("Parameters")]
    public DalleSymbols dalleSymbol;
    public Material MaterialOn;
    public Material MaterialOff;

    [Header("Public Infos")]
    public int currentIndex;

    [Header("References")]
    private MeshRenderer meshRenderer;
    private TriangleManager triangleManager;




    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }


    private void OnMouseDown()
    {
        triangleManager.ChangeDalleOrder(myIndex);
    }


    private void OnMouseEnter()
    {
        meshRenderer.material = MaterialOn;
    }

    private void OnMouseExit()
    {
        meshRenderer.material = MaterialOff;
    }
}
