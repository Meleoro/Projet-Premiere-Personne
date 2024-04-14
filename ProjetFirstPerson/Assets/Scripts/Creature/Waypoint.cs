using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [Header("Parameters")]
    public float waitTimer;
    public WaypointAction waypointAction;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.25f);
    }
}

public enum WaypointAction
{
    pourLeMomentYenAPas
}
