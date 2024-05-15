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
        public CreatureBodyParamData data;

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
        [SerializeField] private HeadIK headIKScript;
        [SerializeField] private Transform targetIKBody;
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
            
            AdaptSpeedWhenRotation();
            AdaptHeightBySpeed();
            AdaptSpeedAccordingToLegs();
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

            currentDir = Vector3.RotateTowards(currentDir, dirToRotateTo, 1, 1);
            
            targetIKBody.position = transform.position + currentDir;
        }

        #endregion


        #region NATURAL MOVEMENT
        
        private void AdaptSpeedWhenRotation()
        {
            float currentRotationDif = Mathf.Abs(bodyIKScript.currentRotationDif);

            navMeshAgent.speed = Mathf.Lerp(saveSpeed, saveSpeed * 0.3f, currentRotationDif);
        }

        private void AdaptHeightBySpeed()
        {
            float currentSpeed = navMeshAgent.velocity.magnitude / agressiveSpeed;
            float wantedYFront = data.frontWantedHeight * data.heightModifierCurveBySpeed.Evaluate(currentSpeed);
            float wantedYBack = data.backWantedHeight * data.heightModifierCurveBySpeed.Evaluate(currentSpeed);

            // Back
            RaycastHit groundHitBack;
            if(Physics.Raycast(bodyIKScript.backJoint.position + Vector3.up, Vector3.down, out groundHitBack, data.maxHeight + 1, LayerManager.Instance.groundLayer))
            {
                bodyIKScript.backJoint.position =
                    Vector3.Lerp(bodyIKScript.backJoint.position, groundHitBack.point + Vector3.up * wantedYBack, Time.deltaTime * 5);
            }
            else
            {
                bodyIKScript.backJoint.position -= Vector3.up * (Time.deltaTime * 3);
            }

            // Front
            RaycastHit groundHitFront;
            if (Physics.Raycast(bodyIKScript.bodyJoint.position + Vector3.up, Vector3.down, out groundHitFront, data.maxHeight + 1, LayerManager.Instance.groundLayer))
            {
                Vector3 wantedPosition = groundHitFront.point + Vector3.up * wantedYFront;

                bodyIKScript.frontYDif = wantedPosition.y - bodyIKScript.bodyJoint.position.y;
            }
            else
            {
                bodyIKScript.frontYDif -= Time.deltaTime * 3;
            }
        }

        private void AdaptSpeedAccordingToLegs()
        {
            if (legsScript.currentWantToMoveLegsCounter >= 1)
            {
                navMeshAgent.speed = saveSpeed * data.legCantMoveSpeedMultiplier;
            }
            else
            {
                navMeshAgent.speed = saveSpeed;
            }
        }

        #endregion


        #region Behavior Functions

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
            headIKScript.FollowChara();

            yield return new WaitForSeconds(waitDuration);

            headIKScript.StopFollowChara();
            StartAggressiveSpeed();
        }

        public void StartAggressiveSpeed()
        {
            saveSpeed = agressiveSpeed;
            isRunning = true;

            navMeshAgent.speed = saveSpeed;
        }

        public void StartSuspicion()
        {
            saveSpeed = suspicionSpeed;
            isRunning = false;

            navMeshAgent.speed = saveSpeed;
        }

        public void StartWalkSpeed()
        {
            saveSpeed = walkSpeed;
            isRunning = false;

            navMeshAgent.speed = saveSpeed;
        }

        public void StartAttackSpeed(float attackSpeed)
        {
            saveSpeed = attackSpeed;
            isRunning = true;

            navMeshAgent.speed = saveSpeed;
        }

        #endregion
    }
}
