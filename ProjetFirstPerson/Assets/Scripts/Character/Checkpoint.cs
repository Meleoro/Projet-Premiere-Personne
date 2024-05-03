using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private Color gizmosColor;
    [SerializeField] private bool showGizmosOnlyOnSelected;



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterManager.Instance.GetComponent<HealthComponent>().SavePos();
            gameObject.SetActive(false);
        }
    }


    private void OnDrawGizmos()
    {
        if (!showGizmosOnlyOnSelected)
        {
            Gizmos.color = gizmosColor;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

            Gizmos.DrawCube(Vector3.zero, transform.localScale);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (showGizmosOnlyOnSelected)
        {
            Gizmos.color = gizmosColor;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

            Gizmos.DrawCube(Vector3.zero, transform.localScale);
        }
    }
}
