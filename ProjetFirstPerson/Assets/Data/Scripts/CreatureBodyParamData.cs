using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CreatureData/BodyParamData")]
public class CreatureBodyParamData : ScriptableObject
{
    [Header("Body Height")]
    public float maxHeight;
    public float wantedHeight;
    public AnimationCurve heightModifierCurveBySpeed;
    public float frontWantedHeight;
    public float backWantedHeight;

    [Header("Rotation")]
    public float rotationSpeed;
    public float aggressiveRotationSpeed;
    public float rotationSpeedFrontJoints;
    public float aggressiveRotationSpeedFrontJoints;
    public float maxRotDifFrontBack;

    [Header("Legs Effects")]
    public AnimationCurve legsHeightImpactAccordingToSpeed;

    [Header("Others")] 
    [Range(0f, 1f)] public float legCantMoveSpeedMultiplier;
}
