using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IK
{
    public class LegIK : MonoBehaviour
    {
        [Header("Parameters")] 
        [SerializeField] private bool inverseArticulation;
    
        [Header("Private Infos")]
        private float l1;
        private float l2;
        private Vector3 offset1;
        private Vector3 offset2;
        
        [Header("References")]
        [SerializeField] private Transform joint0;
        [SerializeField] private Transform joint1;
        [SerializeField] private Transform effector;
        [SerializeField] private Transform target;
        [SerializeField] private Transform foot;


        private void Start()
        {
            l1 = Vector3.Distance(joint0.position, joint1.position);
            l2 = Vector3.Distance(joint1.position, effector.position);

            offset1 = joint0.localEulerAngles;
            
            if(foot)
                offset2 = foot.eulerAngles;
        }

        private void Update()
        {
            ApplyIK();
        }

        private void ApplyIK()
        {
            // First we rotate on the y axis
            Vector3 dif = (joint0.position - target.position);
            //float angleAtan2 = Mathf.Atan2(-dif.z, dif.x) * Mathf.Rad2Deg;
            joint0.localEulerAngles = new Vector3(offset1.x, offset1.y, 0);
            
            
            float l3 = Vector3.Distance(joint0.position, target.position);

            // Angle between joint0 and target
            Vector3 localDif = joint0.InverseTransformDirection(dif).normalized;
            float angleAtan = inverseArticulation ? Mathf.Atan2(localDif.y, -localDif.x) * Mathf.Rad2Deg - 90f : Mathf.Atan2(localDif.y, localDif.x) * Mathf.Rad2Deg - 90f;
            
            float joint1Angle;
            float joint2Angle;

            if (l1 + l2 < l3 + 0.015f)
            {
                joint1Angle = inverseArticulation ? -angleAtan : angleAtan;
                joint2Angle = 0f;
            }
            else
            {
                // Angle alpha
                float cosAngleAlpha = (l3 * l3 + l1 * l1 - l2 * l2) / (2 * l3 * l1);
                float angleAlpha = Mathf.Acos(cosAngleAlpha) * Mathf.Rad2Deg;
            
                // Angle beta
                float cosAngleBeta = (l2 * l2 + l1 * l1 - l3 * l3) / (2 * l2 * l1);
                float angleBeta = Mathf.Acos(cosAngleBeta) * Mathf.Rad2Deg;

                if (inverseArticulation)
                {
                    joint1Angle = -angleAtan + angleAlpha;
                    joint2Angle = -180f + angleBeta;
                }
                else
                {
                    joint1Angle = angleAtan - angleAlpha;
                    joint2Angle = 180f - angleBeta;
                }
            }

            float addedAngle = Vector3.Angle(joint0.InverseTransformDirection(Vector3.down), -joint0.right);
            

            Vector3 eulerJoint1 = joint0.localEulerAngles;
            eulerJoint1.z = joint1Angle + addedAngle;
            //eulerJoint1.y = angleAtan2;
            joint0.localEulerAngles = eulerJoint1;
            
            Vector3 eulerJoint2 = joint1.localEulerAngles;
            eulerJoint2.z = joint2Angle;
            joint1.localEulerAngles = eulerJoint2;

            if(foot)
                foot.transform.eulerAngles = offset2;
        }
    }
}
