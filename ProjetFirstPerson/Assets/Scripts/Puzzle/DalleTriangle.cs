using Puzzle;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace Puzzle
{
    public class DalleTriangle : MonoBehaviour
    {
        [Header("Parameters")]
        public DalleSymbols dalleSymbol;
        public Material MaterialOn;
        public Material MaterialOff;

        [Header("Public Infos")]
        public int currentIndex;
        public bool canMove;

        [Header("References")]
        private MeshRenderer meshRenderer;
        private TriangleManager triangleManager;




        private void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            triangleManager = GetComponentInParent<TriangleManager>();
        }


        private void OnMouseDown()
        {
            if(canMove)
             triangleManager.SelectDalle(this);
        }


        private void OnMouseEnter()
        {
            if(canMove)
                meshRenderer.material = MaterialOn;
        }

        private void OnMouseExit()
        {
            meshRenderer.material = MaterialOff;
        }
    }
}
