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

        [Header("Public Infos")]
        public List<Waypoint> waypoints = new List<Waypoint>();

        [Header("Private Infos")]
        private bool stoppedNormalBehavior;
        private float currentAnger;
        private Waypoint currentWaypoint;
        private int currentIndex;
        private float waitTimer;

        [Header("References")]
        private CreatureMover creatureMoverScript;




        private void Start()
        {
            waypoints = possiblePaths[0].waypoints;

            creatureMoverScript = GetComponent<CreatureMover>();
            creatureMoverScript.wantedPos = waypoints[0].transform;
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
        }


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
            creatureMoverScript.wantedPos = currentWaypoint.transform;
        }



        public void StopWaypointBehavior()
        {
            stoppedNormalBehavior = true;
            waitTimer = 0;
        }

        public void RestartWaypointBehavior()
        {
            stoppedNormalBehavior = false;
        }
    }
}
