using System.Collections;
using System.Collections.Generic;
using IK;
using UnityEngine;

namespace Creature
{
    [RequireComponent(typeof(CreatureMover))]
    public class CreatureWaypoints : MonoBehaviour, ICreatureComponent
    {
        [Header("Parameters")]
        [SerializeField] private List<WaypointsManager> possiblePaths = new List<WaypointsManager>();
        [SerializeField] private float angerPerCycle;
        [SerializeField] private float neededPfDist;
        [SerializeField] private float suspicionWaitDuration;

        [Header("Public Infos")]
        public List<Waypoint> waypoints = new List<Waypoint>();

        [Header("Private Infos")]
        private bool stoppedNormalBehavior;
        private bool didWaypointAction;
        private bool isDoingSpecialAction;
        private float currentAnger;
        private Waypoint currentWaypoint;
        private int currentIndex;
        private float waitTimer;
        private Vector3 placeToGo;

        [Header("References")] 
        [SerializeField] private HeadIK headIKScript;
        private CreatureMover creatureMoverScript;
        private CreatureManager mainScript;



        private void Start()
        {
            if(possiblePaths.Count > 0) 
                waypoints = possiblePaths[0].waypoints;

            creatureMoverScript = GetComponent<CreatureMover>();
            mainScript = GetComponent<CreatureManager>();

            creatureMoverScript.wantedPos = waypoints[0].transform.position;
            currentIndex = 0;
            currentWaypoint = waypoints[0];
        }


        public void ComponentUpdate()
        {
            if (!stoppedNormalBehavior)
            {
                if(Vector3.Distance(transform.position, currentWaypoint.transform.position) < 3f)
                {
                    ReachedWaypoint();
                }
            }

            else
            {
                if (Vector3.Distance(transform.position, placeToGo) < 3f)
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
                        StartCoroutine(headIKScript.LookLeftThenRight(currentWaypoint.timeWaypointAction));
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

            waitTimer = 0;
            currentWaypoint = waypoints[currentIndex];
            creatureMoverScript.wantedPos = currentWaypoint.transform.position;
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

        #endregion


        /// <summary>
        /// Called when the creature is suspicious to setup her destination point
        /// </summary>
        public void ChangeDestinationSuspicious(Vector3 suspicousPlace)
        {
            stoppedNormalBehavior = true;
            waitTimer = 0;

            creatureMoverScript.forcedRot = Vector3.zero;

            placeToGo = suspicousPlace;
            creatureMoverScript.wantedPos = suspicousPlace;
        }


        /// <summary>
        /// Called when the creature is suspicious to setup her destination point
        /// </summary>
        public void ChangeDestinationAggressive(Vector3 suspicousPlace)
        {
            stoppedNormalBehavior = true;
            waitTimer = 0;
            
            creatureMoverScript.forcedRot = Vector3.zero;

            placeToGo = suspicousPlace;
            creatureMoverScript.wantedPos = suspicousPlace;
        }
        
        
        public IEnumerator StopLookLeftRight(float lookDuration)
        {
            if(isDoingSpecialAction)
                yield break;
            
            isDoingSpecialAction = true;
            
            placeToGo = transform.position;
            creatureMoverScript.wantedPos = transform.position;

            StartCoroutine(headIKScript.LookLeftThenRight(lookDuration));
            
            yield return new WaitForSeconds(lookDuration * 1.2f);
            
            isDoingSpecialAction = false;
            
            RestartWaypointBehavior();
        }


        public void RestartWaypointBehavior()
        {
            stoppedNormalBehavior = false;
            waitTimer = 0;

            mainScript.currentSuspicion = 0;
            mainScript.currentState = CreatureState.none;

            creatureMoverScript.forcedRot = Vector3.zero;
            creatureMoverScript.wantedPos = currentWaypoint.transform.position;
        }
    }
}
