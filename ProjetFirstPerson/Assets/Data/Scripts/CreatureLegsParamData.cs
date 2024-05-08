using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CreatureData/LegsParamData")]
public class CreatureLegsParamData : ScriptableObject
{
    [Header("Front Legs Movement Trigger")]
    public float maxFrontLegDistWalk;
    public float maxFrontLegDistRun;

    [Header("Front Legs Movement")]
    public float frontLegMoveDuration;
    public AnimationCurve frontLegMovementYCurve;
    public AnimationCurve frontLegYModifierBySpeed;
    public AnimationCurve frontLegYModifierByRot;
    public AnimationCurve frontLegDurationModifierBySpeed;
    public AnimationCurve frontLegDurationModifierByRot;

    [Header("Back Legs Movement Trigger")]
    public float maxBackLegDistWalk;
    public float maxBackLegDistRun;

    [Header("Back Legs Movement")]
    public float backLegMoveDuration;
    public AnimationCurve backLegMovementYCurve;
    public AnimationCurve backLegYModifierBySpeed;
    public AnimationCurve backLegYModifierByRot;
    public AnimationCurve backLegDurationModifierBySpeed;
    public AnimationCurve backLegDurationModifierByRot;
    
    [Header("Patounes")] 
    public AnimationCurve frontPatouneRot;
    public float frontPatouneRotMultiplier;
    public AnimationCurve backPatouneRot;
    public float backPatouneRotMultiplier;

    [Header("Others")] 
    public float frontLegsOffset;
    public float backLegsOffset;
}
