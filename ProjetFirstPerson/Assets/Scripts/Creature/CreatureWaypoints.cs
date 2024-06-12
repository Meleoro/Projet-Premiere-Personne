using System.Collections;
using System.Collections.Generic;
using IK;
using UnityEngine;
using UnityEngine.AI;

namespace Creature
{
    [RequireComponent(typeof(CreatureMover))]
    public class CreatureWaypoints : MonoBehaviour, ICreatureComponent
    {
        [Header("Parameters")]
        [SerializeField] private List<WaypointsManager> possiblePaths = new List<WaypointsManager>();
        [SerializeField] public int numberOfWaypointBeforeGoNear;
        
        [SerializeField] private float suspicionWaitDuration;
        [SerializeField] private float suspicionPlaceOffsetMultiplier = 2.5f;

        [Header("Public Infos")]
        public List<Waypoint> waypoints = new List<Waypoint>();
        public bool isAttacking;

        [Header("Private Infos")]
        private bool stoppedNormalBehavior;
        private bool didWaypointAction;
        private bool isDoingSpecialAction;
        public int currentWaypointCountNear;
        private Waypoint currentWaypoint;
        private int currentIndex;
        private float waitTimer;
        private Vector3 placeToGo;

        [Header("References")] 
        private CreatureMover creatureMoverScript;
        private CreatureManager mainScript;



        private void Start()
        {
            currentWaypointCountNear = 0;

            if (possiblePaths.Count > 0) 
                waypoints = possiblePaths[0].waypoints;

            creatureMoverScript = GetComponent<CreatureMover>();
            mainScript = GetComponent<CreatureManager>();

            creatureMoverScript.wantedPos = waypoints[0].transform.position;
            currentIndex = 0;
            currentWaypoint = waypoints[0];

            CharacterManager.Instance.GetComponent<HealthComponent>().DieAction += ResetCurrentWaypointManager;
        }


        private float previousDist = 0;
        private float blockedTimer = 0;
        public void ComponentUpdate()
        {
            float currentDist = Vector2.Distance(new Vector2(transform.position.x, transform.position.z),
                new Vector2(currentWaypoint.transform.position.x, currentWaypoint.transform.position.z));
            
            if (!stoppedNormalBehavior)
            {
                if(currentDist < 1f)
                {
                    ReachedWaypoint();
                }
                else if (Mathf.Abs(currentDist - previousDist) < 0.01)
                {
                    blockedTimer += Time.deltaTime;

                    if (blockedTimer > 5)
                    {
                        blockedTimer = 0;
                        NextWaypoint();
                    }
                }
                else
                {
                    blockedTimer = 0;
                }
            }

            else if(mainScript.currentState != CreatureState.aggressive)
            {
                currentDist = Vector2.Distance(new Vector2(transform.position.x, transform.position.z),
                    new Vector2(placeToGo.x, placeToGo.z));
                
                if (currentDist < 2f)
                {
                    ReachedPlaceToGo();
                }
                
                else if (Mathf.Abs(currentDist - previousDist) < 0.01)
                {
                    blockedTimer += Time.deltaTime;

                    if (blockedTimer > 5)
                    {
                        blockedTimer = 0;
                        StartCoroutine(StopLookLeftRight(2.5f));
                
                        creatureMoverScript.forcedRot = Vector3.zero;
                        didWaypointAction = false;
                        mainScript.currentState = CreatureState.none;
                
                        AudioManager.Instance.FadeOutAudioSource(2.5f, 1);

                        creatureMoverScript.StartWalkSpeed();
                    }
                }
                else
                {
                    blockedTimer = 0;
                }
            }

            previousDist = currentDist;
        }


        #region NORMAL BEHAVIOR

        private void ReachedWaypoint()
        {
            waitTimer += Time.deltaTime;
            creatureMoverScript.forcedRot = Vector3.forward.RotateDirection(currentWaypoint.roationAngleY, Vector3.up);

            if (waitTimer > currentWaypoint.timeBeforeWaypointAction && !didWaypointAction)
            {
                didWaypointAction = true;

                switch (currentWaypoint.waypointAction)
                {
                    case WaypointAction.LookLeftThenRight :
                        mainScript.specialMovesScript.LookLeftRight(currentWaypoint.timeWaypointAction);
                        break;
                }
            }
            
            if(waitTimer > currentWaypoint.waitTimer)
            {               
                creatureMoverScript.forcedRot = Vector3.zero;
                didWaypointAction = false;

                NextWaypoint();
            }
        }

        private void NextWaypoint()
        {
            currentIndex++;
            if(currentIndex >= waypoints.Count) currentIndex = 0;

            currentWaypointCountNear++;
            if(currentWaypointCountNear >= numberOfWaypointBeforeGoNear)
            {
                currentWaypointCountNear = 0;
                currentIndex = GetNearestWaypointIndex();
            }

            waitTimer = 0;
            currentWaypoint = waypoints[currentIndex];
            creatureMoverScript.wantedPos = currentWaypoint.transform.position;
        }

        private int GetNearestWaypointIndex()
        {
            int index = 0;
            float bestDist = 100;

            for(int i = 0;i < waypoints.Count; i++)
            {
                float currentDist = Vector3.Distance(waypoints[i].transform.position, CharacterManager.Instance.transform.position);

                if (currentDist < bestDist)
                {
                    index = i;
                    bestDist = currentDist;
                }
            }

            return index;
        }

        #endregion


        #region OTHER BEHVAIORS

        private void ReachedPlaceToGo()
        {
            waitTimer += Time.deltaTime;
            
            if(waitTimer > suspicionWaitDuration)
            {
                StartCoroutine(StopLookLeftRight(2.5f));
                
                creatureMoverScript.forcedRot = Vector3.zero;
                didWaypointAction = false;
                mainScript.currentState = CreatureState.none;
                
                AudioManager.Instance.FadeOutAudioSource(2.5f, 1);

                creatureMoverScript.StartWalkSpeed();
            }
        }


        public void ChangeCurrentWaypointManager(WaypointsManager newWaypointManager, bool tp)
        {
            waypoints = newWaypointManager.waypoints;

            if (tp)
            {
                Vector3 moveDir = waypoints[0].transform.position - transform.position;
                transform.parent.transform.position += moveDir;
            }

            creatureMoverScript.tailIKScript.RebootTargets();

            waitTimer = 0;
            currentIndex = 0;

            RestartWaypointBehavior();
            NextWaypoint();
        }

        private void ResetCurrentWaypointManager()
        {
            creatureMoverScript.navMeshAgent.enabled = false;

            Vector3 moveDir = waypoints[0].transform.position - transform.position;
            transform.parent.transform.position += moveDir;

            creatureMoverScript.navMeshAgent.enabled = true;

            creatureMoverScript.tailIKScript.RebootTargets();

            waitTimer = 0;
            currentIndex = 0;
            
            creatureMoverScript.StartWalkSpeed();
            
            creatureMoverScript.forcedRot = Vector3.zero;
            didWaypointAction = false;
            mainScript.currentState = CreatureState.none;

            RestartWaypointBehavior();
        }

        #endregion


        /// <summary>
        /// Called when the creature is suspicious to setup her destination point
        /// </summary>
        public void ChangeDestinationSuspicious(Vector3 suspicousPlace)
        {
            NavMeshPath path = new NavMeshPath();
            bool esisteNavMesh = creatureMoverScript.navMeshAgent.CalculatePath(suspicousPlace, path);

            if (path.status == NavMeshPathStatus.PathComplete)
            {
                stoppedNormalBehavior = true;
                waitTimer = 0;

                creatureMoverScript.forcedRot = Vector3.zero;

                Vector3 dirToRemove = transform.position - suspicousPlace;

                placeToGo = suspicousPlace + (dirToRemove.normalized * suspicionPlaceOffsetMultiplier);
                creatureMoverScript.wantedPos = placeToGo;
            }

            else
            {
                RestartWaypointBehavior();
            }
        }


        Vector3 saveLastPlace;
        /// <summary>
        /// Called when the creature is suspicious to setup her destination point
        /// </summary>
        public void ChangeDestinationAggressive(Vector3 suspicousPlace)
        {
            if (isAttacking) return;

            stoppedNormalBehavior = true;
            waitTimer = 0;
            
            creatureMoverScript.forcedRot = Vector3.zero;

            placeToGo = suspicousPlace;
            creatureMoverScript.wantedPos = suspicousPlace;

            NavMeshPath path = new NavMeshPath();
            bool esisteNavMesh = creatureMoverScript.navMeshAgent.CalculatePath(suspicousPlace, path);

            if (path.status != NavMeshPathStatus.PathComplete)
            {
                creatureMoverScript.wantedPos = saveLastPlace + (-saveLastPlace + transform.position).normalized * 5;
            }
            else 
            {
                saveLastPlace = suspicousPlace;
            }
        }
        
        
        public IEnumerator StopLookLeftRight(float lookDuration)
        {
            if(isDoingSpecialAction)
                yield break;
            
            isDoingSpecialAction = true;
            
            placeToGo = transform.position;
            creatureMoverScript.wantedPos = transform.position;

            mainScript.specialMovesScript.LookLeftRight(lookDuration);
            
            yield return new WaitForSeconds(lookDuration * 1.2f);
            
            isDoingSpecialAction = false;
            
            RestartWaypointBehavior();
        }


        public void RestartWaypointBehavior()
        {
            stoppedNormalBehavior = false;           
            waitTimer = 0;
            currentWaypointCountNear = 0;

            mainScript.currentState = CreatureState.none;

            creatureMoverScript.forcedRot = Vector3.zero;
            creatureMoverScript.wantedPos = currentWaypoint.transform.position;
        }


        public void DoAttack(Vector3 creaturePos, Vector3 characterPos, float maxWallDist)
        {
            Vector3 wantedPos = creaturePos;

            /*Debug.DrawLine(creaturePos, creaturePos + (characterPos - creaturePos).normalized * maxWallDist, Color.magenta, 2);
            if (Physics.Raycast(creaturePos, (characterPos - creaturePos).normalized, out RaycastHit hit, maxWallDist,
                    LayerManager.Instance.defaultLayer))
            {
                wantedPos = hit.point - (characterPos - creaturePos).normalized * maxWallDist;
                Debug.Log(12);
            }*/

            creatureMoverScript.forcedRot = Vector3.zero;
            creatureMoverScript.wantedPos = wantedPos;

            isAttacking = true;
        }
    }
}
