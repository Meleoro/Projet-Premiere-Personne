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
                legs.Add(new Leg(legsTargets[i], legsOrigins[i]));
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

            if (distOriginTarget > legMaxDist)
            {
                return true;
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
                Debug.DrawRay(origin, currentLeg.origin.TransformDirection(raycastDir * legMaxDist), Color.blue, 1);
                
                if (Physics.Raycast(origin - currentLeg.origin.right * 0.5f, currentLeg.origin.TransformDirection(raycastDir), out RaycastHit hit, legMaxDist, groundLayer))
                {
                    float dist = Vector3.Distance(hit.point, currentTargetPos);

                    if (dist > currentMax)
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
                if (Physics.Raycast(currentLeg.target.position + Vector3.up * 0.5f, -currentLeg.target.up, out RaycastHit hit, 1,
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
            
            currentLeg.isMoving = false;
        }
    }
}

public class Leg
{
    public bool isMoving;
    public Transform target;
    public Transform origin;

    public Leg(Transform target, Transform origin)
    {
        isMoving = false;
        this.target = target;
        this.origin = origin;
    }
}
