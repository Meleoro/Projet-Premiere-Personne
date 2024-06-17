using System;
using System.Collections;
using System.Collections.Generic;
using Creature;
using Unity.Collections;
using UnityEngine;

namespace IK
{
    public class LegIK : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private int legIndex = 0;
        [SerializeField] private bool inverseArticulation;
        [SerializeField] private bool debug;
        [SerializeField] private bool isFront;
        [SerializeField] private float articulationXRotMultiplicator;
        [SerializeField] private float articulationXRotMax;
        [SerializeField] private float yEffectorOffset;
        [SerializeField] private bool gizmos;

        [Header("Public Infos")] 
        [HideInInspector] public float currentPatouneZRot;
        [HideInInspector] public bool canMove;
        [HideInInspector] public bool isMoving;

        [Header("Private Infos")]
        private float saveOriginalXRot;
        private Vector3 offset1;
        private Vector3 offset2;
        private Vector3 offset3;
        private Vector3[] footOffsetsLocal;
        private Vector3[] footOffsetsWorld;
        [HideInInspector] public Vector3 saveTargetOriginOffset;
        
        [Header("References")]
        [SerializeField] private Transform target;
        private Transform joint0;
        private Transform joint1;
        private Transform joint2;
        public Transform effector;
        private Transform[] foot;
        [SerializeField] private Transform transformRotTrRef;
        [SerializeField] private CreatureMover moveScript;
        [SerializeField] private CreatureManager managerScript;


        private void Awake()
        {
            if(legIndex == 0)
            {
                joint0 = managerScript.creatureRefScript.frontLeg1Bone1;
                joint1 = managerScript.creatureRefScript.frontLeg1Bone2;
                effector = managerScript.creatureRefScript.frontLeg1Foot;
            }
            else if (legIndex == 1)
            {
                joint0 = managerScript.creatureRefScript.frontLeg2Bone1;
                joint1 = managerScript.creatureRefScript.frontLeg2Bone2;
                effector = managerScript.creatureRefScript.frontLeg2Foot;
            }
            else if (legIndex == 2)
            {
                joint0 = managerScript.creatureRefScript.backLeg1Bone1;
                joint1 = managerScript.creatureRefScript.backLeg1Bone2;
                joint2 = managerScript.creatureRefScript.backLeg1Bone3;
                effector = managerScript.creatureRefScript.backLeg1Foot;
            }
            else if (legIndex == 3)
            {
                joint0 = managerScript.creatureRefScript.backLeg2Bone1;
                joint1 = managerScript.creatureRefScript.backLeg2Bone2;
                joint2 = managerScript.creatureRefScript.backLeg2Bone3;
                effector = managerScript.creatureRefScript.backLeg2Foot;
            }


            foot = new Transform[1];
            foot[0] = effector;

            target.position = effector.position;
        }

        private void Start()
        {
            transformRotTrRef = managerScript.backTransformRef;
            saveOriginalXRot = joint0.eulerAngles.x;
            saveTargetOriginOffset = transformRotTrRef.InverseTransformPoint(effector.position);
            
            offset1 = joint0.localEulerAngles;
            offset2 = joint1.localEulerAngles;

            if(joint2)
                offset3 = joint2.localEulerAngles;

            footOffsetsWorld = new Vector3[foot.Length];
            footOffsetsLocal = new Vector3[foot.Length];
            for(int i = 0; i < foot.Length; i++)
            {
                footOffsetsWorld[i] = foot[i].eulerAngles;
                footOffsetsLocal[i] = foot[i].localEulerAngles;
            }
        }


        private void Update()
        {
            joint0.localEulerAngles = new Vector3(offset1.x, offset1.y, 0);
            joint1.localEulerAngles = new Vector3(offset2.x, offset2.y, offset2.z);
            
            if(joint2 != null)
            {
                joint2.localEulerAngles = new Vector3(offset3.x, offset3.y, offset3.z);

                ApplyIK2(joint0, joint1, inverseArticulation);
                ApplyIK2(joint1, joint2, !inverseArticulation);
            }
            else
            {
                ApplyIK2(joint0, joint1, inverseArticulation);
            }
            
            ResetTargets();
            ApplySecondaryRot();
            ApplyPatouneRot();
        }

        private void LateUpdate()
        {
            joint0.rotation = Quaternion.Euler(saveOriginalXRot, joint0.eulerAngles.y, joint0.eulerAngles.z);
        }


        private void ApplyIK2(Transform jointA, Transform jointB, bool inverse, bool getLocalTarget = true)
        {
            // We get the difference in the local space 
            Vector3 localTargetPos = target.position;
            if (getLocalTarget)
            {
                Vector3 targetTranslatedPos = transformRotTrRef.InverseTransformVector(jointA.position - target.position);
                localTargetPos = transformRotTrRef.InverseTransformPoint(target.position);
                localTargetPos = localTargetPos + new Vector3(targetTranslatedPos.x, 0, 0);
                localTargetPos = transformRotTrRef.TransformPoint(localTargetPos);
            }
            
            // We calculate the lengthes of the sides of the triangle
            float lA = Vector3.Distance(jointA.position, jointB.position);
            float lB = Vector3.Distance(jointB.position, effector.position);
            float lC = Vector3.Distance(jointA.position, localTargetPos + new Vector3(0, yEffectorOffset, 0));

            if (lC < 0.2f)
                return;
            
            if(debug)
                Debug.DrawLine(localTargetPos, localTargetPos + new Vector3(0, yEffectorOffset, 0));

            // We get the direction from the origin joint to the target in world space and local space
            Vector3 dif = (jointA.position - (localTargetPos + new Vector3(0, yEffectorOffset, 0)));
            Vector3 localDif = joint0.InverseTransformDirection(dif).normalized;


            float angleAtan = Mathf.Atan2(localDif.y, localDif.x) * Mathf.Rad2Deg;
            float angleJointA;
            float angleJointB;

            // If the target is out of range
            if(lA + lB < lC + 0.001f)
            {
                angleJointA = angleAtan;
                angleJointB = 0;
            }

            // If the target is in the range of the leg
            else
            {
                // Angle alpha
                float cosAngleAlpha = (lC * lC + lA * lA - lB * lB) / (2 * lC * lA);
                float angleAlpha = Mathf.Acos(cosAngleAlpha) * Mathf.Rad2Deg;

                // Angle beta
                float cosAngleBeta = (lB * lB + lA * lA - lC * lC) / (2 * lB * lA);
                float angleBeta = Mathf.Acos(cosAngleBeta) * Mathf.Rad2Deg;

                angleJointA = inverse ? angleAtan - angleAlpha : angleAtan + angleAlpha;
                angleJointB = inverse ? 180f - angleBeta : 180f + angleBeta;
            }


            // We apply the angles to the joints
            Vector3 eulerJoint1 = jointA.localEulerAngles;
            eulerJoint1.z = angleJointA;
            jointA.localEulerAngles = eulerJoint1;

            Vector3 eulerJoint2 = jointB.localEulerAngles;
            eulerJoint2.z = angleJointB;
            jointB.localEulerAngles = eulerJoint2;
        }
        
        private void ApplySecondaryRot()
        {
            Vector3 dif = transformRotTrRef.InverseTransformVector(joint0.position - target.position);

            dif.x = Mathf.Clamp(dif.x, -articulationXRotMax, articulationXRotMax);

            joint0.localEulerAngles = new Vector3(offset1.x - dif.x * articulationXRotMultiplicator, joint0.localEulerAngles.y, joint0.localEulerAngles.z);
            joint1.localEulerAngles = new Vector3(offset2.x + dif.x * 1.5f * articulationXRotMultiplicator, joint1.localEulerAngles.y, joint1.localEulerAngles.z);
        }
        
        private void ApplyPatouneRot()
        {
            // We keep the toes / feet to a given rotation
            for (int i = 0; i < foot.Length; i++)
            {
                foot[i].localEulerAngles = footOffsetsLocal[i];
                foot[i].eulerAngles = new Vector3(footOffsetsWorld[i].x, foot[i].eulerAngles.y, footOffsetsWorld[i].z + currentPatouneZRot); ;
            }
        }

        private void ResetTargets()
        {
            if (managerScript.debugIK) return;
            
            RaycastHit hit;
            
            if (moveScript.navMeshAgent.velocity.magnitude < 0.25 && Mathf.Abs(moveScript.bodyIKScript.currentRotationDif) < 0.05)
            {
                //Debug.Log(moveScript.bodyIKScript.currentRotationDif);
                
                float offset = isFront ? managerScript.legData.frontLegsOffset :managerScript.legData.backLegsOffset;
                
                Vector3 wantedPos = Vector3.Lerp(target.position, transformRotTrRef.TransformPoint(saveTargetOriginOffset), Time.deltaTime * 2f);
                if (Physics.Raycast(wantedPos + Vector3.up * 1f, -Vector3.up, out hit, 5f,
                        LayerManager.Instance.groundLayer))
                {
                    wantedPos.y = hit.point.y;
                }

                target.position = wantedPos;
                canMove = false;
            }
            else
            {
                canMove = true;
            }

            if (!isMoving)
            {
                if (Physics.Raycast(joint0.position, -Vector3.up, out hit, 5f,
                        LayerManager.Instance.groundLayer))
                {
                    target.position = new Vector3(target.position.x, Mathf.Lerp(target.position.y, hit.point.y, Time.deltaTime * 10), target.position.z);
                }
            }
            
            Vector3 currentTargetPos = target.position;
            currentTargetPos = joint0.InverseTransformPoint(currentTargetPos);
            currentTargetPos = new Vector3(currentTargetPos.x, currentTargetPos.y - Mathf.Abs(currentTargetPos.z) * Time.deltaTime * 3, 0);
            target.position = Vector3.Slerp(target.position, joint0.TransformPoint(currentTargetPos), 1);
        }

        private void OnDrawGizmos()
        {
            if (gizmos)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(joint0.position, 0.2f);
                Gizmos.DrawLine(joint0.position,joint1.position);
                Gizmos.DrawSphere(joint1.position, 0.2f);
                Gizmos.DrawLine(joint1.position,joint2.position);
                if(joint2 != null)
                    Gizmos.DrawSphere(joint2.position, 0.2f);
                
                Gizmos.DrawLine(joint2.position,target.position);
                
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(target.position, 0.25f);
            }
        }
    }
}
