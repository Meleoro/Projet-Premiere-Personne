using System;
using System.Collections;
using System.Collections.Generic;
using Creature;
using UnityEngine;
using UnityEngine.AI;

namespace IK
{
    public class HeadIK : MonoBehaviour
    {
        [Header("Parameters Base Neck")] 
        [SerializeField] private float inclinationMaxBaseNeck;
        [SerializeField] private float rotationMax;

        [Header("Public infos")] 
        [HideInInspector] public bool isLookingLeftRight;
        
        [Header("Private Infos")] 
        private Vector3 saveBaseNeck;
        private Vector3 saveHeadJoint;
        private float currentRatio;
        private bool followChara;

        [Header("References")] 
        [SerializeField] private CreatureMover moveScript;
        [SerializeField] private CreatureLegsMover legsScript;
        [SerializeField] private NavMeshAgent rb;
        [SerializeField] private Transform baseNeckTr;
        [SerializeField] private Transform headJointTr;
        [SerializeField] private Transform target;


        private void Start()
        {
            saveBaseNeck = baseNeckTr.localEulerAngles;
            saveHeadJoint = headJointTr.localEulerAngles;
        }

        private void Update()
        {
            ModifyInclinationBaseNeck(rb.velocity.magnitude / moveScript.agressiveSpeed);

            if (!isLookingLeftRight)
            {
                ModifyRotationHeadTarget();
            }
        }


        private float currentZ;
        private void ModifyInclinationBaseNeck(float inclinationRatio)
        {
            float Zvalue = Mathf.Lerp(saveBaseNeck.z, saveBaseNeck.z - inclinationMaxBaseNeck, inclinationRatio);
            currentZ = Mathf.Lerp(currentZ, Zvalue, Time.deltaTime * 5);
            
            baseNeckTr.localEulerAngles = new Vector3(
                baseNeckTr.localEulerAngles.x, baseNeckTr.localEulerAngles.y,
                currentZ);    
        }



        public void FollowChara()
        {
            followChara = true;
        }

        public void StopFollowChara()
        {
            followChara = false;
        }
        

        private void ModifyRotationHeadTarget()
        {
            if (followChara)
                target.position = CharacterManager.Instance.transform.position;

            else
            {
                Vector3 dirToRotateTo = (moveScript.wantedPos - moveScript.transform.position).normalized * 10;
            
                Vector3 currentDir = target.position - moveScript.transform.position;
                currentDir = currentDir.normalized * 10;

                currentDir = Vector3.RotateTowards(currentDir, dirToRotateTo.normalized, 1, 1);
            
                target.position = moveScript.transform.position + currentDir;
            }
            
            
            Vector3 dir1 = (target.position - headJointTr.position).normalized;
            float angle1 = Mathf.Atan2(dir1.x, dir1.z) * Mathf.Rad2Deg;

            Vector3 dir2 = legsScript.mainTrRotRefFront.forward;
            float angle2 = Mathf.Atan2(dir2.x, dir2.z) * Mathf.Rad2Deg;

            if (angle1 < 0)
                angle1 += 360;

            if (angle2 < 0)
                angle2 += 360;

            float finalAngle = angle1 - angle2;
            
            ModifyRotationHead(Mathf.Clamp(0.5f + (finalAngle / 90), 0, 1));
        }
        
        
        private void ModifyRotationHead(float rotationRatio)
        {
            Vector3 euler = new Vector3(baseNeckTr.localEulerAngles.x, 
                Mathf.Lerp(saveBaseNeck.y + rotationMax, saveBaseNeck.y - rotationMax, rotationRatio),
                baseNeckTr.localEulerAngles.z);
            
            baseNeckTr.localRotation = Quaternion.Lerp(baseNeckTr.localRotation, Quaternion.Euler(euler), Time.deltaTime * 10);

            euler = new Vector3(Mathf.Lerp(saveHeadJoint.x + rotationMax, saveHeadJoint.x - rotationMax, rotationRatio),
                saveHeadJoint.y, saveHeadJoint.z);
            
            headJointTr.localRotation = Quaternion.Lerp(headJointTr.localRotation, Quaternion.Euler(euler), Time.deltaTime * 10);
        }


        private float timerLookLeftRight;
        public IEnumerator LookLeftThenRight(float duration)
        {
            timerLookLeftRight = 0;
            isLookingLeftRight = true;

            while (timerLookLeftRight < duration * 0.25f)
            {
                timerLookLeftRight += Time.deltaTime;

                ModifyRotationHead(Mathf.Lerp(0.5f, 0, timerLookLeftRight / (duration * 0.25f)));

                yield return null;
            }
            
            yield return new WaitForSeconds(0.1f * duration);

            timerLookLeftRight = 0;
            while (timerLookLeftRight < duration * 0.75f)
            {
                timerLookLeftRight += Time.deltaTime;

                ModifyRotationHead(Mathf.Lerp(0, 1, timerLookLeftRight / (duration * 0.5f)));

                yield return null;
            }
            
            yield return new WaitForSeconds(0.1f * duration);
            
            timerLookLeftRight = 0;
            while (timerLookLeftRight < duration)
            {
                timerLookLeftRight += Time.deltaTime;

                ModifyRotationHead(Mathf.Lerp(1, 0.5f, timerLookLeftRight / (duration * 0.25f)));

                yield return null;
            }
            
            isLookingLeftRight = false;
        }
    }
}
