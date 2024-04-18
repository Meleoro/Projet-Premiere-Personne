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
        private float atanOrigin;
        private float atanToRemove;
        private float wantedAtan;

        [Header("References")] 
        [SerializeField] private Transform[] tailJoints;
        [SerializeField] private Transform tailStart;


        private void Start()
        {
            saveEulerTailStart = tailStart.eulerAngles;
            saveLocalEulerTailStart = tailStart.localEulerAngles;
        }


        private void Update()
        {
            KeepTailStraight();
            DoMoveTail1();
            ApplyIK();
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

        private void ApplyIK()
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
    }
}
