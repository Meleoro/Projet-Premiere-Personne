using Creature;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class WaypointsManager : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private bool autoFillList;      // If true, the list of waypoint will automatically fill (first waypoint = the one to the top and the last is the one at the bottom in the children of this object)
    public List<Waypoint> waypoints = new List<Waypoint>();

    
    void Awake()
    {
        if (autoFillList)
        {
            waypoints = GetComponentsInChildren<Waypoint>().ToList();
        }
    }


    private void OnDrawGizmos()
    {
        for(int i = 1; i < waypoints.Count; i++)
        {
            Gizmos.DrawLine(waypoints[i - 1].transform.position, waypoints[i].transform.position);
        }

        if(waypoints.Count > 2)
        {
            Gizmos.DrawLine(waypoints[0].transform.position, waypoints[waypoints.Count - 1].transform.position);
        }
    }
}
