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
        public Material MaterialHighlighted;
        public Material MaterialSelected;
        public Material MaterialOff;

        [Header("Public Infos")]
        public int currentIndex;
        public bool canMove;
        public bool isSelected;

        [Header("References")]
        [HideInInspector] public MeshRenderer meshRenderer;
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
            if(canMove && !isSelected)
                meshRenderer.material = MaterialHighlighted;
        }

        private void OnMouseExit()
        {
            if(!isSelected)
                meshRenderer.material = MaterialOff;
        }
    }
}
