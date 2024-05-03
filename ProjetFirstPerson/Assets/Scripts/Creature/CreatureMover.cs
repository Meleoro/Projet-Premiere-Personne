using System;
using System.Collections;
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

        [Header("Speed Parameters")]
        [SerializeField] private float walkSpeed;
        [SerializeField] private float suspicionSpeed;
        public float agressiveSpeed;
        
        [Header("Debug Parameters")] 
        [SerializeField] private bool doDebugMovement;
        [SerializeField] private Transform debugWantedPos;

        [Header("Public Infos")]
        [HideInInspector] public Vector3 wantedPos;
        [HideInInspector] public Vector3 forcedRot;
        [HideInInspector] public bool isRunning;

        [Header("Private Infos")] 
        private float saveSpeed;
        private bool stopMoving;

        [Header("References")] 
        [SerializeField] private BodyIK bodyIKScript;
        [SerializeField] private Transform targetIKBody;
        [SerializeField] private Transform transformToRotate;
        [HideInInspector] public NavMeshAgent navMeshAgent;
        private CreatureLegsMover legsScript;


        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            legsScript = GetComponent<CreatureLegsMover>();

            saveSpeed = navMeshAgent.speed;

            StartWalkSpeed();
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
        }

        #endregion


        #region NATURAL MOVEMENT
        
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

        public IEnumerator StartAggressiveBehavior(float waitDuration)
        {
            saveSpeed = 0.1f;

            yield return new WaitForSeconds(waitDuration);

            StartAggressiveSpeed();
        }

        public void StartAggressiveSpeed()
        {
            saveSpeed = agressiveSpeed;
            isRunning = true;
        }

        public void StartSuspicion()
        {
            saveSpeed = suspicionSpeed;
            isRunning = false;
        }

        public void StartWalkSpeed()
        {
            saveSpeed = walkSpeed;
            isRunning = false;
        }

        public void StartAttackSpeed(float attackSpeed)
        {
            saveSpeed = attackSpeed;
            isRunning = true;
        }
    }
}
