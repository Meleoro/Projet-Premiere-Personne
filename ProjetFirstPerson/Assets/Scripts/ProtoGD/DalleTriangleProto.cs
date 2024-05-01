using Proto;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dalleTriangleProto : MonoBehaviour
{
    private TriangleManager manager;
    public string myName;
    public int myIndex;
    public bool canMove;
    private MeshRenderer mesh;
    public Material MaterialOn;
    public Material MaterialOff;

    private void Start()
    {
        manager = transform.parent.GetComponent<TriangleManager>();
        mesh = GetComponent<MeshRenderer>();
    }

    
    private void OnMouseDown()
    {
        if(canMove)
            manager.ChangeDalleOrder(myIndex);
    }
    

    private void OnMouseEnter()
    {
        mesh.material = MaterialOn;
    }
    
    private void OnMouseExit()
    {
        mesh.material = MaterialOff;
    }
}
