using System;
using UnityEngine;

namespace IK
{
    public class TailIK : MonoBehaviour
    {
        [Header("Parameters")] 
        [SerializeField] private float tailWiggleSpeed;
        [SerializeField] private float tailWiggleAmplitude;
        [SerializeField] private int groundedTailJointIndex;       // From which index the tail is stuck to the ground
        [SerializeField] private AnimationCurve groundYHeight;       
        
        [Header("Private Infos")]
        private Vector3 saveEulerTailStart;
        private Vector3 saveLocalEulerTailStart;
        private Vector3 tailGroundTarget;
        private Vector3 joint0Offset;
        private Vector3 saveTailGroundTarget;
        private float atanOrigin;
        private float atanToRemove;
        private float wantedAtan;
        public float[] tailHeightsSave;
        private Vector3[] tailTargets;

        [Header("References")] 
        [SerializeField] private Transform[] tailJoints;
        [SerializeField] private Transform tailStart;


        private void Start()
        {
            saveEulerTailStart = tailStart.eulerAngles;
            saveLocalEulerTailStart = tailStart.localEulerAngles;
            joint0Offset = tailJoints[0].localEulerAngles;
            saveTailGroundTarget = tailStart.InverseTransformPoint(tailJoints[groundedTailJointIndex].position);

            ActualiseSaveHeights();
        }

        private void ActualiseSaveHeights()
        {
            tailHeightsSave = new float[tailJoints.Length];

            for (int i = 0; i < tailJoints.Length; i++)
            {
                RaycastHit hit;
                if (Physics.Raycast(tailJoints[i].position + Vector3.up, Vector3.down, out hit, 10, LayerManager.Instance.groundLayer))
                {
                    tailHeightsSave[i] = Vector3.Distance(tailJoints[i].position, hit.point);
                }
            }
        }


        private void Update()
        {
            KeepTailStraight();
            DoMoveTail1();
            
            ApplyOscillationIK();

            ActualiseGroundTarget();
            ApplyHeightIK();
        }

        private float timer = 0;
        private void DoMoveTail1()
        {
            timer += Time.deltaTime * tailWiggleSpeed;
            
            ChangeAimedDir(Mathf.Sin(timer) * tailWiggleAmplitude);
        }
        

        public void ChangeAimedDir(float newWantedAtan)
        {
            wantedAtan = newWantedAtan;
        }
        

        private void KeepTailStraight()
        {
            tailStart.eulerAngles = new Vector3(saveEulerTailStart.x, tailStart.eulerAngles.y, saveEulerTailStart.z);
        }
        
        private void ApplyOscillationIK()
        {
            // To avoid too much abrupt body rotations
            if (wantedAtan < -80f && atanOrigin > 80f)
                atanOrigin -= 360f;
            else if (atanOrigin < -80f && wantedAtan > 80f)
                atanOrigin += 360f;
            
            if (wantedAtan < -80f && atanToRemove > 80f)
                atanToRemove -= 360f;
            else if (atanToRemove < -80f && wantedAtan > 80f)
                atanToRemove += 360f;

            
            atanOrigin = Mathf.Lerp(atanOrigin, wantedAtan, Time.deltaTime * 2);
            atanToRemove = Mathf.Lerp( atanToRemove, wantedAtan - atanOrigin, Time.deltaTime * 3);
            
            Vector3 eulerBack = tailStart.localEulerAngles;
            eulerBack.y = saveLocalEulerTailStart.y + atanOrigin;
            tailStart.localEulerAngles = eulerBack;

            for (int i = 0; i < tailJoints.Length; i++)
            {
                tailJoints[i].localEulerAngles = new Vector3(tailJoints[i].localEulerAngles.x, atanToRemove,
                    tailJoints[i].localEulerAngles.z);
            }
        }


        #region Height Functions

        private void ActualiseGroundTarget()
        {
            tailTargets = new Vector3[tailJoints.Length];
            RaycastHit hit;
            for(int i = 0; i < tailJoints.Length; i++)
            {
                if (Physics.Raycast(tailJoints[i].position + Vector3.up, Vector3.down, out hit, 10, LayerManager.Instance.groundLayer))
                {
                    tailTargets[i] = hit.point + Vector3.up * tailHeightsSave[i];
                }

                if(i != 0)
                    Debug.DrawLine(tailTargets[i], tailTargets[i - 1]);
            }
        }

        private void ApplyHeightIK()
        {
            tailJoints[0].localEulerAngles = new Vector3(joint0Offset.x, joint0Offset.y, 0);
            for (int i = 0; i < tailJoints.Length - 2; i++)
            {
                ApplyIKOnOneJoint(tailJoints[i], tailJoints[i + 1], tailTargets[i + 2], tailJoints[i + 2].position, true);
            }
        }

        private void ApplyIKOnOneJoint(Transform jointA, Transform jointB, Vector3 target, Vector3 effector, bool inverse)
        {
            float lA = Vector3.Distance(jointA.position, jointB.position);
            float lB = Vector3.Distance(jointB.position, effector);
            float lC = Vector3.Distance(jointA.position, target);

            Vector3 dif = jointA.position - target;
            Vector3 localDif = jointA.InverseTransformDirection(dif).normalized;

            float angleAtan = Mathf.Atan2(localDif.y, localDif.x) * Mathf.Rad2Deg;
            float angleJointA;
            float angleJointB;

            // If the target is out of range
            if (lA + lB < lC + 1f)
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

        #endregion
    }
}
