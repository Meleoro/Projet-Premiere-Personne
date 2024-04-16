using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace IK
{
    public class HeadIK : MonoBehaviour
    {
        [Header("Parameters Base Neck")] 
        [SerializeField] private float inclinationMaxBaseNeck;
        [SerializeField] private float rotationMax;
        [SerializeField] private float speedMaxInclination;

        [Header("Public infos")] 
        [HideInInspector] public bool isLookingLeftRight;
        
        [Header("Private Infos")] 
        private Vector3 saveBaseNeck;
        private Vector3 saveHeadJoint;

        [Header("References")] 
        [SerializeField] private NavMeshAgent rb;
        [SerializeField] private Transform baseNeckTr;
        [SerializeField] private Transform headJointTr;


        private void Start()
        {
            saveBaseNeck = baseNeckTr.localEulerAngles;
            saveHeadJoint = headJointTr.localEulerAngles;
        }

        private void Update()
        {
            ModifyInclinationBaseNeck(rb.velocity.magnitude / speedMaxInclination);
        }


        private void ModifyInclinationBaseNeck(float inclinationRatio)
        {
            baseNeckTr.localEulerAngles = new Vector3(baseNeckTr.localEulerAngles.x, baseNeckTr.localEulerAngles.y,
                Mathf.Lerp(saveBaseNeck.z, saveBaseNeck.z - inclinationMaxBaseNeck, inclinationRatio));

            /*headJointTr.localEulerAngles = new Vector3(Mathf.Lerp(originalInclinationHeadJoint.x, originalInclinationHeadJoint.x - inclinationMaxBaseNeck, inclinationRatio),
                originalInclinationHeadJoint.y, originalInclinationHeadJoint.z);*/
        }
        
        
        private void ModifyRotationHead(float rotationRatio)
        {
            baseNeckTr.localEulerAngles = new Vector3(baseNeckTr.localEulerAngles.x, 
                Mathf.Lerp(saveBaseNeck.y + rotationMax, saveBaseNeck.y - rotationMax, rotationRatio),
                baseNeckTr.localEulerAngles.z);
            
            headJointTr.localEulerAngles = new Vector3(Mathf.Lerp(saveHeadJoint.x + rotationMax, saveHeadJoint.x - rotationMax, rotationRatio),
                saveHeadJoint.y, saveHeadJoint.z);
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
