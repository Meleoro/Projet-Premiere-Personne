using Creature;
using System.Collections.Generic;
using UnityEngine;

namespace IK
{
    public class BodyIK : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private CreatureBodyParamData data;

        [Header("Public Infos")] 
        [HideInInspector] public float currentRotationDif;
        [HideInInspector] public bool hasToDoHugeTurn;
        [HideInInspector] public float currentAtanDif;
        [HideInInspector] public float frontYDif;
        [HideInInspector] public Vector3 saveOffset2;

        [Header("Private Infos")]
        private float currentAtan;
        private float currentAtanBack;
        private Vector3 backLocalPosSave;
        private Vector3[] savesLocalEulers;

        
        [Header("References")]
        [SerializeReference] public Transform[] bodyJoints;
        public Transform bodyJoint;
        public Transform backJoint;
        [SerializeField] private Transform target;
        public CreatureLegsMover legsScript;
        public CreatureMover moveScript;



        private void Start()
        {
            backLocalPosSave = backJoint.localPosition;
            saveOffset2 = backJoint.localEulerAngles;

            savesLocalEulers = new Vector3[bodyJoints.Length];
            for(int i = 0; i < bodyJoints.Length; i++)
            {
                savesLocalEulers[i] = bodyJoints[i].localEulerAngles;
            }
        }


        private void Update()
        {
            ApplyMainIK2();

            ApplyZIK();
            ApplyLegsEffects();
        }


        private void ApplyMainIK2()
        {
            float currentSpeed1 = moveScript.isRunning ? data.aggressiveRotationSpeed : data.rotationSpeed;
            float currentSpeed2 = moveScript.isRunning ? data.aggressiveRotationSpeedFrontJoints : data.rotationSpeedFrontJoints;

            Vector3 dif = backJoint.position - target.position;
            float atan = Mathf.Atan2(-dif.z, dif.x) * Mathf.Rad2Deg;

            // To avoid too much abrupt body rotations
            if (atan < -80f && currentAtanBack > 80f)
                currentAtanBack -= 360f;
            else if (currentAtanBack < -80f && atan > 80f)
                currentAtanBack += 360f;
            
            if (atan < -80f && currentAtan > 80f)
                currentAtan -= 360f;
            else if (currentAtan < -80f && atan > 80f)
                currentAtan += 360f;
            
            // Back Part
            currentAtanBack = Mathf.Lerp(currentAtanBack, atan, Time.deltaTime * currentSpeed1);

            Vector3 eulerBack = backJoint.localEulerAngles;
            eulerBack.y = saveOffset2.y + currentAtanBack;
            backJoint.localEulerAngles = eulerBack;
            
            // Spine Part
            currentAtan = Mathf.Lerp(currentAtan, atan - currentAtanBack, Time.deltaTime * currentSpeed2);
            currentAtan = Mathf.Clamp(currentAtan, -data.maxRotDifFrontBack, data.maxRotDifFrontBack);
            
            currentRotationDif = currentAtan / data.maxRotDifFrontBack;
            currentAtanDif = atan - currentAtanBack;

            float angleAddedPerJoint = currentAtan / bodyJoints.Length;

            for (int i = 0; i < bodyJoints.Length; i++)
            {
                Vector3 eulerJointBody = bodyJoints[i].eulerAngles;
                eulerJointBody.x = 0;
                eulerJointBody.y = backJoint.eulerAngles.y + angleAddedPerJoint * (i + 1);
                bodyJoints[i].eulerAngles = eulerJointBody;
            }
        }

        private void ApplyZIK()
        {
            // Ground Part
            for (int i = 0; i < bodyJoints.Length; i++)
            {
                Vector3 eulerJointBody = bodyJoints[i].localEulerAngles;
                eulerJointBody.z = Mathf.Lerp(bodyJoints[i].localEulerAngles.z, bodyJoints[i].localEulerAngles.z + frontYDif, Time.deltaTime * 5);

                bodyJoints[i].localEulerAngles = eulerJointBody;
            }

            // Legs Part
            Vector3 frontAveragePos;
            Vector3 backAveragePos;

            (frontAveragePos, backAveragePos) = GetLegsAveragePositions();
            float difY = frontAveragePos.y - backAveragePos.y;     // Is > 0 if the front is upside the back

            for (int i = 0; i < bodyJoints.Length; i++)
            {
                Vector3 eulerJointBody = bodyJoints[i].localEulerAngles;
                float changedZ = savesLocalEulers[i].z + difY *
                    data.legsHeightImpactAccordingToSpeed.Evaluate(moveScript.navMeshAgent.velocity.magnitude / moveScript.agressiveSpeed);

                if (Mathf.Abs(changedZ - eulerJointBody.z) > 80)
                    changedZ += 360;

                eulerJointBody.z = Mathf.Lerp(bodyJoints[i].localEulerAngles.z , changedZ, Time.deltaTime * 15);

                bodyJoints[i].localEulerAngles = eulerJointBody;
            }
        }

        private (Vector3, Vector3) GetLegsAveragePositions()
        {
            Vector3 frontAveragePos = Vector3.zero;
            Vector3 backAveragePos = Vector3.zero;

            for (int i = 0; i < legsScript.legs.Count; i++)
            {
                if (legsScript.legs[i].isFrontLeg)
                {
                    frontAveragePos += legsScript.legs[i].target.position;
                }
                else
                {
                    backAveragePos += legsScript.legs[i].target.position;
                }
            }

            return (frontAveragePos, backAveragePos);
        }


        private void ApplyLegsEffects()
        {
            float frontAddedY = 0;
            float backAddedY = 0;
            float reductiveFactor = 0.5f;

            for(int i = 0; i < legsScript.legs.Count; i++)
            {
                if (legsScript.legs[i].isFrontLeg)
                {
                    frontAddedY += legsScript.legs[i].currentAddedY * reductiveFactor;
                }

                else
                {
                    backAddedY += legsScript.legs[i].currentAddedY * reductiveFactor;
                }
            }

            float followSpeed = 3;

            backJoint.transform.localPosition = Vector3.Lerp(backJoint.transform.localPosition, backLocalPosSave + Vector3.up * backAddedY, Time.deltaTime * followSpeed);
        }
    }
}
