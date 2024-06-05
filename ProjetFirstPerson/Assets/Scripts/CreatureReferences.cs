using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureReferences : MonoBehaviour
{
    [Header("Front Leg1 References")]
    public Transform frontLeg1Bone1;
    public Transform frontLeg1Bone2;
    public Transform frontLeg1Foot;

    [Header("Front Leg2 References")]
    public Transform frontLeg2Bone1;
    public Transform frontLeg2Bone2;
    public Transform frontLeg2Foot;

    [Header("Back Leg2 References")]
    public Transform backLeg1Bone1;
    public Transform backLeg1Bone2;
    public Transform backLeg1Bone3;
    public Transform backLeg1Foot;

    [Header("Back Leg2 References")]
    public Transform backLeg2Bone1;
    public Transform backLeg2Bone2;
    public Transform backLeg2Bone3;
    public Transform backLeg2Foot;

    [Header("Body References")]
    public List<Transform> spineBones;
    public Transform pantherPelvis;
    public Transform pantherRibCage;

    [Header("Head References")]
    public List<Transform> neckBones;
    public Animator coleretteAnimator;

    [Header("Tail References")]
    public List<Transform> tailBones;
}
