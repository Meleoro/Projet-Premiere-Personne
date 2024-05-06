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
        [Header("Debug Parameters")] 
        [SerializeField] private bool debugIK;
        
        [Header("View / Hear Parameters")] 
        [SerializeField] private float earLoudRadius;
        [SerializeField] private float earNormalRadius;
        [SerializeField] private float earLowRadius;
        [SerializeField] private float visionRange;
        [SerializeField] [Range(0, 90)] private int visionRadiusX;
        [SerializeField] [Range(0, 90)] private int visionRadiusY;
        [SerializeField] [Range(0, 10)] private int raycastDensity;

        [Header("Suspision Parameters")]
        [SerializeField] private float suspisionLostSpeed;
        [SerializeField] private float suspisionLostSpeedAggressive;
        [SerializeField] private float suspisionThresholdSuspicieux = 50;
        [SerializeField] private float suspisionThresholdAggressif = 100;
        [SerializeField] private float maxSuspicion = 200;

        [Header("Other Parameters")]
        [SerializeField] private float detectedWaitDuration;

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
        [SerializeField] private Transform headJoint;
        public CreatureSpecialMoves specialMovesScript;
        private List<ICreatureComponent> creatureComponents = new List<ICreatureComponent>();
        private CreatureWaypoints waypointsScript;
        private CreatureMover moveScript;

        
        private void Start()
        {
            creatureComponents = GetComponents<ICreatureComponent>().ToList();
            waypointsScript = GetComponent<CreatureWaypoints>();
            moveScript = GetComponent<CreatureMover>();
        }


        private void Update()
        {
            if(debugIK)
                return;
            
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
            /*if (currentState != CreatureState.none)
                return;*/
            
            
            if (Vector3.Distance(headJoint.position, CharacterManager.Instance.transform.position) < earLoudRadius)
            {
                if (CharacterManager.Instance.currentNoiseType == NoiseType.Loud)
                {
                    currentSuspicion += Time.deltaTime * suspisionAddedCourse;
                }
                
                else if (Vector3.Distance(headJoint.position, CharacterManager.Instance.transform.position) < earNormalRadius)
                {
                    if (CharacterManager.Instance.currentNoiseType == NoiseType.Normal)
                    {
                        currentSuspicion += Time.deltaTime * suspisionAddedMarche;
                    }

                    else if(Vector3.Distance(headJoint.position, CharacterManager.Instance.transform.position) < earLowRadius)
                    {

                        if (CharacterManager.Instance.currentNoiseType == NoiseType.Quiet)
                        {
                            currentSuspicion += Time.deltaTime * suspisionAddedMarcheSneak;
                        }
                    }
                }
            }
        }

        private Vector3 offset = new Vector3(0, 0, 5);
        private void DoViewAI()
        {
            Vector3 currentDir = -headJoint.right;
            currentDir = Quaternion.Euler(0, -visionRadiusY * 0.5f, 0) * currentDir;
            currentDir = headJoint.InverseTransformVector(currentDir);
            currentDir = Quaternion.Euler(0, 0, -visionRadiusX * 0.5f) * currentDir;
            currentDir = headJoint.TransformVector(currentDir);
            
            UIManager.Instance.isInCreatureView = false;
            
            for (int x = 0; x < visionRadiusX; x+= raycastDensity)
            {
                for (int y = 0; y < visionRadiusY; y+= raycastDensity)
                {
                    Debug.DrawLine(headJoint.position, headJoint.position + currentDir * visionRange, Color.cyan, 0.1f);

                    if (Physics.Raycast(headJoint.position, currentDir, out RaycastHit hit, visionRange,
                            LayerManager.Instance.playerGroundLayer))
                    {
                        if (hit.collider.CompareTag("Player") && !CharacterManager.Instance.isHidden)
                        {
                            currentSuspicion += Time.deltaTime * suspisionAddedView;
                            UIManager.Instance.isInCreatureView = true;

                            return;
                        }
                    }
                    
                    currentDir = Quaternion.Euler(0, raycastDensity, 0) * currentDir;
                }

                currentDir = Quaternion.Euler(0, -visionRadiusY - raycastDensity * 0.5f, 0) * currentDir;
                currentDir = headJoint.InverseTransformVector(currentDir);
                currentDir = Quaternion.Euler(0, 0, raycastDensity) * currentDir;
                currentDir = headJoint.TransformVector(currentDir);
            }
        }


        private void ManageSuspision()
        {
            if(currentSuspicion > suspisionThresholdSuspicieux && currentState == CreatureState.none)
            {
                currentState = CreatureState.suspicious;
                waypointsScript.ChangeDestinationSuspicious(CharacterManager.Instance.transform.position);

                moveScript.StartSuspicion();
            }

            else if (currentSuspicion > suspisionThresholdAggressif || currentState == CreatureState.aggressive)
            {
                if (currentState != CreatureState.aggressive)
                    StartCoroutine(moveScript.StartAggressiveBehavior(detectedWaitDuration));

                currentState = CreatureState.aggressive;
                waypointsScript.ChangeDestinationAggressive(CharacterManager.Instance.transform.position);

                if (currentSuspicion <= 0)
                {
                    currentState = CreatureState.none;

                    moveScript.StartWalkSpeed();

                    specialMovesScript.LookLeftRight(2.5f);
                }
            }
        }


        public void TurnAggressive()
        {
            currentState = CreatureState.aggressive;
            currentSuspicion = 200;

            moveScript.StartAggressiveSpeed();
            waypointsScript.ChangeDestinationAggressive(CharacterManager.Instance.transform.position);

            specialMovesScript.CancelSpecialMoves();
        }



        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(headJoint.position, earLoudRadius);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(headJoint.position, earNormalRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(headJoint.position, earLowRadius);

            Gizmos.color = Color.white;
            Gizmos.matrix = Matrix4x4.TRS(headJoint.position, headJoint.rotation * Quaternion.Euler(0, -90, 0), Vector3.one);
            Gizmos.DrawFrustum(Vector3.zero, visionRadiusX, visionRange, 0, (float)-visionRadiusX / visionRadiusY);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.collider.CompareTag("Player"))
            {
                TurnAggressive();
            }
        }
    }
}

public enum CreatureState
{
    none,
    suspicious,
    aggressive
}

