using Creature;
using System.Collections.Generic;
using UnityEngine;

namespace IK
{
    public class BodyIK : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private float rotationSpeed;

        [Header("Public Infos")] 
        public float currentRotationDif;
        
        [Header("Private Infos")]
        public float currentAtan;
        public float currentAtanBack;
        private Vector3 saveOffset1;
        private Vector3 saveOffset2;
        
        [Header("References")]
        public Transform bodyJoint;
        public Transform backJoint;
        public Transform backTransform;
        [SerializeField] private Transform target;
        [SerializeField] private CreatureLegsMover legsScript;


        private void Start()
        {
            saveOffset1 = bodyJoint.localEulerAngles;
            saveOffset2 = backJoint.localEulerAngles;
        }


        private void Update()
        {
            ApplyMainIK();
            AdaptJointsRotations();
        }

        private void ApplyMainIK()
        {
            Vector3 dif = bodyJoint.position - target.position;
            float atan = Mathf.Atan2(-dif.z, dif.x) * Mathf.Rad2Deg;
            
            // To avoid too much abrupt body roations
            if (atan < -80f && currentAtanBack > 80f)
                currentAtanBack -= 360f;
            else if (currentAtanBack < -80f && atan > 80f)
                currentAtanBack += 360f;
            
            if (atan < -80f && currentAtan > 80f)
                currentAtan -= 360f;
            else if (currentAtan < -80f && atan > 80f)
                currentAtan += 360f;


            currentAtan = Mathf.Lerp(atan - currentAtanBack, 0, Time.deltaTime * 1.5f * rotationSpeed);
            currentAtanBack = Mathf.Lerp(currentAtanBack, atan, Time.deltaTime * 2f * rotationSpeed);
            

            Vector3 eulerBack = backJoint.localEulerAngles;
            eulerBack.y = saveOffset2.y + currentAtanBack;
            backJoint.localEulerAngles = eulerBack;
        
            Vector3 eulerJointBody = bodyJoint.eulerAngles;
            eulerJointBody.y = backJoint.eulerAngles.y + currentAtan;
            bodyJoint.eulerAngles = eulerJointBody;
        }


        private float currentXRotateBodyFront;
        private float currentZRotateBodyFront;
        private float currentXRotateBodyBack;
        private float currentZRotateBodyBack;
        // Changes the ropation of the body joints according to the current positions of the legs
        private void AdaptJointsRotations()
        {
            Vector3 frontAveragePos = Vector3.zero;
            Vector3 backAveragePos = Vector3.zero;

            for (int i = 0; i < legsScript.legs.Count; i++)
            {
                if (legsScript.legs[i].isMoving)
                {
                    if (legsScript.legs[i].isFrontLeg)
                    {
                        Vector3 dif = legsScript.legs[i].originalPos - bodyJoint.InverseTransformPoint(legsScript.legs[i].target.position);

                        if (bodyJoint.InverseTransformPoint(legsScript.legs[i].target.position).z < 0)
                        {
                            frontAveragePos += new Vector3(-dif.y, dif.y, dif.z);
                        }

                        else
                            frontAveragePos += new Vector3(dif.y, dif.y, dif.z);
                    }
                    else
                    {
                        Vector3 dif = legsScript.legs[i].originalPos - bodyJoint.InverseTransformPoint(legsScript.legs[i].target.position);

                        if (bodyJoint.InverseTransformPoint(legsScript.legs[i].target.position).z < 0)
                            backAveragePos += new Vector3(-dif.y, dif.y, dif.z);

                        else
                            backAveragePos += new Vector3(dif.y, dif.y, dif.z);
                    }
                }
            }

            currentXRotateBodyFront = Mathf.Lerp(currentXRotateBodyFront, frontAveragePos.x, Time.deltaTime * 4);
            currentZRotateBodyFront = Mathf.Lerp(currentZRotateBodyFront, frontAveragePos.y, Time.deltaTime * 4);

            currentXRotateBodyBack = Mathf.Lerp(currentXRotateBodyBack, backAveragePos.x, Time.deltaTime * 4);
            currentZRotateBodyBack = Mathf.Lerp(currentZRotateBodyBack, backAveragePos.y, Time.deltaTime * 4);


            bodyJoint.localEulerAngles = new Vector3(saveOffset1.x + currentXRotateBodyFront * 5, bodyJoint.localEulerAngles.y, saveOffset1.z + currentZRotateBodyFront);
            backJoint.localEulerAngles = new Vector3(saveOffset2.x + currentXRotateBodyBack * 5, backJoint.localEulerAngles.y, saveOffset2.z + currentZRotateBodyBack);
        }
    }
}
