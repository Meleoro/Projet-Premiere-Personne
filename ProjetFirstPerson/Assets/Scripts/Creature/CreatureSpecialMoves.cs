using IK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creature
{
    public class CreatureSpecialMoves : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private float hugeTurnDurationMultiplicator;

        [Header("Private Infos")]
        private bool isDoingHugeTurn;

        [Header("References")]
        [SerializeField] private CreatureLegsMover legsScript;
        private BodyIK bodyIKScript;



        private void Start()
        {
            bodyIKScript = GetComponentInChildren<BodyIK>();
        }

        private void Update()
        {
            VerifyHugeTurn();
        }


        #region Huge Turn 

        private void VerifyHugeTurn()
        {
            if (isDoingHugeTurn) return;

            if (Mathf.Abs(bodyIKScript.currentAtanDif) > 65)
            {
                StartCoroutine(DoHugeTurnCoroutine());
            }
        }


        private IEnumerator DoHugeTurnCoroutine()
        {
            isDoingHugeTurn = true;

            yield return new WaitForSeconds(0.05f);

            Debug.Log(Mathf.Abs(bodyIKScript.currentAtanDif));

            float turnAmplitude = Mathf.Abs(bodyIKScript.currentAtanDif);
            float turnDuration = hugeTurnDurationMultiplicator * turnAmplitude * 0.1f;

            for (int i = 0; i < legsScript.legs.Count; i++) 
            {
                if (legsScript.legs[i].isFrontLeg)
                {
                    StartCoroutine(legsScript.MoveLeg(legsScript.legs[i], CalculateHugeTurnLegFinalPos(legsScript.legs[i]), turnDuration, 2.5f));
                }
            }

            yield return new WaitForSeconds(turnDuration);

            isDoingHugeTurn = false;
        }

        private Vector3 CalculateHugeTurnLegFinalPos(Leg currentLeg)
        {
            Vector3 calculatedPos;

            Vector3 localLegTargetPos = bodyIKScript.backJoint.InverseTransformPoint(currentLeg.target.position);
            Quaternion saveBackJointRot = bodyIKScript.backJoint.rotation;

            Vector3 eulerBack = bodyIKScript.backJoint.localEulerAngles;
            eulerBack.y = bodyIKScript.saveOffset2.y + bodyIKScript.currentAtanDif;
            bodyIKScript.backJoint.localEulerAngles = eulerBack;

            calculatedPos = bodyIKScript.backJoint.TransformPoint(localLegTargetPos);
            bodyIKScript.backJoint.rotation = saveBackJointRot;

            return calculatedPos;
        }

        #endregion
    }
}
