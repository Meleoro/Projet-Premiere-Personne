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
        [Header("Parameters Move Trigger")] 
        [SerializeField] private int maxMovingLegsAmount;
        [SerializeField] private float frontLegMaxDist;
        [SerializeField] private float backLegMaxDist;
        [SerializeField] private LayerMask groundLayer;
        
        [Header("Parameters Leg Movement")]
        [SerializeField] private float legMoveDuration;
        [SerializeField] private AnimationCurve legMoveDurationRotModifier;
        [SerializeField] private AnimationCurve mouvementYRotModifier;
        [SerializeField] private AnimationCurve movementY;
        
        [Header("Public Infos")] 
        public List<Leg> legs = new List<Leg>();
        
        [Header("Private Infos")] 
        private int currentMovingLegsAmount;
        private bool canMoveLeg;
        
        [Header("References")] 
        [SerializeField] private List<Transform> legsTargets;
        [SerializeField] private List<Transform> legsOrigins;
        [SerializeField] private Transform mainTrRotRefFront;
        [SerializeField] private Transform mainTrRotRefBack;
        [SerializeField] private BodyIK bodyIK;


        private void Awake()
        {
            legs = new List<Leg>();
            
            for (int i = 0; i < legsTargets.Count; i++)
            {
                legs.Add(new Leg(legsTargets[i], legsOrigins[i], 
                    Vector3.Distance(legsOrigins[i].position, mainTrRotRefBack.position) > Vector3.Distance(legsOrigins[i].position, mainTrRotRefFront.position), 
                    transform.InverseTransformPoint(legsTargets[i].position)));
            }

            canMoveLeg = true;
        }

        public void ComponentUpdate()
        {
            ActualiseMovingLegsCounter();
            VerifyLegs();
        }

         
        private void VerifyLegs()
        {
            if (currentMovingLegsAmount >= maxMovingLegsAmount) return;

            if (!canMoveLeg) return;
            
            for (int i = 0; i < legs.Count; i++)
            {
                if (!legs[i].isMoving)
                {
                    if (VerifyLegNeedsToMove(legs[i]))
                    {
                        Vector3 endPos = GetNextPos(legs[i]);

                        if (endPos != Vector3.zero)
                        {
                            StartCoroutine(CooldownMoveLeg());
                            StartCoroutine(MoveLeg(legs[i], endPos, legMoveDuration * legMoveDurationRotModifier.Evaluate(Mathf.Abs(bodyIK.currentRotationDif)), mouvementYRotModifier.Evaluate(Mathf.Abs(bodyIK.currentRotationDif))));
                        }
                    }
                }
            }
        }

        
        private void ActualiseMovingLegsCounter()
        {
            currentMovingLegsAmount = 0;
            
            for (int i = 0; i < legs.Count; i++)
            {
                if (legs[i].isMoving)
                    currentMovingLegsAmount += 1;
            }
        }

        
        private bool VerifyLegNeedsToMove(Leg currentLeg)
        {
            float distOriginTarget = Vector3.Distance(currentLeg.origin.position, currentLeg.target.position);

            if (currentLeg.timerCooldownMove <= 0)
            {
                if(currentLeg.isFrontLeg && distOriginTarget > frontLegMaxDist)
                    return true;
                
                if (!currentLeg.isFrontLeg && distOriginTarget > backLegMaxDist)
                    return true;
            }
            else
            {
                currentLeg.timerCooldownMove -= Time.deltaTime;
            }
            
            return false;
        }


        private Vector3 GetNextPos(Leg currentLeg)
        {
            Vector3 origin = currentLeg.origin.position;
            Vector3 currentTargetPos = currentLeg.target.position;
            Transform transformRef = currentLeg.isFrontLeg ? mainTrRotRefFront : mainTrRotRefBack;
            
            Vector3 targetTranslatedPos = transformRef.InverseTransformVector(currentLeg.origin.position - currentLeg.target.position);
            Vector3 saveOriginalRot = transformRef.localEulerAngles;
            transformRef.localEulerAngles = new Vector3(0, Mathf.Atan2(targetTranslatedPos.z, targetTranslatedPos.x) * Mathf.Rad2Deg, 0);
            
            
            Vector3 raycastDir = transformRef.InverseTransformDirection(Vector3.down).RotateDirection(45, Vector3.right);

            float currentMax = 0;
            Vector3 chosenPos = currentLeg.origin.position - Vector3.down * 0.9f;
            float legMaxDist = currentLeg.isFrontLeg ? frontLegMaxDist : backLegMaxDist;

            for (int i = 0; i < 45; i++)
            {
                Debug.DrawRay(origin, transformRef.TransformDirection(raycastDir * (legMaxDist * 1.5f)), Color.blue, 1);
                
                if (Physics.Raycast(origin, transformRef.TransformDirection(raycastDir), out RaycastHit hit, legMaxDist * 1.2f, groundLayer))
                {
                    float dist = Vector3.Distance(hit.point, currentTargetPos);

                    if (dist > currentMax && Vector3.Distance(hit.point, origin) < legMaxDist * 0.9f)
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
            Vector3 startPos = transform.InverseTransformPoint(currentLeg.target.position);
            Vector3 localEnd = transform.InverseTransformPoint(endPos);
            float timer = 0;
            RaycastHit hit;

            while (timer < moveDuration)
            {
                timer += Time.deltaTime;
                
                float wantedY = 0;
                float addedY = movementY.Evaluate(timer / moveDuration);
                if (Physics.Raycast(currentLeg.target.position + Vector3.up * 1f, -currentLeg.target.up, out hit, 3f,
                        LayerManager.Instance.groundLayer))
                {
                    wantedY = hit.point.y + addedY * yMultiplier;
                }
                
                Vector3 wantedPos = Vector3.Lerp(startPos, localEnd, timer / moveDuration) + new Vector3(0, addedY, 0);
                
                currentLeg.target.position = transform.TransformPoint(wantedPos);
                
                if(wantedY != 0)
                    currentLeg.target.position = new Vector3(currentLeg.target.position.x, wantedY, currentLeg.target.position.z);

                yield return null;
            }
            
            if (Physics.Raycast(currentLeg.target.position + Vector3.up * 1f, -currentLeg.target.up, out hit, 2f,
                    LayerManager.Instance.groundLayer))
            {
                currentLeg.target.position = hit.point;
            }
            //currentLeg.target.position = transform.TransformPoint(localEnd);

            currentLeg.timerCooldownMove = 0.15f;
            currentLeg.isMoving = false;
        }

        public IEnumerator MoveLegStatic(Leg currentLeg, float moveDuration, float yMultiplier)
        {
            currentLeg.isMoving = true;
            Vector3 posToKeep = mainTrRotRefFront.InverseTransformPoint(currentLeg.target.position);
            float timer = 0;
            RaycastHit hit;

            while (timer < moveDuration)
            {
                timer += Time.deltaTime;

                float wantedY = 0;
                float addedY = movementY.Evaluate(timer / moveDuration);
                if (Physics.Raycast(currentLeg.target.position + Vector3.up * 1f, -currentLeg.target.up, out hit, 3f,
                        LayerManager.Instance.groundLayer))
                {
                    wantedY = hit.point.y + addedY * yMultiplier;
                }

                
                currentLeg.target.position = mainTrRotRefFront.TransformPoint(posToKeep);

                if (wantedY != 0)
                    currentLeg.target.position = new Vector3(currentLeg.target.position.x, wantedY, currentLeg.target.position.z);

                yield return null;
            }

            if (Physics.Raycast(currentLeg.target.position + Vector3.up * 1f, -currentLeg.target.up, out hit, 3f,
                    LayerManager.Instance.groundLayer))
            {
                currentLeg.target.position = hit.point;
            }

            currentLeg.timerCooldownMove = 0.15f;
            currentLeg.isMoving = false;
        }
    }
}

[Serializable]
public class Leg
{
    public bool isMoving;
    public bool isFrontLeg;
    public Transform target;
    public Transform origin;
    public float timerCooldownMove;

    public Vector3 originalPos;

    public Leg(Transform target, Transform origin, bool isFrontLeg, Vector3 originalPos)
    {
        isMoving = false;
        this.isFrontLeg = isFrontLeg;
        this.target = target;
        this.origin = origin;
        this.originalPos = originalPos;
    }
}
