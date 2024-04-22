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

        [Header("Public Infos")]
        [HideInInspector] public Vector3 wantedPos;
        [HideInInspector] public Vector3 forcedRot;

        [Header("Private Infos")] 
        private float addedForceY;
        private float timerNoiseY;
        private float zRotationLerp;
        private float saveSpeed;
        private Vector3 originalLegsPos;
        private Vector3 currentLegsAverageLerp;
        private bool goDown;
        private bool stopMoving;

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

            saveSpeed = navMeshAgent.speed;
        }


        public void ComponentUpdate()
        {
            if (stopMoving) return;
            
            SetNextPos();
            ManageRotation();

            //AdaptHeightBody();
            AdaptSpeedWhenRotation();
        }

        
        #region BASE MOVEMENT

        private void SetNextPos()
        {
            if (doDebugMovement)
            {
                navMeshAgent.SetDestination(debugWantedPos.position);
            }
            else
            {
                navMeshAgent.SetDestination(wantedPos);
            }
        }

        private void ManageRotation()
        {
            // Y Rotation
            Vector3 dirToRotateTo = navMeshAgent.velocity;
            if (forcedRot != Vector3.zero) dirToRotateTo = forcedRot;
            
            Vector3 currentDir = targetIKBody.position - transform.position;
            currentDir = currentDir.normalized * 4;

            currentDir = Vector3.RotateTowards(currentDir, dirToRotateTo, Time.deltaTime * rotateSpeed, Time.deltaTime * rotateSpeed);
            
            targetIKBody.position = transform.position + currentDir;
            
            // Z Rotation
            /*Vector3 legsAveragePos = Vector3.zero;
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
            zRotationLerp = Mathf.Clamp(zRotationLerp, -0.5f, 0.5f);
            
            transformToRotate.localRotation = Quaternion.Euler( transformToRotate.localEulerAngles.x,  transformToRotate.localEulerAngles.y, zRotationLerp * 20 + currentLegsAverageLerp.y * 65);*/
        }

        #endregion


        #region NATURAL MOVEMENT
        
        /*private void AdaptHeightBody()
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
        }*/
        
        private void AdaptSpeedWhenRotation()
        {
            float currentRotationDif = Mathf.Abs(bodyIKScript.currentRotationDif);

            navMeshAgent.speed = Mathf.Lerp(saveSpeed, saveSpeed * 0.1f, currentRotationDif);
        }

        #endregion


        public void StopMoving()
        {
            navMeshAgent.SetDestination(transform.position);
            stopMoving = true;
        }

        public void RestartMoving()
        {
            SetNextPos();
            stopMoving = false;
        }
    }
}
