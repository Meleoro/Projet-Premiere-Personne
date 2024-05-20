using UnityEngine;

namespace Creature
{
    public class CreatureTrigger : MonoBehaviour
    {
        [Header("Parameters")] 
        [SerializeField] private CreatureWaypoints creatureToMove;
        [SerializeField] private Transform placeToTP;
        [SerializeField] private WaypointsManager newWaypointsManager;
        [SerializeField] private bool turnsTheCreatureAggressive;

        [Header("Parameters Gizmos")] 
        [SerializeField] private bool showGizmosOnlyOnSelected;
        [SerializeField] private Color gizmosColor;
    
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                ActivateTrigger();
                gameObject.SetActive(false);
            }
        }


        private void ActivateTrigger()
        {
            if (newWaypointsManager != null)
            {
                creatureToMove.ChangeCurrentWaypointManager(newWaypointsManager);
            }

            if (placeToTP != null)
            {
                creatureToMove.transform.GetComponent<CreatureMover>().navMeshAgent.enabled = false;
                Vector3 moveDir = placeToTP.position - creatureToMove.transform.position;
                creatureToMove.transform.parent.transform.position += moveDir;

                creatureToMove.transform.GetComponent<CreatureMover>().navMeshAgent.enabled = true;
            }

            if (turnsTheCreatureAggressive)
            {
                creatureToMove.GetComponent<CreatureManager>().TurnAggressive();
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
}
