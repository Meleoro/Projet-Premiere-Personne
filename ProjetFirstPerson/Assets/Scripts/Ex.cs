using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Ex 
{
    /// <summary>
    /// RETURN THE VECTOR 2 FROM THE DIRECTION OF A GIVEN ANGLE
    /// </summary>
    public static Vector2 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }



    /// <summary>
    /// RETURN THE ANGLE FROM A GIVEN DIRECTION
    /// </summary>
    public static float GetAngleFromVector(this Vector2 dir)
    {
        dir = dir.normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle < 0)
            angle += 360;

        return angle;
    }


    /// <summary>
    /// ROTATES A VECTOR FROM A CERTAIN ANGLE
    /// </summary>
    public static Vector2 RotateDirection(this Vector2 originalDirection, float addedAngle)
    {
        float currentAngle = GetAngleFromVector(originalDirection);
 
        currentAngle += addedAngle;

        return GetVectorFromAngle(currentAngle);
    }
    
    
    /// <summary>
    /// ROTATES A VECTOR FROM A CERTAIN ANGLE
    /// </summary>
    public static Vector3 RotateDirection(this Vector3 originalDirection, float addedAngle, Vector3 axis)
    {
        //Quaternion.
        
        
        float currentAngle = 0;
        if(axis == Vector3.up)
            currentAngle = GetAngleFromVector(new Vector2(originalDirection.x, originalDirection.z));
        
        else if(axis == Vector3.right)
            currentAngle = GetAngleFromVector(new Vector2(originalDirection.z, originalDirection.y));
        
        else
            currentAngle = GetAngleFromVector(new Vector2(originalDirection.x, originalDirection.y));
        

        currentAngle += addedAngle;
        
        if(axis == Vector3.up) 
            return new Vector3(GetVectorFromAngle(currentAngle).x, originalDirection.y, GetVectorFromAngle(currentAngle).y);
        
        if(axis == Vector3.right)
            return new Vector3(originalDirection.x, GetVectorFromAngle(currentAngle).y, GetVectorFromAngle(currentAngle).x);
        
        return new Vector3(GetVectorFromAngle(currentAngle).x, GetVectorFromAngle(currentAngle).y, originalDirection.z);
    }
}
