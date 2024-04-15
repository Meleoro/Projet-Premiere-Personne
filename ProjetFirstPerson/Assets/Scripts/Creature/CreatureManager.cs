using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;


namespace Creature
{
    public class CreatureManager : MonoBehaviour
    {
        [Header("View / Hear Parameters")] 
        [SerializeField] private float earLoudRadius;
        [SerializeField] private float earNormalRadius;
        [SerializeField] private float earLowRadius;
        [SerializeField] private float visionRange;
        [SerializeField] [Range(0, 60)] private int visionRadiusX;
        [SerializeField] [Range(0, 60)] private int visionRadiusY;

        [Header("Suspision Parameters")]
        [SerializeField] private float suspisionLostSpeed;
        [SerializeField] private float suspisionLostSpeedAggressive;
        [SerializeField] private float suspisionThresholdSuspicieux = 50;
        [SerializeField] private float suspisionThresholdAggressif = 100;
        [SerializeField] private float maxSuspicion = 200;

        [Header("Valeurs Listen")]      // Pour chacune de ces valeurs, il faut les voir comme 'combien de suspision sont ajout�es par secondes' car elles seront multipli�s par le delta time (sauf l'int�raction)
        [SerializeField] private float suspisionAddedMarche;
        [SerializeField] private float suspisionAddedCourse;
        [SerializeField] private float suspisionAddedMarcheSneak;
        [SerializeField] private float suspisionAddedInteraction;
        [SerializeField] private float suspisionAddedView;


        [Header("Public Infos")]
        [HideInInspector] public CreatureState currentState;
        public float currentSuspicion;

        [Header("Private Infos")]


        [Header("References")] 
        [SerializeField] private Transform mainRotationJoint;
        private List<ICreatureComponent> creatureComponents = new List<ICreatureComponent>();
        private CreatureWaypoints waypointsScript;

        
        private void Start()
        {
            creatureComponents = GetComponents<ICreatureComponent>().ToList();
            waypointsScript = GetComponent<CreatureWaypoints>();
        }


        private void Update()
        {
            // Do AI Part
            float saveSuspision = currentSuspicion;

            DoEarAI();
            DoViewAI();
            ManageSuspision();

            if (saveSuspision == currentSuspicion && currentSuspicion > 0)
                currentSuspicion -= (currentState == CreatureState.aggressive) ? Time.deltaTime * suspisionLostSpeedAggressive : Time.deltaTime * suspisionLostSpeed;

            currentSuspicion = Mathf.Clamp(currentSuspicion, 0, maxSuspicion);

            // Moves the body, rotates it, moves the legs etc...
            for (int i = 0; i < creatureComponents.Count; i++)
            {
                creatureComponents[i].ComponentUpdate();
            }
        }


        private void DoEarAI()
        {
            if (currentState != CreatureState.none)
                return;
            
            
            if (Vector3.Distance(mainRotationJoint.position, CharacterManager.Instance.transform.position) < earLoudRadius)
            {
                if (CharacterManager.Instance.currentNoiseType == NoiseType.Loud)
                {
                    currentSuspicion += Time.deltaTime * suspisionAddedCourse;
                }
                
                else if (Vector3.Distance(mainRotationJoint.position, CharacterManager.Instance.transform.position) < earNormalRadius)
                {
                    if (CharacterManager.Instance.currentNoiseType == NoiseType.Normal)
                    {
                        currentSuspicion += Time.deltaTime * suspisionAddedMarche;
                    }

                    else if(Vector3.Distance(mainRotationJoint.position, CharacterManager.Instance.transform.position) < earLowRadius)
                    {
                        currentSuspicion += Time.deltaTime * suspisionAddedMarcheSneak;
                    }
                }
            }
        }


        private void DoViewAI()
        {
            Vector3 currentDir = -mainRotationJoint.right;
            currentDir = Quaternion.Euler(-visionRadiusX * 0.5f, -visionRadiusY * 0.5f, 0) * currentDir;
            
            for (int x = 0; x < visionRadiusX; x+=4)
            {
                for (int y = 0; y < visionRadiusX; y+=4)
                {
                    Debug.DrawLine(mainRotationJoint.position, mainRotationJoint.position + currentDir * visionRange, Color.cyan, 0.1f);

                    if (Physics.Raycast(mainRotationJoint.position, currentDir, out RaycastHit hit, visionRange,
                            LayerManager.Instance.playerGroundLayer))
                    {
                        if (hit.collider.CompareTag("Player"))
                        {
                            currentSuspicion += Time.deltaTime * suspisionAddedView;

                            return;
                        }
                    }
                    
                    currentDir = Quaternion.Euler(0, 4, 0) * currentDir;
                }
                
                currentDir = Quaternion.Euler(4, -visionRadiusY, 0) * currentDir;
            }
        }


        private void ManageSuspision()
        {
            if(currentSuspicion > suspisionThresholdSuspicieux && currentState == CreatureState.none)
            {
                Debug.Log("IsSuspicious");

                currentState = CreatureState.suspicious;
                waypointsScript.ChangeDestinationSuspicious(CharacterManager.Instance.transform.position);
            }

            else if (currentSuspicion > suspisionThresholdAggressif || currentState == CreatureState.aggressive)
            {
                Debug.Log("IsAggressive");

                currentState = CreatureState.aggressive;
                waypointsScript.ChangeDestinationAggressive(CharacterManager.Instance.transform.position);

                if(currentSuspicion <= 0)
                {
                    currentState = CreatureState.none;
                    waypointsScript.RestartWaypointBehavior();
                }
            }
        }


        public void TurnAggressive()
        {
            currentState = CreatureState.aggressive;
            currentSuspicion = 100;
            
            waypointsScript.ChangeDestinationAggressive(CharacterManager.Instance.transform.position);
        }



        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(mainRotationJoint.position, earLoudRadius);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(mainRotationJoint.position, earNormalRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(mainRotationJoint.position, earLowRadius);

            Gizmos.color = Color.white;
            Gizmos.matrix = Matrix4x4.TRS(mainRotationJoint.position, mainRotationJoint.rotation * Quaternion.Euler(0, -90, 0), Vector3.one);
            Gizmos.DrawFrustum(Vector3.zero, visionRadiusX, visionRange, 0, (float)-visionRadiusX / visionRadiusY);
        }
    }
}

public enum CreatureState
{
    none,
    suspicious,
    aggressive
}

