using System.Collections.Generic;
using UnityEngine;

namespace IK
{
    public class BodyIK : MonoBehaviour
    {
        [Header("Parameters")] 
        [SerializeField] private float maxBodyJointRot;
        [SerializeField] private bool rotateBackWhenMax;

        [Header("Private Infos")] 
        private float straightTimer;
        
        [Header("References")]
        public Transform bodyJoint;
        public Transform backJoint;
        public Transform backTransform;
        [SerializeField] private Transform target;
        

        private void Update()
        {
            ApplyMainIK();
        }

        private void ApplyMainIK()
        {
            Vector3 dif = bodyJoint.position - target.position;
            float atan = Mathf.Atan2(-dif.z, dif.x) * Mathf.Rad2Deg;

            if (rotateBackWhenMax)
            {
                if (Mathf.Abs(atan) > maxBodyJointRot)
                {
                    float addedY = atan - Mathf.Clamp(atan, -maxBodyJointRot, maxBodyJointRot);
                    
                    Vector3 eulerBack = backJoint.localEulerAngles;
                    eulerBack.y = addedY;
                    backJoint.localEulerAngles = eulerBack;

                    straightTimer = 1;
                }
                else
                {
                    Debug.Log(straightTimer);
                    straightTimer -= Time.deltaTime;

                    float addedY = (backJoint.localEulerAngles.y > 0) ? Mathf.Lerp(0, maxBodyJointRot, straightTimer) : Mathf.Lerp(0, -maxBodyJointRot, straightTimer);
                    
                    Vector3 eulerBack = backJoint.localEulerAngles;
                    eulerBack.y = addedY;
                    backJoint.localEulerAngles = eulerBack;
                }
            }
            
            atan = Mathf.Clamp(atan, -maxBodyJointRot, maxBodyJointRot);
        
            Vector3 eulerJointBody = bodyJoint.localEulerAngles;
            eulerJointBody.y = atan;
            bodyJoint.localEulerAngles = eulerJointBody;
        }
    }
}
