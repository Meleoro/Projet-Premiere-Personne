using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TextCore.Text;

public class TransitionTemple : MonoBehaviour
{
    [Header("Gizmos Parameters")] 
    [SerializeField] private Color gizmosColor;

    [Header("Outside Parameters")] 
    [SerializeField][ColorUsage(true, true)] private Color outAmbientEquatorColor;
    [SerializeField][ColorUsage(true, true)]  private Color outAmbientGroundColor;
    [SerializeField] private Color outFogColor;
    
    [Header("Inside Parameters")] 
    [SerializeField][ColorUsage(true, true)]  private Color inAmbientEquatorColor;
    [SerializeField][ColorUsage(true, true)]  private Color inAmbientGroundColor;
    [SerializeField] private Color inFogColor;
    


    private void OnTriggerStay(Collider other)
    {
        float difZ = transform.InverseTransformPoint(CharacterManager.Instance.transform.position).z;
        float t = (difZ + (transform.localScale.z * 0.5f)) / transform.localScale.z;

        RenderSettings.ambientEquatorColor = Color.Lerp(outAmbientEquatorColor, inAmbientGroundColor, t);
        RenderSettings.ambientGroundColor = Color.Lerp(outAmbientGroundColor, inAmbientGroundColor, t);
        RenderSettings.fogColor = Color.Lerp(outFogColor, inFogColor, t);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = gizmosColor;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

        Gizmos.DrawCube(Vector3.zero, transform.localScale);
    }
    
}
