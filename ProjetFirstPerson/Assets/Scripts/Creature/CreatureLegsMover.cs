using IK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Creature
{
    public class CreatureLegsMover : MonoBehaviour, ICreatureComponent
    {
        [Header("Parameters")]
        public int maxMovingLegsAmountWalk;
        [SerializeField] private LayerMask groundLayer;
        
        [Header("Public Infos")] 
        public List<Leg> legs = new List<Leg>();
        [HideInInspector] public int currentWantToMoveLegsCounter = 0;
        
        [Header("Private Infos")] 
        private CreatureLegsParamData data;
        private int currentMovingLegsFront;
        private int currentMovingLegsBack;
        private bool canMoveLeg;
        
        [Header("References")] 
        [SerializeField] private List<LegIK> legsIK;
        [SerializeField] private List<Transform> legsTargets;
        [SerializeField] private List<Transform> legsOrigins;
        public Transform mainTrRotRefFront;
        public Transform mainTrRotRefBack;
        [SerializeField] private BodyIK bodyIK;
        private CreatureMover creatureMover;


        private void Awake()
        {
            data = GetComponent<CreatureManager>().legData;
            creatureMover = GetComponent<CreatureMover>();

            legs = new List<Leg>();
            
            for (int i = 0; i < legsTargets.Count; i++)
            {
                legs.Add(new Leg(legsTargets[i], legsOrigins[i], 
                    Vector3.Distance(legsOrigins[i].position, mainTrRotRefBack.position) > Vector3.Distance(legsOrigins[i].position, mainTrRotRefFront.position), 
                    legsIK[i]));
            }

            canMoveLeg = true;
        }

        public void ComponentUpdate()
        {
            ActualiseMovingLegsCounter();
            VerifyLegs();
        }


        #region Verify Functions

        private bool moveFrontLegs;
        private bool moveBackLegs;
        private void VerifyLegs()
        {
            currentWantToMoveLegsCounter = 0;

            moveFrontLegs = false;
            moveBackLegs = false;
            
            if (!canMoveLeg) return;
            
            for (int i = 0; i < legs.Count; i++)
            {
                ActualiseMovingLegsCounter();

                if (!legs[i].scriptIK.canMove) continue;
                if (legs[i].isFrontLeg)
                {
                    if (currentMovingLegsFront >= maxMovingLegsAmountWalk && !creatureMover.isRunning && !legs[i].isMoving)
                    {
                        if (VerifyLegNeedsToMove(legs[i], true))
                        {
                            currentWantToMoveLegsCounter += 1;
                            legs[i].target.position = mainTrRotRefFront.TransformPoint(legs[i].saveLastTargetPos);
                            
                            continue;
                        }
                        else if(VerifyLegNeedsToMove(legs[i], false))
                        {
                            currentWantToMoveLegsCounter += 1;
                            legs[i].saveLastTargetPos =
                                mainTrRotRefFront.InverseTransformPoint(legs[i].target.position);
                            
                            continue;
                        }
                    }
                    else if (!VerifyLegNeedsToMove(legs[i], false))
                    {
                        legs[i].saveLastTargetPos =
                            mainTrRotRefFront.InverseTransformPoint(legs[i].target.position);

                        continue;
                    }
                }
                else
                {
                    if (currentMovingLegsBack >= maxMovingLegsAmountWalk && !creatureMover.isRunning && !legs[i].isMoving)
                    {
                        if (VerifyLegNeedsToMove(legs[i], true))
                        {
                            currentWantToMoveLegsCounter += 1;
                            
                            continue;
                        }
                        else
                        {
                            currentWantToMoveLegsCounter += 1;
                            
                            continue;
                        }
                    }
                    else if (!VerifyLegNeedsToMove(legs[i], false))
                    {
                        legs[i].saveLastTargetPos =
                            mainTrRotRefFront.InverseTransformPoint(legs[i].target.position);

                        continue;
                    }
                }

                if (!legs[i].isMoving)
                {
                    if (VerifyLegNeedsToMove(legs[i], false) || (legs[i].isFrontLeg && moveFrontLegs) || (!legs[i].isFrontLeg && moveBackLegs))
                    {
                        if(!legs[i].isFrontLeg) moveBackLegs = false;
                        if(legs[i].isFrontLeg) moveFrontLegs = false;

                        Vector3 endPos = GetNextPos(legs[i]);
                        float moveDuration = legs[i].isFrontLeg ? data.frontLegMoveDuration : data.backLegMoveDuration;
                        float currentSpeedRatio = creatureMover.navMeshAgent.speed / creatureMover.agressiveSpeed;
                        float currentRotRatio = bodyIK.currentRotationDif;

                        float YModifierBySpeed = legs[i].isFrontLeg ? data.frontLegYModifierBySpeed.Evaluate(currentSpeedRatio) : data.backLegYModifierBySpeed.Evaluate(currentSpeedRatio);
                        float YModifierByRot = legs[i].isFrontLeg ? data.frontLegYModifierByRot.Evaluate(currentRotRatio) : data.backLegYModifierByRot.Evaluate(currentRotRatio);
                        float durationModifierBySpeed = legs[i].isFrontLeg ? data.frontLegDurationModifierBySpeed.Evaluate(currentSpeedRatio) : data.backLegDurationModifierBySpeed.Evaluate(currentSpeedRatio);
                        float durationModifierByRot = legs[i].isFrontLeg ? data.frontLegDurationModifierByRot.Evaluate(currentRotRatio) : data.backLegDurationModifierByRot.Evaluate(currentRotRatio);


                        if (endPos != Vector3.zero)
                        {
                            StartCoroutine(CooldownMoveLeg());
                            StartCoroutine(MoveLeg(legs[i], endPos, moveDuration * durationModifierByRot * durationModifierBySpeed, YModifierBySpeed * YModifierByRot));

                            if (creatureMover.isRunning && !legs[i].isFrontLeg)
                            {
                                moveBackLegs = true;
                            }

                            if (creatureMover.isRunning && legs[i].isFrontLeg)
                            {
                                moveFrontLegs = true;
                            }
                        }
                    }
                }
            }
        }

        
        private void ActualiseMovingLegsCounter()
        {
            currentMovingLegsBack = 0;
            currentMovingLegsFront = 0;

            for (int i = 0; i < legs.Count; i++)
            {
                if (legs[i].isMoving && legs[i].isFrontLeg)
                {
                    currentMovingLegsFront += 1;
                }

                else if (legs[i].isMoving && !legs[i].isFrontLeg)
                {
                    currentMovingLegsBack += 1;
                }
            }
        }

        
        private bool VerifyLegNeedsToMove(Leg currentLeg, bool shouldntMove)
        {
            float distOriginTarget = Vector3.Distance(currentLeg.isFrontLeg ?  currentLeg.origin.position + mainTrRotRefBack.forward * data.frontLegsOffset :
                currentLeg.origin.position + mainTrRotRefBack.forward * data.backLegsOffset, currentLeg.target.position);

            if (currentLeg.timerCooldownMove <= 0)
            {
                if (currentLeg.isFrontLeg && mainTrRotRefFront.InverseTransformPoint(currentLeg.target.position).z > -0.1f)
                    return false;
                
                if (!currentLeg.isFrontLeg && mainTrRotRefBack.InverseTransformPoint(currentLeg.target.position).z > -0.1f)
                    return false;
                
                if (shouldntMove)
                {
                    if (currentLeg.isFrontLeg && distOriginTarget > data.maxFrontLegDistWalk * 1.1f)
                        return true;

                    if (!currentLeg.isFrontLeg && distOriginTarget > data.maxFrontLegDistWalk * 1.1f)
                        return true;
                }
                
                else if (creatureMover.isRunning)
                {
                    if (currentLeg.isFrontLeg && distOriginTarget > data.maxFrontLegDistRun)
                        return true;

                    if (!currentLeg.isFrontLeg && distOriginTarget > data.maxBackLegDistRun)
                        return true;
                }

                else
                {
                    if (currentLeg.isFrontLeg && distOriginTarget > data.maxFrontLegDistWalk * 0.9f)
                        return true;

                    if (!currentLeg.isFrontLeg && distOriginTarget > data.maxBackLegDistWalk * 0.9f)
                        return true;
                }
            }
            else
            {
                currentLeg.timerCooldownMove -= Time.deltaTime;
            }
            
            return false;
        }

        #endregion


        #region Move Functions

        private Vector3 GetNextPos(Leg currentLeg)
        {
            Vector3 origin = currentLeg.isFrontLeg ? currentLeg.origin.position + mainTrRotRefBack.forward * data.frontLegsOffset 
                : currentLeg.origin.position + mainTrRotRefBack.forward * data.backLegsOffset;
            Vector3 currentTargetPos = currentLeg.target.position;
            Transform transformRef = currentLeg.isFrontLeg ? mainTrRotRefFront : mainTrRotRefBack;

            //Vector3 targetTranslatedPos = transformRef.InverseTransformVector(currentLeg.origin.position - currentLeg.target.position);
            Vector3 saveOriginalRot = transformRef.localEulerAngles;
            //transformRef.localEulerAngles = new Vector3(0, Mathf.Atan2(targetTranslatedPos.z, targetTranslatedPos.x) * Mathf.Rad2Deg, 0);

            Vector3 raycastDir = transformRef.InverseTransformDirection(Vector3.down).RotateDirection(45, Vector3.right);

            float currentMax = 0;
            Vector3 chosenPos = currentLeg.origin.position - Vector3.down * 1.5f;

            float legMaxDist = currentLeg.isFrontLeg ? data.maxFrontLegDistWalk : data.maxBackLegDistWalk;
            if (creatureMover.isRunning) legMaxDist = currentLeg.isFrontLeg ? data.maxFrontLegDistRun : data.maxBackLegDistRun;

            for (int i = 0; i < 45; i++)
            {
                Debug.DrawRay(origin, transformRef.TransformDirection(raycastDir * (legMaxDist * 2f)), Color.blue, 1);

                if (Physics.Raycast(origin, transformRef.TransformDirection(raycastDir), out RaycastHit hit, legMaxDist * 2f, groundLayer))
                {
                    float dist = Vector3.Distance(hit.point, currentTargetPos);

                    if (dist > currentMax && Vector3.Distance(hit.point, origin) < legMaxDist * 1.05f)
                    {
                        currentMax = dist;
                        chosenPos = hit.point;
                    }
                }

                raycastDir = raycastDir.RotateDirection(-2, Vector3.right);
            }

            transformRef.localEulerAngles = saveOriginalRot;

            return chosenPos;
        }

        private IEnumerator CooldownMoveLeg()
        {
            canMoveLeg = false;

            yield return new WaitForSeconds(0.1f);

            canMoveLeg = true;
        }

        public IEnumerator MoveLeg(Leg currentLeg, Vector3 endPos, float moveDuration, float yMultiplier)
        {
            currentLeg.isMoving = true;
            Vector3 startPos = transform.InverseTransformPoint(currentLeg.target.position);
            Vector3 localEnd = transform.InverseTransformPoint(endPos);
            AnimationCurve currentYCurve = currentLeg.isFrontLeg ? data.frontLegMovementYCurve : data.backLegMovementYCurve;
            float timer = 0;
            RaycastHit hit;

            while (timer < moveDuration)
            {
                timer += Time.deltaTime;
                
                float wantedY = 0;
                float addedY = currentYCurve.Evaluate(timer / moveDuration);
                currentLeg.currentAddedY = addedY;

                if (Physics.Raycast(currentLeg.target.position + Vector3.up * 1f, -currentLeg.target.up, out hit, 3f,
                        LayerManager.Instance.groundLayer))
                {
                    wantedY = hit.point.y + addedY * yMultiplier;
                }
                
                Vector3 wantedPos = Vector3.Lerp(startPos, localEnd, timer / moveDuration) + new Vector3(0, addedY, 0);
                
                currentLeg.target.position = transform.TransformPoint(wantedPos);
                
                if(wantedY != 0)
                    currentLeg.target.position = new Vector3(currentLeg.target.position.x, wantedY, currentLeg.target.position.z);

                currentLeg.scriptIK.currentPatouneZRot = data.frontPatouneRot.Evaluate(timer / moveDuration) * data.frontPatouneRotMultiplier;

                yield return null;
            }
            
            if (Physics.Raycast(currentLeg.target.position + Vector3.up * 1f, -currentLeg.target.up, out hit, 3f,
                    LayerManager.Instance.groundLayer))
            {
                currentLeg.target.position = hit.point;
            }
            //currentLeg.target.position = transform.TransformPoint(localEnd);

            currentLeg.timerCooldownMove = 0.3f;
            currentLeg.isMoving = false;
        }

        #endregion
    }
}

[Serializable]
public class Leg
{
    public LegIK scriptIK;
    
    public bool isMoving;
    public bool isFrontLeg;
    public Transform target;
    public Transform origin;
    public float timerCooldownMove;
    public float currentAddedY;

    public Vector3 saveLastTargetPos;

    public Leg(Transform target, Transform origin, bool isFrontLeg, LegIK ik)
    {
        isMoving = false;
        this.isFrontLeg = isFrontLeg;
        this.target = target;
        this.origin = origin;
        scriptIK = ik;
    }
}
