using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dalleTriangleProto : MonoBehaviour
{
    private TriangleManager manager;
    public string myName;
    public int myIndex;
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
