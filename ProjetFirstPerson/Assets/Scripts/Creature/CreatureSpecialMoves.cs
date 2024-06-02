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
        private Coroutine lookLeftRightCoroutine;

        [Header("References")]
        [SerializeField] private CreatureMover bodyScript;
        [SerializeField] private CreatureLegsMover legsScript;
        private BodyIK bodyIKScript;
        private HeadIK headIKScript;



        private void Start()
        {
            bodyIKScript = GetComponentInChildren<BodyIK>();
            headIKScript = GetComponentInChildren<HeadIK>();
        }

        private void Update()
        {
            VerifyHugeTurn();
        }


        public void CancelSpecialMoves()
        {
            if(lookLeftRightCoroutine != null)
            {
                StopCoroutine(lookLeftRightCoroutine);
                headIKScript.isLookingLeftRight = false;
            }
        }


        #region Huge Turn 

        private void VerifyHugeTurn()
        {
            if (isDoingHugeTurn || bodyScript.isRunning) return;

            Vector3 dir1 = bodyIKScript.target.position - bodyScript.transform.position;
            Vector3 dir2 = bodyIKScript.bodyJoint.position - bodyIKScript.backJoint.position;
            
            if (Vector3.Angle(dir1, dir2) > 150)
            {
                StartCoroutine(DoHugeTurnCoroutine(Vector3.Distance(bodyScript.wantedPos, bodyScript.transform.position) < 3));
            }
        }


        public IEnumerator DoHugeTurnCoroutine(bool surPlace)
        {
            if(surPlace) yield break;

            float timer = 0;
            
            isDoingHugeTurn = true;
            Vector3 originalPos = legsScript.transform.position;
            legsScript.maxMovingLegsAmountWalk = 1;

            bodyScript.forcedPos = originalPos;
            if (surPlace)
            {
                while (timer < 2.5f)
                {
                    timer += Time.deltaTime;
                    bodyScript.forcedPos = legsScript.mainTrRotRefFront.TransformPoint(new Vector3(-0.5f, 0, 0.5f));

                    Debug.DrawLine(bodyScript.forcedPos, transform.position, Color.green, 1);

                    yield return null;
                }
            }

            else
            {
                while (timer < 2.5f)
                {
                    timer += Time.deltaTime;
                    bodyScript.forcedPos = legsScript.mainTrRotRefFront.TransformPoint(new Vector3(-0.5f, 0, 0.5f));

                    Debug.DrawLine(bodyScript.forcedPos, transform.position, Color.green, 1);

                    yield return null;
                }
            }

            legsScript.maxMovingLegsAmountWalk = 1;
            bodyScript.forcedPos = Vector3.zero;
            
            isDoingHugeTurn = false;
        }
        
        #endregion


        #region Look Left Right

        public void LookLeftRight(float duration)
        {
            lookLeftRightCoroutine = StartCoroutine(headIKScript.LookLeftThenRight(duration));
        }

        #endregion
    }
}
