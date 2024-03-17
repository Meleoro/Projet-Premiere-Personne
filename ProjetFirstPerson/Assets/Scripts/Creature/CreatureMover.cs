using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Creature
{
    [RequireComponent(typeof(CreatureLegsMover))]
    public class CreatureMover : MonoBehaviour, ICreatureComponent
    {
        [Header("Parameters")] 
        [SerializeField] private float maxGroundDist;
        [SerializeField] private float wantedGroundDist;
        [SerializeField] private float rotateSpeed;
        
        [Header("Debug Parameters")] 
        [SerializeField] private bool doDebugMovement;
        [SerializeField] private Transform debugWantedPos;

        [Header("Private Infos")] 
        private float timerMovement;
        private Vector3 originalLegsPos;
        private Vector3 currentLegsAverageLerp;
        private float addedForceY;
        private float timerNoiseY;
        private bool goDown;

        [Header("References")] 
        [SerializeField] private Transform targetIKBody;
        [SerializeField] private Transform transformToRotate;
        private NavMeshAgent navMeshAgent;
        private CreatureLegsMover legsScript;


        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            legsScript = GetComponent<CreatureLegsMover>();
        }


        public void ComponentUpdate()
        {
            SetNextPos();
            
            ManageRotation();

            AdaptHeightBodyParts();
        }

        
        #region BASE MOVEMENT

        private void SetNextPos()
        {
            if (doDebugMovement)
            {
                navMeshAgent.SetDestination(debugWantedPos.position);
            }
        }

        private void ManageRotation()
        {
            // Y Rotation
            Vector3 dirToRotateTo = navMeshAgent.velocity;
            Vector3 currentDir = targetIKBody.position - transform.position;
            currentDir = currentDir.normalized * 4;

            currentDir = Vector3.RotateTowards(currentDir, dirToRotateTo, Time.deltaTime * rotateSpeed, Time.deltaTime * rotateSpeed);
            
            targetIKBody.position = transform.position + currentDir;
            
            // Z Rotation
            Vector3 legsAveragePos = Vector3.zero;
            for (int i = 0; i < legsScript.legs.Count; i++)
            {
                legsAveragePos += legsScript.legs[i].target.position;
            }
            legsAveragePos /= legsScript.legs.Count;
            legsAveragePos = transform.InverseTransformPoint(legsAveragePos);

            if (originalLegsPos == Vector3.zero)
            {
                currentLegsAverageLerp = legsAveragePos;
                originalLegsPos = legsAveragePos;
            }

            else
            {
                legsAveragePos = originalLegsPos - legsAveragePos;
                currentLegsAverageLerp = Vector3.Lerp(currentLegsAverageLerp, legsAveragePos, Time.deltaTime * 15);
                
                transformToRotate.localRotation = Quaternion.Euler( transformToRotate.localEulerAngles.x,  transformToRotate.localEulerAngles.y, legsAveragePos.y * 80);
            }
        }

        #endregion


        #region NATURAL MOVEMENT

        private void AdaptHeightBodyParts()
        {
            Debug.DrawRay(transformToRotate.position, -transformToRotate.up * maxGroundDist);
            
            if (Physics.Raycast(transformToRotate.position, -transformToRotate.up, out RaycastHit hit, maxGroundDist, LayerManager.Instance.groundLayer))
            {
                float groundDist = Vector3.Distance(transformToRotate.position, hit.point);
                
                timerNoiseY += goDown ? -Time.deltaTime : Time.deltaTime;
                if (timerNoiseY <= -0.7f)
                    goDown = false;
                else if (timerNoiseY >= 0.7f)
                    goDown = true;
                
                addedForceY = Mathf.Lerp(addedForceY, (wantedGroundDist - groundDist) + timerNoiseY * 0.4f, Time.deltaTime * 5);

                transformToRotate.position += transformToRotate.up * (addedForceY * Time.deltaTime);
            }

            else
            {
                addedForceY = Mathf.Lerp(addedForceY, -1, Time.deltaTime * 5);
                
                transformToRotate.position += transformToRotate.up * (addedForceY * Time.deltaTime);
            }
        }

        #endregion
    }
}
