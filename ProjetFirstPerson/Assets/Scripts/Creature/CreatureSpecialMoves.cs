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
                StartCoroutine(DoHugeTurnCoroutineFar(Vector3.Distance(bodyScript.wantedPos, bodyScript.transform.position) < 3));
            }
        }


        public IEnumerator DoHugeTurnCoroutineFar(bool surPlace)
        {
            float timer = 0;
            float dist = 2f;
            float duration = 0.9f;
            
            isDoingHugeTurn = true;
            Vector3 originalPos = legsScript.transform.position;
            legsScript.maxMovingLegsAmountWalk = 1;

            Vector3 p1 = legsScript.mainTrRotRefBack.TransformPoint(new Vector3(-0.7f, 0, 0.9f) * dist);
            Vector3 p2 = legsScript.mainTrRotRefBack.TransformPoint(new Vector3(-1f, 0, 0.3f) * dist);
            Vector3 p3 = legsScript.mainTrRotRefBack.TransformPoint(new Vector3(-0.6f, 0, 0f) * dist);

            if (surPlace)
            {
                dist = dist * 0.75f;
                duration = duration * 1f;
                
                p1 = legsScript.mainTrRotRefBack.TransformPoint(new Vector3(-0.25f, 0, 0.75f) * dist);
                p2 = legsScript.mainTrRotRefBack.TransformPoint(new Vector3(0.25f, 0, 0.5f) * dist);
                p3 = legsScript.mainTrRotRefBack.TransformPoint(new Vector3(0f, 0, 0f) * dist);
            }
            
            Debug.DrawLine(originalPos, p1, Color.green, 1);
            Debug.DrawLine(p1, p2, Color.green, 1);
            Debug.DrawLine(p2, p3, Color.green, 1);

            bodyScript.forcedPos = originalPos;
            
            while (timer < duration)
            {
                timer += Time.deltaTime;
                bodyScript.forcedPos = Vector3.Lerp(originalPos, p1, timer);
                
                yield return new WaitForSeconds(Time.deltaTime);
            }
            
            timer = 0;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                bodyScript.forcedPos = Vector3.Lerp(p1, p2, timer);
                
                yield return new WaitForSeconds(Time.deltaTime);
            }
            
            timer = 0;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                bodyScript.forcedPos = Vector3.Lerp(p2, p3, timer);
                
                yield return new WaitForSeconds(Time.deltaTime);
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
