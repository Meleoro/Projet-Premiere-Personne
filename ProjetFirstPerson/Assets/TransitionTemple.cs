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
    [SerializeField] private Volume outVolume;
    
    [Header("Inside Parameters")] 
    [SerializeField][ColorUsage(true, true)]  private Color inAmbientEquatorColor;
    [SerializeField][ColorUsage(true, true)]  private Color inAmbientGroundColor;
    [SerializeField] private Color inFogColor;
    [SerializeField] private Volume inVolume;
    


    private void OnTriggerStay(Collider other)
    {
        float difZ = transform.InverseTransformPoint(CharacterManager.Instance.transform.position).z;
        float t = difZ + 0.5f;
        
        RenderSettings.ambientEquatorColor = Color.Lerp(outAmbientEquatorColor, inAmbientEquatorColor, t);
        RenderSettings.ambientGroundColor = Color.Lerp(outAmbientGroundColor, inAmbientGroundColor, t);
        RenderSettings.fogColor = Color.Lerp(outFogColor, inFogColor, t);

        if (outVolume != null) outVolume.weight = Mathf.Clamp(1 - t, 0 ,1);
        if (inVolume != null) inVolume.weight = Mathf.Clamp(t, 0 ,1);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = gizmosColor;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

        Gizmos.DrawCube(Vector3.zero, transform.localScale);
    }
    
}
