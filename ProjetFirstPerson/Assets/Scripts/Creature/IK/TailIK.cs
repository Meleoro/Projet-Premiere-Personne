using System;
using UnityEngine;

namespace IK
{
    public class TailIK : MonoBehaviour
    {
        [Header("Parameters")] 
        [SerializeField] private float tailWiggleSpeed;
        [SerializeField] private float tailWiggleAmplitude;
        
        [Header("Private Infos")]
        private Vector3 saveEulerTailStart;
        private Vector3 saveLocalEulerTailStart;
        private Vector3 tailGroundTarget;
        private float atanOrigin;
        private float atanToRemove;
        private float wantedAtan;
        public float[] tailHeightsSave;
        public Vector3[] tailPositionsSave;
        private Vector3[] tailTargets;

        [Header("References")] 
        [SerializeField] private Transform[] tailJoints;
        [SerializeField] private Transform tailStart;
        [SerializeField] private BodyIK bodyIK;


        private void Start()
        {
            saveEulerTailStart = tailStart.eulerAngles;
            saveLocalEulerTailStart = tailStart.localEulerAngles;

            ActualiseSaveHeights();
        }

        private void ActualiseSaveHeights()
        {
            tailPositionsSave = new Vector3[tailJoints.Length + 1];
            tailHeightsSave = new float[tailJoints.Length + 1];
            tailTargets = new Vector3[tailJoints.Length + 1];

            for (int i = 0; i < tailJoints.Length; i++)
            {
                tailPositionsSave[i] = tailStart.InverseTransformPoint(tailJoints[i].position);
                tailTargets[i] = tailJoints[i].position;

                RaycastHit hit;
                if (Physics.Raycast(tailJoints[i].position + Vector3.up, Vector3.down, out hit, 10, LayerManager.Instance.groundLayer))
                {
                    tailHeightsSave[i] = Vector3.Distance(tailJoints[i].position, hit.point);
                }
            }
            
            tailTargets[tailTargets.Length - 1] =
                tailJoints[tailJoints.Length - 1].position - tailJoints[tailJoints.Length - 1].right * 0.25f;
            
            tailPositionsSave[tailTargets.Length - 1] =
                tailStart.InverseTransformPoint(tailTargets[tailTargets.Length - 1]);
            
            tailHeightsSave[tailTargets.Length - 1] = tailHeightsSave[tailTargets.Length - 2];
        }


        private void Update()
        {
            ActualiseTargets();
            
            ApplyOscillationIK();
            ApplyHeightIK();
        }

        
        private void ActualiseTargets()
        {
            Vector3 saveTailStartAngles = tailStart.eulerAngles;
            
            for (int i = 0; i < tailTargets.Length; i++)
            {
                tailTargets[i] = Vector3.Lerp(tailTargets[i], tailStart.TransformPoint(tailPositionsSave[i]), Time.deltaTime * (4 - (i / ((float)tailTargets.Length) * 2f)));

                RaycastHit hit;
                if (Physics.Raycast(tailTargets[i] + Vector3.up, Vector3.down, out hit, 10, LayerManager.Instance.groundLayer))
                {
                    Vector3 wantedPos = tailTargets[i] +
                                        (-tailTargets[i] + hit.point + new Vector3(0, tailHeightsSave[i], 0));
                    tailTargets[i].y = Mathf.Lerp(tailTargets[i].y, wantedPos.y, Time.deltaTime * 5);
                }

                if (i != 0)
                    Debug.DrawLine(tailTargets[i], tailTargets[i - 1], Color.red);
            }
            
            tailStart.eulerAngles = saveTailStartAngles;
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
                Vector3 currentDir = -tailJoints[i].right;
                float currentAtan = Mathf.Atan2(currentDir.z, currentDir.x) * Mathf.Rad2Deg;
                
                Vector3 newDir = tailTargets[i + 1] - tailJoints[i].position;
                float newAtan = Mathf.Atan2(newDir.z, newDir.x) * Mathf.Rad2Deg;

                float angle = currentAtan - newAtan;
                
                tailJoints[i].localRotation = Quaternion.Lerp(tailJoints[i].localRotation, 
                    Quaternion.Euler(new Vector3(tailJoints[i].localEulerAngles.x, angle, tailJoints[i].localEulerAngles.z)), Time.deltaTime * 5);
            }
        }


        #region Height Functions

        private void ApplyHeightIK()
        {
            for (int i = 1; i < tailJoints.Length - 1; i++)
            {
                ApplyIKOnOneJoint(tailJoints[i], tailTargets[i + 1], tailJoints[i - 1]);
                Debug.DrawLine(tailJoints[i].position, tailTargets[i + 1]);
            }
        }

        private void ApplyIKOnOneJoint(Transform jointA, Vector3 target, Transform previous)
        {
            Vector3 dif = jointA.position - target;
            Vector3 localDif = bodyIK.legsScript.mainTrRotRefBack.InverseTransformDirection(dif).normalized;

            float angleAtan = -Mathf.Atan2(localDif.y, localDif.z) * Mathf.Rad2Deg;
            float angleJointA;

            angleJointA = -angleAtan;

            // We apply the angles to the joints
            Vector3 eulerJoint1 = jointA.eulerAngles;
            eulerJoint1.z = angleJointA;
            jointA.eulerAngles = eulerJoint1;
        }

        #endregion
    }
}
