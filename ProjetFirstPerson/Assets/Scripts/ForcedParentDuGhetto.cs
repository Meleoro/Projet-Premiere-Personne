using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceParentDuGhetto : MonoBehaviour
{
    [SerializeField] private Transform forcedParent;
    private Vector3 originalOffset;
    private Vector3 angleOffset;


    private void Start()
    {
        originalOffset = transform.position - forcedParent.position;
        angleOffset = -transform.eulerAngles + forcedParent.eulerAngles;
    }

    private void Update()
    {
        transform.position = forcedParent.position + originalOffset;

        transform.rotation = forcedParent.rotation * Quaternion.Euler(angleOffset);
    }
}

