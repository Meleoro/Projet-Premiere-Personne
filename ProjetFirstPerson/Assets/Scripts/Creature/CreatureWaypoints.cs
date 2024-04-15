using System.Collections;
using System.Collections.Generic;
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
        private float currentAnger;
        private Waypoint currentWaypoint;
        private int currentIndex;
        private float waitTimer;
        private Vector3 placeToGo;

        [Header("References")]
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

            if(waitTimer > currentWaypoint.waitTimer)
            {
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
                RestartWaypointBehavior();

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

            placeToGo = suspicousPlace;
            creatureMoverScript.wantedPos = suspicousPlace;
        }


        public void RestartWaypointBehavior()
        {
            stoppedNormalBehavior = false;
            waitTimer = 0;

            mainScript.currentSuspicion = 0;

            creatureMoverScript.wantedPos = currentWaypoint.transform.position;
        }
    }
}
