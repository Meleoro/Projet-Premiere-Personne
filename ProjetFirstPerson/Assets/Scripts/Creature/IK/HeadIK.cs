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
        [Header("Public infos")] 
        [HideInInspector] public bool isLookingLeftRight;
        
        [Header("Private Infos")] 
        private CreatureBodyParamData data;
        private Vector3 saveBaseNeck;
        private Vector3 saveHeadJoint;
        private Quaternion saveResetBaseNeck;
        private bool followChara;

        [Header("References")]
        [SerializeField] private CreatureReferences referencesScript;
        [SerializeField] private CreatureMover moveScript;
        [SerializeField] private CreatureLegsMover legsScript;
        [SerializeField] private NavMeshAgent rb;
        [HideInInspector] public Transform baseNeckTr;
        [HideInInspector] public Transform headJointTr;
        [SerializeField] private Transform target;


        private void Awake()
        {
            baseNeckTr = referencesScript.neckBones[0];
            headJointTr = referencesScript.neckBones[referencesScript.neckBones.Count - 1];
        }

        private void Start()
        {
            data = moveScript.GetComponent<CreatureManager>().bodyData;
            
            saveBaseNeck = baseNeckTr.localEulerAngles;
            saveHeadJoint = headJointTr.localEulerAngles;

            saveResetBaseNeck = baseNeckTr.rotation;
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
            float Zvalue = Mathf.Lerp(saveBaseNeck.z, saveBaseNeck.z - data.inclinationMaxNeck, Mathf.Clamp(inclinationRatio, 0, 1));
            
            
            currentZ = Zvalue;
            //currentZ = Zvalue;
            
            /*baseNeckTr.localRotation = Quaternion.Lerp(transform.localRotation ,Quaternion.Euler(new Vector3(
                baseNeckTr.localEulerAngles.x, baseNeckTr.localEulerAngles.y, currentZ)), Time.deltaTime * 5);    */
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
            {
                Vector3 dirToRotateTo = (CharacterManager.Instance.transform.position- headJointTr.transform.position).normalized * 10;
                Vector3 currentDir = target.position - headJointTr.transform.position;
                
                currentDir = currentDir.normalized * 10;

                currentDir = Vector3.RotateTowards(currentDir, dirToRotateTo.normalized, 1, 1);
            
                target.position = headJointTr.transform.position + currentDir;
            }

            else
            {
                Vector3 dirToRotateTo = (moveScript.wantedPos - moveScript.transform.position).normalized * 10;
                if (Vector3.Distance(moveScript.wantedPos, moveScript.transform.position) < 1f)
                    dirToRotateTo = (baseNeckTr.position - moveScript.transform.position).normalized * 10;

                dirToRotateTo.y = 0;

                Vector3 currentDir = target.position - moveScript.transform.position;
                currentDir = currentDir.normalized * 10;

                currentDir = Vector3.RotateTowards(currentDir, dirToRotateTo.normalized, 1, 1);
            
                target.position = moveScript.transform.position + currentDir;
            }
            
            
            Vector3 dir1 = (target.position - headJointTr.position).normalized;
            float angle1 = Mathf.Atan2(dir1.x, dir1.z) * Mathf.Rad2Deg;

            Vector3 dir2 = legsScript.mainTrRotRefFront.forward;
            float angle2 = Mathf.Atan2(dir2.x, dir2.z) * Mathf.Rad2Deg;
            
            if (angle1 < -80f && angle2 > 80f)
                angle2 -= 360f;
            else if (angle2 < -80f && angle1 > 80f)
                angle2 += 360f;
            
            if (angle1 < -80f && angle2 > 80f)
                angle1 -= 360f;
            else if (angle2 < -80f && angle1 > 80f)
                angle1 += 360f;
            
            float finalAngle = angle1 - angle2;
                
            
            ModifyRotationHead(Mathf.Clamp(0.5f + (finalAngle / 90), 0, 1));
        }


        private float saveZ = 0;
        private void ModifyRotationHead(float rotationRatio)
        {
            Vector3 euler = new Vector3(Mathf.Lerp(saveHeadJoint.x + data.rotationMaxNeck, saveHeadJoint.x - data.rotationMaxNeck, rotationRatio),
                Mathf.Lerp(saveBaseNeck.y + data.rotationMaxNeck, saveBaseNeck.y - data.rotationMaxNeck, rotationRatio),
                currentZ);

            saveZ = currentZ;
            
            baseNeckTr.localRotation = Quaternion.Lerp(baseNeckTr.localRotation, Quaternion.Euler(euler), Time.deltaTime * 5);
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
