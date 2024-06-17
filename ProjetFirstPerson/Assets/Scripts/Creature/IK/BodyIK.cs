using Creature;
using System.Collections.Generic;
using UnityEngine;

namespace IK
{
    public class BodyIK : MonoBehaviour
    {
        [Header("Public Infos")] 
        [HideInInspector] public float currentRotationDif;
        [HideInInspector] public float currentAtanDif;
        public float frontYDif;
        [HideInInspector] public Vector3 saveOffset2;

        [Header("Private Infos")]
        private CreatureBodyParamData data;
        private CreatureLegsParamData dataLegs;
        private float currentAtan;
        private float currentAtanBack;
        private float saveThoraxX;
        private Vector3 backLocalPosSave;
        private Vector3[] savesLocalEulers;
        private Quaternion saveRotThorax;
        public float rotationSpeedModifier;
        
        [Header("References")]
        [SerializeReference] private CreatureReferences referencesScript;
        [HideInInspector] public Transform[] bodyJoints;
        [HideInInspector] public Transform bodyJoint;
        [HideInInspector] public Transform backJoint;
        public Transform target;
        public CreatureLegsMover legsScript;
        public CreatureMover moveScript;
        private CreatureManager manager;
        public HeadIK headIK;


        private void Awake()
        {
            bodyJoint = referencesScript.pantherRibCage;
            backJoint = referencesScript.pantherPelvis;

            bodyJoints = new Transform[referencesScript.spineBones.Count];

            for(int i = 0; i < referencesScript.spineBones.Count; i++)
            {
                bodyJoints[i] = referencesScript.spineBones[i];
            } 
        }


        private void Start()
        {
            data = moveScript.GetComponent<CreatureManager>().bodyData;
            dataLegs = moveScript.GetComponent<CreatureManager>().legData;
            manager = moveScript.GetComponent<CreatureManager>();

            backLocalPosSave = backJoint.localPosition;
            saveOffset2 = backJoint.localEulerAngles;

            saveThoraxX = bodyJoint.eulerAngles.x;

            savesLocalEulers = new Vector3[bodyJoints.Length];
            for(int i = 0; i < bodyJoints.Length; i++)
            {
                savesLocalEulers[i] = bodyJoints[i].localEulerAngles;
            }

            saveRotThorax = bodyJoint.localRotation;
        }


        private void Update()
        {
            ApplyMainIK2();

            ApplyZIK();

            RotatePelvis();
        }


        private void ApplyMainIK2()
        {
            float currentSpeed1 = moveScript.isRunning ? data.aggressiveRotationSpeed : data.rotationSpeed;
            float currentSpeed2 = moveScript.isRunning ? data.aggressiveRotationSpeedFrontJoints : data.rotationSpeedFrontJoints;

            if (Mathf.Abs(currentRotationDif) < 0.05f)
                rotationSpeedModifier -= Time.deltaTime;
            
            else
                rotationSpeedModifier += Time.deltaTime;

            rotationSpeedModifier = Mathf.Clamp(rotationSpeedModifier, 0.1f, 1f);

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
            currentAtanBack = Mathf.Lerp(currentAtanBack, atan, Time.deltaTime * Mathf.Abs(currentSpeed1 / (atan - currentAtanBack)) * rotationSpeedModifier);

            Vector3 eulerBack = backJoint.localEulerAngles;
            eulerBack.y = saveOffset2.y + currentAtanBack;
            backJoint.localRotation = Quaternion.Lerp(backJoint.localRotation, Quaternion.Euler(eulerBack), 0.8f);
            
            // Spine Part
            currentAtan = Mathf.Lerp(currentAtan, atan - currentAtanBack, Time.deltaTime * Mathf.Abs(currentSpeed2 / (atan - currentAtanBack)) * rotationSpeedModifier);
            currentAtan = Mathf.Clamp(currentAtan, -data.maxRotDifFrontBack, data.maxRotDifFrontBack);
            
            currentRotationDif = currentAtan /( data.maxRotDifFrontBack * 0.95f);
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
                eulerJointBody.z = Mathf.Lerp(bodyJoints[i].localEulerAngles.z, bodyJoints[i].localEulerAngles.z + frontYDif, Time.deltaTime * 10);

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
                float changedZ = bodyJoints[i].localEulerAngles.z + difY *
                    data.legsHeightImpactAccordingToSpeed.Evaluate(moveScript.navMeshAgent.velocity.magnitude / moveScript.agressiveSpeed);

                if (Mathf.Abs(changedZ - eulerJointBody.z) > 80)
                    changedZ -= 360;
                
                if(Mathf.Abs(changedZ - eulerJointBody.z) < -80)
                    changedZ += 360;

                eulerJointBody.z = Mathf.Lerp(bodyJoints[i].localEulerAngles.z , changedZ, Time.deltaTime * 15);

                bodyJoints[i].localRotation = Quaternion.Euler(eulerJointBody);
            }
        }

        private (Vector3, Vector3) GetLegsAveragePositions()
        {
            Vector3 frontAveragePos = Vector3.zero;
            Vector3 backAveragePos = Vector3.zero;

            if (manager.debugIK)
            {
                for (int i = 0; i < legsScript.legs.Count; i++)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(legsScript.legs[i].target.position + Vector3.up * 1f, -legsScript.legs[i].target.up, out hit, 3f,
                         LayerManager.Instance.groundLayer))
                    {
                        legsScript.legs[i].currentAddedY = -legsScript.legs[i].origin.position.y + legsScript.legs[i].target.position.y;
                    }
                }
            }
            
            for (int i = 0; i < legsScript.legs.Count; i++)
            {
                if (legsScript.legs[i].isFrontLeg && (legsScript.legs[i].isMoving || manager.debugIK))
                {
                    frontAveragePos += Vector3.up * legsScript.legs[i].currentAddedY;
                }
                else if(legsScript.legs[i].isMoving || manager.debugIK)
                {
                    backAveragePos += Vector3.up * legsScript.legs[i].currentAddedY;
                }
            }

            return (frontAveragePos, backAveragePos);
        }


        private void ApplyLegsEffects()
        {
            float frontAddedY = 0;
            float backAddedY = 0;
            float reductiveFactor = 0.08f;

            /*if (moveScript.isRunning && moveScript.navMeshAgent.velocity.magnitude > 2)
                reductiveFactor = 0.15f;*/

            if (!manager.debugIK)
            {
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
            }
            else
            {
                Vector3 frontAveragePos;
                Vector3 backAveragePos;

                (frontAveragePos, backAveragePos) = GetLegsAveragePositions();
                frontAddedY = frontAveragePos.y - backAveragePos.y;
                backAddedY = -frontAveragePos.y + backAveragePos.y;
            }
            
            backJoint.transform.position += new Vector3(0, backAddedY, 0);
        }


        private float currentRotXPelvis;
        private float currentRotXThorax;
        [HideInInspector] public float currentAddedFrontY;
        [HideInInspector] public float currentAddedBackY;
        private void RotatePelvis()
        {
            float rotXPelvis = 0;
            float rotXThorax = 0;

            Quaternion saveRotSpine1 = bodyJoints[0].rotation;
            Quaternion saveRotNeck = headIK.baseNeckTr.rotation;

            for (int i = 0; i < legsScript.legs.Count; i++)
            {
                if (legsScript.legs[i].isFrontLeg)
                {
                    rotXThorax += legsScript.mainTrRotRefFront.InverseTransformPoint(legsScript.legs[i].target.position).z * data.thorasLegXRotationMultiplicator
                        * (legsScript.mainTrRotRefFront.InverseTransformPoint(legsScript.legs[i].origin.position).x > 0 ? -1f : 1f);

                    rotXThorax = Mathf.Clamp(rotXThorax, -data.maxThoraxLegXRotation, data.maxThoraxLegXRotation);
                }

                else
                {
                    rotXPelvis += legsScript.mainTrRotRefBack.InverseTransformPoint(legsScript.legs[i].target.position).z * data.pelvisLegXRotationMultiplicator *
                                  (legsScript.mainTrRotRefBack.InverseTransformPoint(legsScript.legs[i].origin.position).x > 0 ? -1 : 1);
                    
                    rotXPelvis = Mathf.Clamp(rotXPelvis, -data.maxPelvisLegXRotation, data.maxPelvisLegXRotation);
                }
            }

            currentRotXPelvis = Mathf.Lerp(currentRotXPelvis, rotXPelvis, Time.deltaTime * 10);
            currentRotXThorax = Mathf.Lerp(currentRotXThorax, rotXThorax, Time.deltaTime * 10);

            backJoint.rotation = Quaternion.Euler(currentRotXPelvis, backJoint.eulerAngles.y, backJoint.eulerAngles.z);
            bodyJoint.rotation = Quaternion.Euler(saveThoraxX + currentRotXThorax, bodyJoint.eulerAngles.y, bodyJoint.eulerAngles.z);
            bodyJoint.localRotation = Quaternion.Euler(bodyJoint.localEulerAngles.x, saveRotThorax.eulerAngles.y, saveRotThorax.eulerAngles.z);

            bodyJoints[0].rotation = saveRotSpine1;
            //headIK.baseNeckTr.rotation = saveRotNeck;

            currentAddedBackY = - Mathf.Abs(currentRotXPelvis) / 40;
            currentAddedFrontY = 0.1f - Mathf.Abs(currentRotXThorax) / 40;

            ApplyLegsEffects();
        }
    }
}
