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
}
