using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creature
{
    public class CreatureWaypoints : MonoBehaviour
    {
        [Header("Public Infos")]
        public List<Waypoint> waypoints = new List<Waypoint>();

        [Header("Private Infos")]
        private bool stoppedNormalBehavior;







        public void StopWaypointBehavior()
        {
            stoppedNormalBehavior = true;
        }

        public void RestartWaypointBehavior()
        {
            stoppedNormalBehavior = false;
        }
    }
}
