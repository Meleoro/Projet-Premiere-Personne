using System.Collections.Generic;
using UnityEngine;

namespace IK
{
    public class BodyIK : MonoBehaviour
    {
        [Header("Parameters")] 
        [SerializeField] private bool rotateBackWhenMax;

        [Header("Public Infos")] 
        public float currentRotationDif;
        
        [Header("Private Infos")] 
        private float currentAtan;
        private float currentAtanBack;
        
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
            
            if (atan < -80f && currentAtanBack > 80f)
                currentAtanBack -= 360f;
            else if (currentAtanBack < -80f && atan > 80f)
                currentAtanBack += 360f;
            
            if (atan < -80f && currentAtan > 80f)
                currentAtan -= 360f;
            else if (currentAtan < -80f && atan > 80f)
                currentAtan += 360f;

            currentAtan = Mathf.Lerp(currentAtan, atan, Time.deltaTime * 5);
            currentAtanBack = Mathf.Lerp(currentAtanBack, atan, Time.deltaTime * 3);

            currentRotationDif = Mathf.Abs(currentAtan - currentAtanBack);
            
            if (rotateBackWhenMax)
            {
                Vector3 eulerBack = backJoint.localEulerAngles;
                eulerBack.y = currentAtanBack;
                backJoint.localEulerAngles = eulerBack;
            }
            
            atan = currentAtan - currentAtanBack;
        
            Vector3 eulerJointBody = bodyJoint.localEulerAngles;
            eulerJointBody.y = atan;
            bodyJoint.localEulerAngles = eulerJointBody;
        }
    }
}
