using System;
using System.Collections;
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
        
        [Header("Valeurs Listen Agressive")]      // Pour chacune de ces valeurs, il faut les voir comme 'combien de suspision sont ajout�es par secondes' car elles seront multipli�s par le delta time (sauf l'int�raction)
        [SerializeField] private float suspisionAddedMarcheAgressive;
        [SerializeField] private float suspisionAddedCourseAgressive;
        [SerializeField] private float suspisionAddedMarcheSneakAgressive;
        [SerializeField] private float suspisionAddedViewAgressive;


        [Header("Public Infos")]
        public CreatureState currentState;
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

            CharacterManager.Instance.GetComponent<HealthComponent>().DieAction += () => currentSuspicion = 0;
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

            if (currentState == CreatureState.aggressive)
            {
                UIManager.Instance.isInCreatureView = true;
            }
     

            // Moves the body, rotates it, moves the legs etc...
            for (int i = 0; i < creatureComponents.Count; i++)
            {
                creatureComponents[i].ComponentUpdate();
            }
        }


        private void DoEarAI()
        {
            float currentDist = Vector3.Distance(headJoint.position, CharacterManager.Instance.transform.position);

            switch (CharacterManager.Instance.currentNoiseType)
            {
                case NoiseType.Quiet:
                    if (currentDist < earLowRadius)
                    {
                        if (currentState != CreatureState.aggressive)
                            currentSuspicion += Time.deltaTime * suspisionAddedMarche;
                        else
                            currentSuspicion += Time.deltaTime * suspisionAddedMarcheSneakAgressive;
                    }
                    break;

                case NoiseType.Normal:
                    if (currentDist < earLowRadius)
                    {
                        if (currentState != CreatureState.aggressive)
                            currentSuspicion += Time.deltaTime * suspisionAddedMarche * 2;
                        else
                            currentSuspicion += Time.deltaTime * suspisionAddedMarcheAgressive * 2;
                    }
                    else if(currentDist < earNormalRadius)
                    {
                        if (currentState != CreatureState.aggressive)
                            currentSuspicion += Time.deltaTime * suspisionAddedMarche;
                        else
                            currentSuspicion += Time.deltaTime * suspisionAddedMarcheAgressive;
                    }
                    break;

                case NoiseType.Loud:
                    if (currentDist < earLowRadius)
                    {
                        if (currentState != CreatureState.aggressive)
                            currentSuspicion += Time.deltaTime * suspisionAddedCourse * 3;
                        else
                            currentSuspicion += Time.deltaTime * suspisionAddedCourseAgressive * 3;
                    }
                    else if (currentDist < earNormalRadius)
                    {
                        if (currentState != CreatureState.aggressive)
                            currentSuspicion += Time.deltaTime * suspisionAddedCourse * 2;
                        else
                            currentSuspicion += Time.deltaTime * suspisionAddedCourseAgressive * 2;
                    }
                    else if(currentDist < earLoudRadius)
                    {
                        if (currentState != CreatureState.aggressive)
                            currentSuspicion += Time.deltaTime * suspisionAddedCourse;
                        else
                            currentSuspicion += Time.deltaTime * suspisionAddedCourseAgressive;
                    }
                    break;
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
                    StartCoroutine(QuitAggressiveBehavior());
                }
            }
        }

        private IEnumerator QuitAggressiveBehavior()
        {
            currentState = CreatureState.none;

            moveScript.StartWalkSpeed();

            specialMovesScript.LookLeftRight(2.5f);

            yield return new WaitForSeconds(2.5f);

            moveScript.StartWalkSpeed();

            waypointsScript.RestartWaypointBehavior();
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

