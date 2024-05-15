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
        [SerializeField] private int numberOfWaypointBeforeGoNear;
        
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


        public void ComponentUpdate()
        {
            float currentDist = Vector2.Distance(new Vector2(transform.position.x, transform.position.z),
                new Vector2(currentWaypoint.transform.position.x, currentWaypoint.transform.position.z));
            
            if (!stoppedNormalBehavior)
            {
                if(currentDist < 2f)
                {
                    ReachedWaypoint();
                }
            }

            else
            {
                currentDist = Vector2.Distance(new Vector2(transform.position.x, transform.position.z),
                    new Vector2(placeToGo.x, placeToGo.z));
                
                if (currentDist < 1f)
                {
                    ReachedPlaceToGo();
                }
            }
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

                creatureMoverScript.StartWalkSpeed();
            }
        }


        public void ChangeCurrentWaypointManager(WaypointsManager newWaypointManager)
        {
            waypoints = newWaypointManager.waypoints;
            transform.position = waypoints[0].transform.position;

            waitTimer = 0;
            currentIndex = 0;

            NextWaypoint();
        }

        private void ResetCurrentWaypointManager()
        {
            transform.position = waypoints[0].transform.position;
            
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
                creatureMoverScript.wantedPos = saveLastPlace;
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


        public void DoAttack(Vector3 creaturePos, Vector3 characterPos)
        {
            Vector3 wantedPos = creaturePos + (characterPos - creaturePos).normalized * 5f;

            creatureMoverScript.forcedRot = Vector3.zero;
            creatureMoverScript.wantedPos = wantedPos;

            isAttacking = true;
        }
    }
}
