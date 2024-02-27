using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TestLeg : MonoBehaviour
{
    [Header("Joints")]
    public Transform joint0;
    public Transform joint1;

    [Header("Private Infos")]
    private float length1;
    private float length2;

    [Header("Public Infos")]
    public Vector3 originalOffset;
    public bool isMoving;

    [Header("References")]
    [SerializeField] private Transform baseTransform;
    [SerializeField] private Transform effectorTransform;
    [SerializeField] private Transform targetTransform;
    private Transform bodyTransform;


    private void Start()
    {
        length1 = Vector3.Distance(joint0.position, joint1.position);
        length2 = Vector3.Distance(joint1.position, effectorTransform.position);
    }


    private void Update()
    {
        ApplyIK2();
    }


    public void SetupLeg(Transform bodyTr)
    {
        bodyTransform = bodyTr;

        originalOffset = targetTransform.position - bodyTr.position;
    }


    private void ApplyIK1()
    {
        Vector3 targetPos = targetTransform.position;

        Transform currentJoint = effectorTransform;

        while (currentJoint != baseTransform)
        {
            currentJoint = currentJoint.parent;

            Vector3 dirToTarget = (targetPos - currentJoint.position).normalized; 
            Quaternion targetRotation = Quaternion.FromToRotation((effectorTransform.position - currentJoint.position).normalized, dirToTarget);

            currentJoint.rotation = targetRotation * currentJoint.rotation; 
        }
    }


    private void ApplyIK2()
    {
        float length3 = Vector3.Distance(targetTransform.position, joint0.position);

        // Angle Alpha
        float cosAngleAlpha = ((length3 * length3) + (length2 * length2) - (length1 * length1) / (2 * length1 * length3));
        float angleAlpha = Mathf.Acos(cosAngleAlpha) * Mathf.Rad2Deg;

        // Angle Beta
        float cosAngleBeta = ((length1 * length1) + (length2 * length2) - (length3 * length3) / (2 * length1 * length2));
        float angleBeta = Mathf.Acos(cosAngleBeta) * Mathf.Rad2Deg;

        // Angle between joint1 and target
        Vector2 diff = targetTransform.position - joint0.position;
        float angleAtan = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;


        float jointAngle0 = angleAtan - angleAlpha;    // Angle A
        float jointAngle1 = 180f - angleBeta;    // Angle B


        Vector3 euler0 = joint0.transform.eulerAngles;
        euler0.z = jointAngle0 + 90;
        joint0.transform.eulerAngles = euler0;

        Vector3 euler1 = joint1.transform.eulerAngles;
        euler1.z = jointAngle1 + 90;
        joint1.transform.eulerAngles = euler1;
    }

    public IEnumerator MoveLegCoroutine(Vector3 endPos, float moveDuration)
    {
        isMoving = true;

        Vector3 originalPos = targetTransform.position;
        float timer = 0;

        RaycastHit hit;
        float currentWantedY = 0;

        while(moveDuration > timer)
        {
            if (Physics.Raycast(targetTransform.position, -bodyTransform.forward, out hit, 100, LayerMask.NameToLayer("Ground")))
            {
                currentWantedY = bodyTransform.InverseTransformPoint(hit.point).z;
            }

            timer += Time.deltaTime;

            targetTransform.position = Vector3.Lerp(originalPos, endPos, timer / moveDuration);

            if( timer / moveDuration < 0.5f)
                targetTransform.position += bodyTransform.forward * Mathf.Lerp(currentWantedY, currentWantedY + 0.4f, 
                    timer * 2 / moveDuration);

            else
                targetTransform.position += bodyTransform.forward * Mathf.Lerp(currentWantedY, currentWantedY + 0.4f, 
                    0.5f - timer / moveDuration);

            yield return new WaitForSeconds(Time.deltaTime);
        }

        isMoving = false;
    }
}
