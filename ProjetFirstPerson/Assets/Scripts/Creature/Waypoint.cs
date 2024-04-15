using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [Header("Parameters")]
    public float waitTimer;
    public float roationAngleY;
    public WaypointAction waypointAction;

    
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.25f);
        
        Gizmos.DrawLine(transform.position, transform.position + Vector3.forward.RotateDirection(roationAngleY, transform.up));
    }
}

public enum WaypointAction
{
    PourLeMomentYenAPas
}
