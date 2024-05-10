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

    [Header("Rotation")]
    public float rotationSpeed;
    public float aggressiveRotationSpeed;
    public float rotationSpeedFrontJoints;
    public float aggressiveRotationSpeedFrontJoints;
    public float maxRotDifFrontBack;

    [Header("Others")] 
    [Range(0f, 1f)] public float legCantMoveSpeedMultiplier;
}
