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
        private float currentAtan;
        
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
            
            if (atan < -160f && currentAtan > 160f)
                currentAtan -= 360f;
            else if (currentAtan < -160f && atan > 160f)
                currentAtan += 360f;

            currentAtan = Mathf.Lerp(currentAtan, atan, Time.deltaTime * 5);
            
            if (rotateBackWhenMax)
            {
                Vector3 eulerBack = backJoint.localEulerAngles;
                eulerBack.y = currentAtan;
                backJoint.localEulerAngles = eulerBack;

                straightTimer = 1;
            }
            
            atan = atan - currentAtan;
        
            Vector3 eulerJointBody = bodyJoint.localEulerAngles;
            eulerJointBody.y = atan;
            bodyJoint.localEulerAngles = eulerJointBody;
        }
    }
}
