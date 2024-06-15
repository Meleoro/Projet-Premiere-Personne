using IK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

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
        public bool canMoveLeg;
        
        [Header("References")] 
        [SerializeField] private List<LegIK> legsIK;
        [SerializeField] private List<Transform> legsTargets;
        private List<Transform> legsOrigins;
        [HideInInspector] public Transform mainTrRotRefFront;
        [HideInInspector] public Transform mainTrRotRefBack;
        [SerializeField] private BodyIK bodyIK;
        [SerializeField] private DecalScript decalCreatureLeft;
        [SerializeField] private DecalScript decalCreatureRight;
        private CreatureMover creatureMover;
        private CreatureManager creatureManager;


        private void Awake()
        {
            data = GetComponent<CreatureManager>().legData;
            creatureMover = GetComponent<CreatureMover>();
            creatureManager = GetComponent<CreatureManager>();

            legsOrigins = new List<Transform>();
            legsOrigins.Add(creatureManager.creatureRefScript.frontLeg1Bone1);
            legsOrigins.Add(creatureManager.creatureRefScript.frontLeg2Bone1);
            legsOrigins.Add(creatureManager.creatureRefScript.backLeg1Bone1);
            legsOrigins.Add(creatureManager.creatureRefScript.backLeg2Bone1);

            creatureManager.ActualiseTransformRefs();
            mainTrRotRefBack = creatureManager.backTransformRef;
            mainTrRotRefFront = creatureManager.frontTransformRef;

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

        private bool cooldownFront;
        private bool cooldownBack;
        private void VerifyLegs()
        {
            currentWantToMoveLegsCounter = 0;

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
                            legs[i].target.position = Vector3.Slerp(legs[i].target.position, mainTrRotRefFront.TransformPoint(legs[i].saveLastTargetPos), Time.deltaTime * 20);
                            
                            continue;
                        }
                        else
                        {
                            legs[i].saveLastTargetPos =
                                mainTrRotRefFront.InverseTransformPoint(legs[i].target.position);

                            continue;
                        }
                    }
                    else if (creatureMover.isRunning)
                    {
                        if (VerifyLegNeedsToMove(legs[i], true))
                        {
                            legs[i].target.position = mainTrRotRefFront.TransformPoint(legs[i].saveLastTargetPos);
                        }
                        else
                        {
                            legs[i].saveLastTargetPos =
                                mainTrRotRefFront.InverseTransformPoint(legs[i].target.position);

                            continue;
                        }
                    }
                }
                else
                {
                    if (currentMovingLegsBack >= maxMovingLegsAmountWalk && !creatureMover.isRunning && !legs[i].isMoving)
                    {
                        if (VerifyLegNeedsToMove(legs[i], true))
                        {
                            currentWantToMoveLegsCounter += 1;
                            legs[i].target.position = Vector3.Slerp(legs[i].target.position, mainTrRotRefBack.TransformPoint(legs[i].saveLastTargetPos), Time.deltaTime * 20);

                            continue;
                        }
                        else
                        {
                            legs[i].saveLastTargetPos =
                                mainTrRotRefBack.InverseTransformPoint(legs[i].target.position);
                            
                            continue;
                        }
                    }
                    else if (creatureMover.isRunning)
                    {
                         if (VerifyLegNeedsToMove(legs[i], true))
                         {
                             legs[i].target.position = mainTrRotRefBack.TransformPoint(legs[i].saveLastTargetPos);
                         }
                         else
                         {
                             legs[i].saveLastTargetPos =
                                mainTrRotRefBack.InverseTransformPoint(legs[i].target.position);

                             continue;
                         }
                    }
                }

                if (!legs[i].isMoving)
                {
                    if(creatureMover.isRunning && creatureMover.navMeshAgent.velocity.magnitude > 2)
                    {
                        if (!VerifyLegCanMoveRun(legs[i].isFrontLeg))
                        {
                            continue;
                        }
                    }

                    if (VerifyLegNeedsToMove(legs[i], false) 
                        || (legs[i].isFrontLeg && currentMovingLegsFront == 1 && creatureMover.isRunning && !cooldownFront) 
                        || (!legs[i].isFrontLeg && currentMovingLegsBack == 1 && creatureMover.isRunning && !cooldownBack))
                    {
                        Vector3 endPos = GetNextPos(legs[i], creatureMover.isRunning);
                        float moveDuration = legs[i].isFrontLeg ? data.frontLegMoveDuration : data.backLegMoveDuration;
                        float currentSpeedRatio = creatureMover.navMeshAgent.speed / creatureMover.agressiveSpeed;
                        float currentRotRatio = Mathf.Abs(bodyIK.currentRotationDif);

                        float YModifierBySpeed = legs[i].isFrontLeg ? data.frontLegYModifierBySpeed.Evaluate(currentSpeedRatio) : data.backLegYModifierBySpeed.Evaluate(currentSpeedRatio);
                        float YModifierByRot = legs[i].isFrontLeg ? data.frontLegYModifierByRot.Evaluate(currentRotRatio) : data.backLegYModifierByRot.Evaluate(currentRotRatio);
                        float durationModifierBySpeed = legs[i].isFrontLeg ? data.frontLegDurationModifierBySpeed.Evaluate(currentSpeedRatio) : data.backLegDurationModifierBySpeed.Evaluate(currentSpeedRatio);
                        float durationModifierByRot = legs[i].isFrontLeg ? data.frontLegDurationModifierByRot.Evaluate(currentRotRatio) : data.backLegDurationModifierByRot.Evaluate(currentRotRatio);

                        if (endPos != Vector3.zero)
                        {
                            StartCoroutine(CooldownMoveLeg());
                            StartCoroutine(MoveLeg(legs[i], endPos, moveDuration * durationModifierByRot * durationModifierBySpeed, 
                                YModifierBySpeed * YModifierByRot));
                        }

                        if (creatureMover.isRunning && creatureMover.navMeshAgent.velocity.magnitude > 2)
                        {
                            if (legs[i].isFrontLeg) cooldownFront = true;
                            else cooldownBack = true;
                            
                            StartCoroutine(CooldownLegRun(0.14f, legs[i].isFrontLeg));
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

        private IEnumerator CooldownLegRun(float duration, bool front)
        {
            yield return new WaitForSeconds(duration);
            
            if (front) cooldownFront = false;
            else cooldownBack = false;
        }

        
        private bool VerifyLegNeedsToMove(Leg currentLeg, bool shouldntMove)
        {
            float distOriginTarget = Vector3.Distance(currentLeg.isFrontLeg ?  currentLeg.origin.position + mainTrRotRefBack.forward * data.frontLegsOffset :
                currentLeg.origin.position + mainTrRotRefBack.forward * data.backLegsOffset, currentLeg.target.position);

            if (currentLeg.timerCooldownMove <= 0)
            {
                if (currentLeg.isFrontLeg && mainTrRotRefFront.InverseTransformPoint(currentLeg.target.position).z + data.frontLegsOffset > 0.15f &&
                    distOriginTarget > data.maxFrontLegDistWalk * 0.95)
                    return false;
                
                if (!currentLeg.isFrontLeg && mainTrRotRefBack.InverseTransformPoint(currentLeg.target.position).z + data.backLegsOffset > 0.15f&&
                    distOriginTarget > data.maxBackLegDistWalk * 0.95)
                    return false;
                
                if (shouldntMove && !creatureMover.isRunning)
                {
                    if (currentLeg.isFrontLeg && distOriginTarget > data.maxFrontLegDistWalk * 1.05)
                        return true;

                    if (!currentLeg.isFrontLeg && distOriginTarget > data.maxFrontLegDistWalk * 1.05)
                        return true;
                }

                else if (shouldntMove && creatureMover.isRunning)
                {
                    if (currentLeg.isFrontLeg && distOriginTarget > data.maxFrontLegDistRun * 1.05)
                        return true;

                    if (!currentLeg.isFrontLeg && distOriginTarget > data.maxBackLegDistRun * 1.05)
                        return true;
                }

                else if (creatureMover.isRunning)
                {
                    if ((cooldownFront && currentLeg.isFrontLeg) || (cooldownBack && !currentLeg.isFrontLeg))
                        return false;
                    
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

        private bool VerifyLegCanMoveRun(bool verifyFront)
        {
            List<Leg> list = new List<Leg>();
            for(int i = 0; i < legs.Count; i++)
            {
                if(verifyFront == legs[i].isFrontLeg)
                {
                    list.Add(legs[i]);
                }
            }

            if (creatureMover.navMeshAgent.velocity.magnitude < 2)
            {
                return true;
            }

            for(int i = 0; i < list.Count; i++)
            {
                Leg currentLeg = list[i];

                if (currentLeg.isMoving) return true;

                float distOriginTarget = Vector3.Distance(currentLeg.isFrontLeg ? currentLeg.origin.position + mainTrRotRefBack.forward * data.frontLegsOffset :
                    currentLeg.origin.position + mainTrRotRefBack.forward * data.backLegsOffset, currentLeg.target.position);

                if ((currentLeg.isFrontLeg && distOriginTarget < data.maxFrontLegDistRun * 0.8f) && 
                    !(currentLeg.isFrontLeg && distOriginTarget > data.maxFrontLegDistRun * 1.1f) && !currentLeg.isMoving)
                    return false;

                if ((!currentLeg.isFrontLeg && distOriginTarget < data.maxBackLegDistRun * 0.8f) && 
                    !(!currentLeg.isFrontLeg && distOriginTarget > data.maxBackLegDistRun * 1.1f) && !currentLeg.isMoving)
                    return false;

                if (mainTrRotRefBack.InverseTransformPoint(currentLeg.target.position).z > mainTrRotRefBack.InverseTransformPoint(currentLeg.origin.position).z - 0.05f)
                    return false;
            }

            return true;
        }

        #endregion


        #region Move Functions

        private Vector3 GetNextPos(Leg currentLeg, bool forceFront)
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
                //Debug.DrawRay(origin, transformRef.TransformDirection(raycastDir * (legMaxDist * 2f)), Color.blue, 1);

                if (Physics.Raycast(origin, transformRef.TransformDirection(raycastDir), out RaycastHit hit, legMaxDist * 2f, groundLayer))
                {
                    float dist = Vector3.Distance(hit.point, currentTargetPos);
                    if(forceFront)
                        dist = Vector3.Distance(hit.point, mainTrRotRefBack.position);

                    float maxDistMultiplier = Mathf.Lerp(0.95f, 1.05f,
                        creatureMover.navMeshAgent.velocity.magnitude / creatureMover.navMeshAgent.speed);
                    
                    if (dist > currentMax && Vector3.Distance(hit.point, origin) < legMaxDist * maxDistMultiplier)
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

            yield return new WaitForSeconds(0.05f);

            canMoveLeg = true;
        }

        public IEnumerator MoveLeg(Leg currentLeg, Vector3 endPos, float moveDuration, float yMultiplier)
        {
            currentLeg.isMoving = true;
            Vector3 startPos = mainTrRotRefBack.InverseTransformPoint(currentLeg.target.position);
            Vector3 localEnd = mainTrRotRefBack.InverseTransformPoint(endPos);
            AnimationCurve currentYCurve = currentLeg.isFrontLeg ? data.frontLegMovementYCurve : data.backLegMovementYCurve;
            float timer = 0;
            RaycastHit hit;
            float wantedY = mainTrRotRefBack.TransformPoint(currentLeg.scriptIK.saveTargetOriginOffset).y;

            while (timer < moveDuration)
            {
                timer += Time.deltaTime;
                
                float addedY = currentYCurve.Evaluate(timer / moveDuration);
                currentLeg.currentAddedY = addedY;

                if (Physics.Raycast(currentLeg.target.position + Vector3.up * 1f, -currentLeg.target.up, out hit, 3f,
                        LayerManager.Instance.groundLayer))
                {
                    wantedY = Mathf.Lerp(wantedY, hit.point.y + addedY * yMultiplier, Time.deltaTime * 18);
                }
                
                Vector3 wantedPos = Vector3.Lerp(startPos, localEnd, timer / moveDuration) + new Vector3(0, addedY, 0);
                
                currentLeg.target.position = mainTrRotRefBack.TransformPoint(wantedPos);
                
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

            if(!creatureMover.isRunning)
                AudioManager.Instance.PlaySoundOneShot(0, Random.Range(2, 5), 1);

            else
                AudioManager.Instance.PlaySoundOneShot(0, Random.Range(5, 8), 1);

            if (VerifyRock(currentLeg.target.position)) yield break;

            if (mainTrRotRefFront.InverseTransformPoint(currentLeg.origin.position).x < 0)
            {
                Instantiate(decalCreatureLeft, currentLeg.scriptIK.effector.position + mainTrRotRefFront.forward * 0.2f + Vector3.down * 0.6f, 
                    Quaternion.Euler(90, mainTrRotRefFront.rotation.eulerAngles.y - 90, -270));
            }
            else
            {
                Instantiate(decalCreatureRight, currentLeg.scriptIK.effector.position + mainTrRotRefFront.forward * 0.2f + Vector3.down * 0.6f, 
                    Quaternion.Euler(90, mainTrRotRefFront.rotation.eulerAngles.y - 90, -90));
            }
        }

        private List<TerrainDetector> terrainDetectors = new List<TerrainDetector>();
        private bool VerifyRock(Vector3 origin)
        {
            bool isOnRock = false;

            RaycastHit hitInfo;
            if (Physics.Raycast(origin, Vector3.down, out hitInfo, 3f, LayerManager.Instance.groundLayer))
            {
                if (hitInfo.collider.TryGetComponent<TerrainCollider>(out TerrainCollider terrain))
                {
                    int wantedIndex = -1;
                    for (int i = 0; i < terrainDetectors.Count; i++)
                    {
                        if (terrainDetectors[i].terrainData == terrain.terrainData)
                        {
                            wantedIndex = i;
                            break;
                        }
                    }


                    if (wantedIndex == -1)
                    {
                        terrainDetectors.Add(new TerrainDetector(terrain.terrainData, hitInfo.collider.GetComponent<Terrain>(), hitInfo.collider.GetComponent<TerrainRock>().rockLayerIndex));

                        if (terrainDetectors[^1].GetIsWalkingOnRock(origin))
                        {
                            isOnRock = true;
                        }
                    }

                    else
                    {
                        if (terrainDetectors[wantedIndex].GetIsWalkingOnRock(origin))
                        {
                            isOnRock = true;
                        }
                    }
                }
                else if (hitInfo.collider.CompareTag("RockGround"))
                {
                    isOnRock = true;
                }
            }

            return isOnRock;
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
