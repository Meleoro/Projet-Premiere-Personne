using System;
using IK;
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
        private float addedForceY;
        private float timerNoiseY;
        private float zRotationLerp;
        private Vector3 originalLegsPos;
        private Vector3 currentLegsAverageLerp;
        private bool goDown;

        [Header("References")] 
        [SerializeField] private BodyIK bodyIKScript;
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

            AdaptHeightBody();
            AdaptJointsRotations();
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
                currentLegsAverageLerp = Vector3.Lerp(currentLegsAverageLerp, legsAveragePos, Time.deltaTime * 10);
            }

            zRotationLerp = Mathf.Lerp(zRotationLerp, -dirToRotateTo.y, Time.deltaTime * 5);
            
            transformToRotate.localRotation = Quaternion.Euler( transformToRotate.localEulerAngles.x,  transformToRotate.localEulerAngles.y, zRotationLerp * 20 + currentLegsAverageLerp.y * 60);
        }

        #endregion


        #region NATURAL MOVEMENT

        private void AdaptHeightBody()
        {
            float legModificatorY = 0;
            for (int i = 0; i < legsScript.legs.Count; i++)
            {
                if (legsScript.legs[i].isMoving)
                    legModificatorY -= wantedGroundDist * 0.1f;
            }
            
            if (Physics.Raycast(transformToRotate.position, -transformToRotate.up, out RaycastHit hit, maxGroundDist, LayerManager.Instance.groundLayer))
            {
                float groundDist = Vector3.Distance(transformToRotate.position, hit.point);
                
                timerNoiseY += goDown ? -Time.deltaTime : Time.deltaTime;
                if (timerNoiseY <= -0.7f)
                    goDown = false;
                else if (timerNoiseY >= 0.7f)
                    goDown = true;
                
                addedForceY = Mathf.Lerp(addedForceY, (wantedGroundDist - groundDist) + legModificatorY + timerNoiseY * 0.4f, Time.deltaTime * 5);

                transformToRotate.position += transformToRotate.up * (addedForceY * Time.deltaTime);
            }

            else
            {
                addedForceY = Mathf.Lerp(addedForceY, -1, Time.deltaTime * 5);
                
                transformToRotate.position += transformToRotate.up * (addedForceY * Time.deltaTime);
            }
        }


        private float currentXRotateBodyFront;
        private float currentZRotateBodyFront;
        private float currentXRotateBodyBack;
        private float currentZRotateBodyBack;
        private void AdaptJointsRotations()
        {
            Vector3 frontAveragePos = Vector3.zero;
            Vector3 backAveragePos = Vector3.zero;
            
            for (int i = 0; i < legsScript.legs.Count; i++)
            {
                if (legsScript.legs[i].isMoving)
                {
                    if (legsScript.legs[i].isFrontLeg)
                    {
                        Vector3 dif = legsScript.legs[i].originalPos - transform.InverseTransformPoint(legsScript.legs[i].target.position);
                        
                        if(transform.InverseTransformPoint(legsScript.legs[i].target.position).x < 0)
                            frontAveragePos += new Vector3(-dif.y, dif.y, dif.z);
                        
                        else
                            frontAveragePos += new Vector3(dif.y, dif.y, dif.z);
                    }
                    else
                    {
                        Vector3 dif = legsScript.legs[i].originalPos - transform.InverseTransformPoint(legsScript.legs[i].target.position);
                        
                        if(transform.InverseTransformPoint(legsScript.legs[i].target.position).x < 0)
                            backAveragePos += new Vector3(-dif.y, dif.y, dif.z);
                        
                        else
                            backAveragePos += new Vector3(dif.y, dif.y, dif.z);
                    }
                }
            }

            currentXRotateBodyFront = Mathf.Lerp(currentXRotateBodyFront, frontAveragePos.x, Time.deltaTime * 8);
            currentZRotateBodyFront = Mathf.Lerp(currentZRotateBodyFront, frontAveragePos.y, Time.deltaTime * 8);
            
            currentXRotateBodyBack = Mathf.Lerp(currentXRotateBodyBack, backAveragePos.x, Time.deltaTime * 8);
            currentZRotateBodyBack = Mathf.Lerp(currentZRotateBodyBack, backAveragePos.y, Time.deltaTime * 8);
            
            
            bodyIKScript.bodyJoint.localRotation = Quaternion.Euler(currentXRotateBodyFront * 10, bodyIKScript.bodyJoint.localEulerAngles.y, currentZRotateBodyFront);
            bodyIKScript.backTransform.localRotation = Quaternion.Euler(currentXRotateBodyBack * 10, bodyIKScript.backTransform.localEulerAngles.y, currentZRotateBodyBack);
        }

        #endregion
    }
}
