using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creature
{
    public class CreatureLegsMover : MonoBehaviour, ICreatureComponent
    {
        [Header("Parameters Move Trigger")] 
        [SerializeField] private int maxMovingLegsAmount;
        [SerializeField] private float legMaxDist;
        [SerializeField] private LayerMask groundLayer;
        
        [Header("Parameters Leg Movement")]
        [SerializeField] private float legMoveDuration;
        [SerializeField] private AnimationCurve movementY;
        
        [Header("Public Infos")] 
        public List<Leg> legs = new List<Leg>();
        
        [Header("Private Infos")] 
        private int currentMovingLegsAmount;
        
        [Header("References")] 
        [SerializeField] private List<Transform> legsTargets;
        [SerializeField] private List<Transform> legsOrigins;


        private void Start()
        {
            legs = new List<Leg>();
            
            for (int i = 0; i < legsTargets.Count; i++)
            {
                legs.Add(new Leg(legsTargets[i], legsOrigins[i], Vector3.Distance(legsOrigins[i].position, transform.position) > 1, 
                    transform.InverseTransformPoint(legsOrigins[i].position)));
            }
        }

        public void ComponentUpdate()
        {
            ActualiseMovingLegsCounter();
            VerifyLegs();
        }

         
        private void VerifyLegs()
        {
            if (currentMovingLegsAmount >= maxMovingLegsAmount) return;
            
            for (int i = 0; i < legs.Count; i++)
            {
                if (!legs[i].isMoving)
                {
                    if (VerifyLegNeedsToMove(legs[i]))
                    {
                        Vector3 endPos = GetNextPos(legs[i]);
                        
                        if(endPos != Vector3.zero)
                            StartCoroutine(MoveLeg(legs[i], endPos));
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
                if (distOriginTarget > legMaxDist && currentLeg.timerCooldownMove <= 0) 
                {
                    return true;
                }
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
            Vector3 raycastDir = currentLeg.origin.InverseTransformDirection(Vector3.down).RotateDirection(45, Vector3.forward);

            float currentMax = 0;
            Vector3 chosenPos = Vector3.zero;

            for (int i = 0; i < 45; i++)
            {
                Debug.DrawRay(origin - currentLeg.origin.right * 0.5f, currentLeg.origin.TransformDirection(raycastDir * (legMaxDist * 1.2f)), Color.blue, 1);
                
                if (Physics.Raycast(origin - currentLeg.origin.right * 0.5f, currentLeg.origin.TransformDirection(raycastDir), out RaycastHit hit, legMaxDist * 1.2f, groundLayer))
                {
                    float dist = Vector3.Distance(hit.point, currentTargetPos);

                    if (dist > currentMax && Vector3.Distance(hit.point, origin) < legMaxDist * 0.95f)
                    {
                        currentMax = dist;
                        chosenPos = hit.point;
                    }
                }

                raycastDir = raycastDir.RotateDirection(-2, Vector3.forward);
            }

            return chosenPos;
        }

        
        private IEnumerator MoveLeg(Leg currentLeg, Vector3 endPos)
        {
            currentLeg.isMoving = true;
            Vector3 startPos = transform.InverseTransformPoint(currentLeg.target.position);
            Vector3 localEnd = transform.InverseTransformPoint(endPos);
            float timer = 0;

            while (timer < legMoveDuration)
            {
                timer += Time.deltaTime;
                
                float wantedY = 0;
                float addedY = movementY.Evaluate(timer / legMoveDuration);
                if (Physics.Raycast(currentLeg.target.position + Vector3.up * 1f, -currentLeg.target.up, out RaycastHit hit, 2f,
                        LayerManager.Instance.groundLayer))
                {
                    wantedY = hit.point.y + addedY;
                }
                
                Vector3 wantedPos = Vector3.Lerp(startPos, localEnd, timer / legMoveDuration) + new Vector3(0, addedY, 0);
                
                currentLeg.target.position = transform.TransformPoint(wantedPos);
                
                if(wantedY != 0)
                    currentLeg.target.position = new Vector3(currentLeg.target.position.x, wantedY, currentLeg.target.position.z);

                yield return new WaitForSeconds(Time.deltaTime);
            }

            currentLeg.timerCooldownMove = 0.1f;
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
