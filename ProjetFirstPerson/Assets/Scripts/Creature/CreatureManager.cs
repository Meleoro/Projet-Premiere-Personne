using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IK;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;


namespace Creature
{
    public class CreatureManager : MonoBehaviour
    {
        [Header("Datas")] 
        public CreatureBodyParamData bodyData;
        public CreatureLegsParamData legData;
        
        [Header("Debug Parameters")] 
        public bool debugIK;
        
        [Header("View / Hear Parameters")] 
        [SerializeField] private float earLoudRadius;
        [SerializeField] private float earNormalRadius;
        [SerializeField] private float earLowRadius;
        [SerializeField] private float visionRange;
        [SerializeField] [Range(0, 90)] private int visionRadiusX;
        [SerializeField] [Range(0, 90)] private int visionRadiusY;
        [SerializeField] [Range(0, 10)] private int raycastDensity;
        [SerializeField] [Range(0, 90)] private int peripheralVisionRadius;

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
        [SerializeField] private float suspisionAddedPeripheralView;
        
        [Header("Valeurs Listen Agressive")]      // Pour chacune de ces valeurs, il faut les voir comme 'combien de suspision sont ajout�es par secondes' car elles seront multipli�s par le delta time (sauf l'int�raction)
        [SerializeField] private float suspisionAddedMarcheAgressive;
        [SerializeField] private float suspisionAddedCourseAgressive;
        [SerializeField] private float suspisionAddedMarcheSneakAgressive;
        //[SerializeField] private float suspisionAddedViewAgressive;

        [Header("Public Infos")]
        public CreatureState currentState;
        public float currentSuspicion;

        [Header("Private Infos")]
        private Vector3 saveRotBackTrRef;
        private Vector3 saveRotFrontTrRef;
        private Vector3 saveRotCollider;

        [Header("References")] 
        [SerializeField] private HeadIK headIK;
        [SerializeField] private BoxCollider creatureCollider;
        public CreatureSpecialMoves specialMovesScript;
        public Transform backTransformRef;
        public Transform frontTransformRef;
        public CreatureReferences creatureRefScript;
        private List<ICreatureComponent> creatureComponents = new List<ICreatureComponent>();
        private CreatureWaypoints waypointsScript;
        private CreatureMover moveScript;
        private CreatureAttack attackScript;


        private void Awake()
        {
            saveRotBackTrRef = creatureRefScript.pantherPelvis.eulerAngles;
            saveRotFrontTrRef = creatureRefScript.spineBones[creatureRefScript.spineBones.Count - 1].eulerAngles;

            ActualiseTransformRefs();
        }

        private void Start()
        {
            AudioManager.Instance.SetAudioSource(1, GetComponent<AudioSource>());
            
            creatureComponents = GetComponents<ICreatureComponent>().ToList();
            waypointsScript = GetComponent<CreatureWaypoints>();
            moveScript = GetComponent<CreatureMover>();
            attackScript = GetComponent<CreatureAttack>();

            CharacterManager.Instance.GetComponent<HealthComponent>().DieAction += () => currentSuspicion = 0;
        }
    

        private void Update()
        {
            ActualiseTransformRefs();

            if (debugIK) return;

            // Do AI Part
            float saveSuspision = currentSuspicion;
            
            DoEarAI();
            DoViewAI();
            ManageSuspision();
            
            if (saveSuspision == currentSuspicion && currentSuspicion > 0 && !attackScript.attacked)
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


        public void ActualiseTransformRefs()
        {
            backTransformRef.position = creatureRefScript.pantherPelvis.position;
            backTransformRef.rotation = Quaternion.Euler(0, -90, 0) * Quaternion.Euler(new Vector3(0, (-saveRotBackTrRef + creatureRefScript.pantherPelvis.eulerAngles).y, 0));

            frontTransformRef.position = creatureRefScript.spineBones[creatureRefScript.spineBones.Count - 1].position;
            frontTransformRef.rotation = Quaternion.Euler(0, -90, 0) * Quaternion.Euler(new Vector3(0, 
                (-saveRotFrontTrRef + creatureRefScript.spineBones[creatureRefScript.spineBones.Count - 1].eulerAngles).y, 0));
        }


        private void DoEarAI()
        {
            float currentDist = Vector3.Distance(headIK.headJointTr.position, CharacterManager.Instance.transform.position);

            switch (CharacterManager.Instance.currentNoiseType)
            {
                case NoiseType.Quiet:
                    if (currentDist < earLowRadius)
                    {
                        if (!GetIsPlayerSafe()) return;

                        if (currentState != CreatureState.aggressive)
                            currentSuspicion += Time.deltaTime * suspisionAddedMarcheSneak;
                        else
                            currentSuspicion += Time.deltaTime * suspisionAddedMarcheSneakAgressive;
                    }
                    break;

                case NoiseType.Normal:
                    if (currentDist < earLowRadius)
                    {
                        if (!GetIsPlayerSafe()) return;

                        if (currentState != CreatureState.aggressive)
                            currentSuspicion += Time.deltaTime * suspisionAddedMarche * 2;
                        else
                            currentSuspicion += Time.deltaTime * suspisionAddedMarcheAgressive * 2;
                    }
                    else if(currentDist < earNormalRadius)
                    {
                        if (!GetIsPlayerSafe()) return;

                        if (currentState != CreatureState.aggressive)
                            currentSuspicion += Time.deltaTime * suspisionAddedMarche;
                        else
                            currentSuspicion += Time.deltaTime * suspisionAddedMarcheAgressive;
                    }
                    break;

                case NoiseType.Loud:
                    if (currentDist < earLowRadius)
                    {
                        if (!GetIsPlayerSafe()) return;

                        if (currentState != CreatureState.aggressive)
                            currentSuspicion += Time.deltaTime * suspisionAddedCourse * 3;
                        else
                            currentSuspicion += Time.deltaTime * suspisionAddedCourseAgressive * 3;
                    }
                    else if (currentDist < earNormalRadius)
                    {
                        if (!GetIsPlayerSafe()) return;

                        if (currentState != CreatureState.aggressive)
                            currentSuspicion += Time.deltaTime * suspisionAddedCourse * 2;
                        else
                            currentSuspicion += Time.deltaTime * suspisionAddedCourseAgressive * 2;
                    }
                    else if(currentDist < earLoudRadius)
                    {
                        if (!GetIsPlayerSafe()) return;

                        if (currentState != CreatureState.aggressive)
                            currentSuspicion += Time.deltaTime * suspisionAddedCourse;
                        else
                            currentSuspicion += Time.deltaTime * suspisionAddedCourseAgressive;
                    }
                    break;
            }
        }
        
        
        private void DoViewAI()
        {
            UIManager.Instance.isInCreatureView = false;

            if (!GetIsPlayerSafe()) return;

            Quaternion saveRot = headIK.headJointTr.rotation;
            
            headIK.headJointTr.rotation = Quaternion.Euler( headIK.headJointTr.rotation.eulerAngles.x,
                headIK.headJointTr.rotation.eulerAngles.y - visionRadiusY * 0.5f - peripheralVisionRadius * 0.5f, 
                headIK.headJointTr.rotation.eulerAngles.z - visionRadiusX * 0.5f + 20);
            
            Vector3 currentDir = - headIK.headJointTr.right;
            bool isInPeripheral = false;
            headIK.StopFollowChara();
            
            for (int x = 0; x < visionRadiusX; x+= raycastDensity)
            {
                for (int y = 0; y < visionRadiusY + peripheralVisionRadius; y+= raycastDensity)
                {
                    isInPeripheral = false;
                    if (y < peripheralVisionRadius * 0.5f || y > visionRadiusY + peripheralVisionRadius * 0.5f)
                        isInPeripheral = true;
                    
                    Debug.DrawLine( headIK.headJointTr.position,  headIK.headJointTr.position + currentDir * visionRange, isInPeripheral ? Color.yellow : Color.cyan, 0.1f);

                    if (Physics.Raycast( headIK.headJointTr.position, currentDir, out RaycastHit hit, visionRange,
                            LayerManager.Instance.playerGroundLayer))
                    {
                        if (hit.collider.CompareTag("Player") && !CharacterManager.Instance.isHidden)
                        {
                            UIManager.Instance.isInCreatureView = true;
                            
                            if(!isInPeripheral)
                                currentSuspicion += Time.deltaTime * suspisionAddedView;
                            else
                                currentSuspicion += Time.deltaTime * suspisionAddedPeripheralView;

                            headIK.FollowChara();
                            headIK.headJointTr.rotation = saveRot;
                            return;
                        }
                    }
                    
                    headIK.headJointTr.rotation = Quaternion.Euler(headIK.headJointTr.rotation.eulerAngles.x,
                        headIK.headJointTr.rotation.eulerAngles.y + raycastDensity, 
                        headIK.headJointTr.rotation.eulerAngles.z);

                    currentDir = -headIK.headJointTr.right;
                }
                
                headIK.headJointTr.rotation = Quaternion.Euler(headIK.headJointTr.rotation.eulerAngles.x,
                    headIK.headJointTr.rotation.eulerAngles.y - visionRadiusY - peripheralVisionRadius - raycastDensity * 0.5f, 
                    headIK.headJointTr.rotation.eulerAngles.z + raycastDensity);

                currentDir = -headIK.headJointTr.right;
            }

            headIK.headJointTr.rotation = saveRot;
        }


        private bool GetIsPlayerSafe()
        {
            NavMeshPath path = new NavMeshPath();
            bool isOkay = moveScript.navMeshAgent.CalculatePath(CharacterManager.Instance.transform.position, path);

            if (path.status == NavMeshPathStatus.PathComplete)
            {
                return true;
            }

            return false;
        }


        private bool wasAtZeroSus;
        private float timerZeroSus;
        private void ManageSuspision()
        {
            if (currentSuspicion != 0)
            {
                if (wasAtZeroSus && timerZeroSus <= 0)
                {
                    wasAtZeroSus = false;
                    timerZeroSus = 3.5f;
                    AudioManager.Instance.PlaySoundOneShot(0, 5, 1);
                }
            }
            
            if(currentSuspicion > suspisionThresholdSuspicieux && currentState == CreatureState.none)
            {
                currentState = CreatureState.suspicious;
                waypointsScript.ChangeDestinationSuspicious(CharacterManager.Instance.transform.position);
                
                AudioManager.Instance.PlaySoundContinuous(0, 0, 1);
                
                moveScript.StartSuspicion();
            }

            else if (currentSuspicion > suspisionThresholdAggressif || currentState == CreatureState.aggressive)
            {
                if (currentState != CreatureState.aggressive)
                {
                    creatureRefScript.coleretteAnimator.SetBool("IsOpen", true);
                    StartCoroutine(moveScript.StartAggressiveBehavior(detectedWaitDuration));
                }

                currentState = CreatureState.aggressive;
                waypointsScript.ChangeDestinationAggressive(CharacterManager.Instance.transform.position);

                if (currentSuspicion <= 0)
                {
                    creatureRefScript.coleretteAnimator.SetBool("IsOpen", false);
                    StartCoroutine(QuitAggressiveBehavior());
                }
            }

            else if(currentSuspicion == 0 && currentState == CreatureState.none)
            {
                moveScript.StartWalkSpeed();

                if (timerZeroSus > 0)
                    timerZeroSus -= Time.deltaTime;
                
                wasAtZeroSus = true;
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
            Gizmos.DrawWireSphere(creatureRefScript.neckBones[creatureRefScript.neckBones.Count - 1].position, earLoudRadius);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(creatureRefScript.neckBones[creatureRefScript.neckBones.Count - 1].position, earNormalRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(creatureRefScript.neckBones[creatureRefScript.neckBones.Count - 1].position, earLowRadius);

            Gizmos.color = Color.white;
            Gizmos.matrix = Matrix4x4.TRS(creatureRefScript.neckBones[creatureRefScript.neckBones.Count - 1].position, 
                creatureRefScript.neckBones[creatureRefScript.neckBones.Count - 1].rotation * Quaternion.Euler(0, -90, 0), Vector3.one);
            Gizmos.DrawFrustum(Vector3.zero, visionRadiusX, visionRange, 0, (float)-visionRadiusY / visionRadiusX);
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

