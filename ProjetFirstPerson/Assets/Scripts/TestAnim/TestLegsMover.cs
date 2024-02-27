using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLegsMover : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float maxDistTarget;
    [SerializeField] private float minDistTarget;
    [SerializeField] private float moveDuration;

    [Header("References")]
    [SerializeField] private List<TestLeg> legsScripts = new List<TestLeg>();
    [SerializeField] private List<Transform> legsTips = new List<Transform>();
    [SerializeField] private List<Transform> legsTargets = new List<Transform>();
    [SerializeField] private Transform bodyToMove;


    private void Start()
    {
        SetupLegs();
    }

    private void Update()
    {
        //VerifyLegsMovement();

        //MaintainTipsOnTargets();
    }


    private void SetupLegs()
    {
        for(int i = 0; i < legsScripts.Count; i++)
        {
            legsScripts[i].SetupLeg(bodyToMove);
        }
    }

    private void MaintainTipsOnTargets()
    {
        for(int i = 0; i < legsTips.Count; i++)
        {
            legsTips[i].position = legsTargets[i].position;
        } 
    }


    private void VerifyLegsMovement()
    {
        for (int i = 0; i < legsScripts.Count; i++)
        {
            if (!legsScripts[i].isMoving && !VerifyIfTooManyLegsAreMoving())
            {
                Vector3 wantedPos = bodyToMove.position + legsScripts[i].originalOffset;

                if (Vector3.Distance(wantedPos, legsTargets[i].position) > maxDistTarget)
                {
                    Vector3 nextPos = bodyToMove.position + legsScripts[i].originalOffset;

                    StartCoroutine(legsScripts[i].MoveLegCoroutine(nextPos, moveDuration));
                }
            }
        }
    }

    private bool VerifyIfTooManyLegsAreMoving()
    {
        bool tooManyMoving = false;
        int counter = 0;

        for (int i = 0; i < legsScripts.Count; i++)
        {
            if (legsScripts[i].isMoving)
            {
                counter += 1;

                if(counter > 2)
                    tooManyMoving = true;
            }
        }

        return tooManyMoving;
    }

    /*private Vector3 GetLegNextPos(Vector3 currentLegPos, bool goForward)
    {

    }*/
}
