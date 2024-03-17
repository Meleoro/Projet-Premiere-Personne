using System.Collections.Generic;
using UnityEngine;

namespace IK
{
    public class BodyIK : MonoBehaviour
    {
        [Header("Parameters")] 
        [SerializeField] private float maxBodyJointRot;
        [SerializeField] private bool rotateBackWhenMax;
        
        [Header("References")]
        [SerializeField] private Transform bodyJoint;
        [SerializeField] private Transform bodyBack;
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
                    
                    Vector3 eulerBack = bodyBack.localEulerAngles;
                    eulerBack.y = addedY;
                    bodyBack.localEulerAngles = eulerBack;
                }
            }
            
            atan = Mathf.Clamp(atan, -maxBodyJointRot, maxBodyJointRot);
        
            Vector3 eulerJointBody = bodyJoint.localEulerAngles;
            eulerJointBody.y = atan;
            bodyJoint.localEulerAngles = eulerJointBody;
        }
    }
}
