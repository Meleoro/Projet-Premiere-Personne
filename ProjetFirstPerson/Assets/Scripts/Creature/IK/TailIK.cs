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
        [SerializeField] private int tailAngleModificatorZ;       
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
        public Vector3[] tailPositionsSave;
        private Vector3[] tailTargets;

        [Header("References")] 
        [SerializeField] private Transform[] tailJoints;
        [SerializeField] private Transform tailStart;
        [SerializeField] private Transform trRef;


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
            tailPositionsSave = new Vector3[tailJoints.Length];
            tailHeightsSave = new float[tailJoints.Length];

            for (int i = 0; i < tailJoints.Length; i++)
            {
                tailPositionsSave[i] = tailStart.InverseTransformPoint(tailJoints[i].position);

                RaycastHit hit;
                if (Physics.Raycast(tailJoints[i].position + Vector3.up, Vector3.down, out hit, 10, LayerManager.Instance.groundLayer))
                {
                    tailHeightsSave[i] = Vector3.Distance(tailJoints[i].position, hit.point);
                }
            }
        }


        private void Update()
        {
            //KeepTailStraight();
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

            Vector3 saveTailStartAngles = tailStart.eulerAngles;

            for (int i = 0; i < tailJoints.Length; i++)
            {
                tailStart.eulerAngles = saveTailStartAngles;
                if (i != 0)
                    tailStart.eulerAngles = new Vector3(tailStart.eulerAngles.x, tailJoints[i - 1].eulerAngles.y, tailStart.eulerAngles.z);

                tailTargets[i] = tailStart.TransformPoint(tailPositionsSave[i]);

                RaycastHit hit;
                if (Physics.Raycast(tailJoints[i].position + Vector3.up, Vector3.down, out hit, 10, LayerManager.Instance.groundLayer))
                {
                    //tailTargets[i] = tailJoints[i - 1].TransformPoint(tailPositionsSave[i]);
                    tailTargets[i].y = tailJoints[i].position.y + (-tailJoints[i].position.y + hit.point.y + tailHeightsSave[i]);
                }

                if (i != 0)
                    Debug.DrawLine(tailTargets[i], tailTargets[i - 1], Color.red);
            }

            tailStart.eulerAngles = saveTailStartAngles;
        }

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
            Vector3 localDif = previous.InverseTransformDirection(dif).normalized;

            float angleAtan = Mathf.Atan2(localDif.y, localDif.x) * Mathf.Rad2Deg;
            float angleJointA;

            angleJointA = angleAtan + tailAngleModificatorZ;

            // We apply the angles to the joints
            Vector3 eulerJoint1 = jointA.localEulerAngles;
            eulerJoint1.z = angleJointA;
            jointA.localEulerAngles = eulerJoint1;
        }

        #endregion
    }
}
