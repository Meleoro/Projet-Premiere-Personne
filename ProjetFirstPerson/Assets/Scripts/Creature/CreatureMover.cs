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
        [Header("Speed Parameters")]
        [SerializeField] public float walkSpeed;
        [SerializeField] private float suspicionSpeed;
        public float agressiveSpeed;
        
        [Header("Debug Parameters")] 
        [SerializeField] private bool doDebugMovement;
        [SerializeField] private Transform debugWantedPos;
 
        [Header("Public Infos")]
        [HideInInspector] public Vector3 wantedPos;
        [HideInInspector] public Vector3 forcedRot;
        [HideInInspector] public Vector3 forcedPos;
        public bool isRunning;
        [HideInInspector] public Vector3 currentDir;

        [Header("Private Infos")]
        private CreatureBodyParamData data;
        private float saveSpeed;
        private bool stopMoving;

        [Header("References")] 
        public BodyIK bodyIKScript;
        public HeadIK headIKScript;
        [SerializeField] private CreatureSpecialMoves specialMovesScript;
        public Transform targetIKBody;
        [HideInInspector] public NavMeshAgent navMeshAgent;
        public TailIK tailIKScript;
        private CreatureLegsMover legsScript;


        private void Awake()
        {
            data = GetComponent<CreatureManager>().bodyData;
            
            navMeshAgent = GetComponent<NavMeshAgent>();
            legsScript = GetComponent<CreatureLegsMover>();

            saveSpeed = navMeshAgent.speed;

            heightModifierFrontTimer = 0.5f;
            
            StartWalkSpeed();
        }


        public void ComponentUpdate()
        {
            AdaptHeightBySpeed();
            
            if (stopMoving) return;
            
            SetNextPos();
            ManageRotation();
            
            AdaptSpeedWhenRotation();
            AdaptSpeedAccordingToLegs();
        }

        
        #region BASE MOVEMENT

        private void SetNextPos()
        {
            if (doDebugMovement)
            {
                navMeshAgent.SetDestination(debugWantedPos.position);
            }
            else if (forcedPos != Vector3.zero)
            {
                navMeshAgent.SetDestination(forcedPos);
            }
            else
            {
                navMeshAgent.SetDestination(wantedPos);
            }
        }

        private void ManageRotation()
        {
            // Y Rotation
            Vector3 dirToRotateTo = navMeshAgent.velocity.normalized;
            if (forcedRot != Vector3.zero) dirToRotateTo = forcedRot;
            else if (forcedPos != Vector3.zero) dirToRotateTo = forcedPos - legsScript.mainTrRotRefBack.position;

            currentDir = targetIKBody.position - legsScript.mainTrRotRefBack.position;
            currentDir = currentDir.normalized * 7;

            currentDir = Vector3.RotateTowards(currentDir, dirToRotateTo, 1, 1);
            
            targetIKBody.position = transform.position + currentDir;
            
            if (Vector3.Angle(currentDir, dirToRotateTo) > 140)
                StartCoroutine(specialMovesScript.DoHugeTurnCoroutine(Vector3.Distance(wantedPos, transform.position) < 3));
        }

        #endregion


        #region NATURAL MOVEMENT
        
        private void AdaptSpeedWhenRotation()
        {
            float currentRotationDif = Mathf.Abs(bodyIKScript.currentRotationDif);

            navMeshAgent.speed = Mathf.Lerp(saveSpeed, saveSpeed * 0.25f, currentRotationDif);
        }

        private void AdaptHeightBySpeed()
        {
            ActualiseIdleModificator();
            float currentSpeed = navMeshAgent.velocity.magnitude / agressiveSpeed;
            float wantedYFront = data.frontWantedHeight * data.heightModifierCurveBySpeed.Evaluate(currentSpeed);
            float wantedYBack = data.backWantedHeight * data.heightModifierCurveBySpeed.Evaluate(currentSpeed);

            // Back
            RaycastHit groundHitBack;
            if(Physics.Raycast(bodyIKScript.backJoint.position + Vector3.up, Vector3.down, out groundHitBack, data.maxHeight + 1, LayerManager.Instance.groundLayer))
            {
                bodyIKScript.backJoint.position =
                    Vector3.Lerp(bodyIKScript.backJoint.position, 
                        groundHitBack.point + Vector3.up * (wantedYBack + bodyIKScript.currentAddedBackY + heightModifierBack), 
                        Time.deltaTime * 5);
            }
            else
            {
                bodyIKScript.backJoint.position -= Vector3.up * (Time.deltaTime * 3);
            }

            // Front
            RaycastHit groundHitFront;
            if (Physics.Raycast(bodyIKScript.bodyJoint.position + Vector3.up, Vector3.down, out groundHitFront, data.maxHeight + 1, LayerManager.Instance.groundLayer))
            {
                Vector3 wantedPosition = groundHitFront.point + Vector3.up * (wantedYFront + bodyIKScript.currentAddedFrontY);

                bodyIKScript.frontYDif = wantedPosition.y - bodyIKScript.bodyJoint.position.y + heightModifierFront;
            }
            else
            {
                bodyIKScript.frontYDif -= Time.deltaTime * 3;
            }
        }


        private float heightModifierFrontTimer;
        private float heightModifierBackTimer;
        private float heightModifierFront = -0.4f;
        private float heightModifierBack;
        private bool goUpFront;
        private bool goUpBack;
        private void ActualiseIdleModificator()
        {
            heightModifierFrontTimer += Time.deltaTime;
            if (heightModifierFrontTimer > 1)
            {
                heightModifierFrontTimer = -0.1f;
                goUpFront = !goUpFront;
            }
            
            heightModifierBackTimer += Time.deltaTime;
            if (heightModifierBackTimer > 1)
            {
                heightModifierBackTimer = -0.1f;
                goUpBack = !goUpBack;
            }
            
            if (goUpFront) heightModifierFront = Mathf.Lerp(0, 0.1f, heightModifierFrontTimer);
            else heightModifierFront = Mathf.Lerp(0.1f, 0, heightModifierFrontTimer);
            
            if (goUpBack) heightModifierBack = Mathf.Lerp(0, 0.1f, heightModifierBackTimer);
            else heightModifierBack = Mathf.Lerp(0.1f, 0, heightModifierBackTimer);
            
        }

        private void AdaptSpeedAccordingToLegs()
        {
            /*if (legsScript.currentWantToMoveLegsCounter >= 1)
            {
                navMeshAgent.speed = saveSpeed * data.legCantMoveSpeedMultiplier;
            }
            else
            {
                navMeshAgent.speed = saveSpeed;
            }*/
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
            
            AudioManager.Instance.PlaySoundOneShot(0, 1, 1);

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
