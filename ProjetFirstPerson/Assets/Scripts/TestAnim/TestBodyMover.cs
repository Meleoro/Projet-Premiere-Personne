using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBodyMover : MonoBehaviour
{
    [Header("Levitation Parameters")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float wantedGroundDist;
    [SerializeField] private float maxGroundDist;
    [SerializeField] private float levitationStrength;

    [Header("References")]
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Rigidbody rb;


    private void Update()
    {
        ManageLevitation();
    }



    #region LEVITATE BODY FUNCTIONS

    private void ManageLevitation()
    {
        float distanceFromGround = GetGroundDist();
        float forceToAdd = wantedGroundDist - distanceFromGround;

        rb.AddForce(bodyTransform.forward * (forceToAdd * levitationStrength), ForceMode.Force);
    }

    private float GetGroundDist()
    {
        RaycastHit hit;
        float dist = maxGroundDist;

        if(Physics.Raycast(bodyTransform.position, -bodyTransform.forward, out hit, maxGroundDist, groundMask))
        {
            dist = Vector3.Distance(bodyTransform.position, hit.point);
        }

        return dist;
    }

    #endregion
}
