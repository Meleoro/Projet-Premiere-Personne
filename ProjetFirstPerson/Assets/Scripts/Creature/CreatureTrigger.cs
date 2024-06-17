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
        [SerializeField] private bool changeWaypointTillChase;
        [SerializeField] private int newWaypointTillChase;
        [SerializeField] private bool changeWalkSpeed;
        [SerializeField] private float newWalkSpeed;
        [SerializeField] private bool isFinalPoursuite;
        
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
                if(placeToTP != null)
                    creatureToMove.ChangeCurrentWaypointManager(newWaypointsManager, true);

                else
                    creatureToMove.ChangeCurrentWaypointManager(newWaypointsManager, false);
            }

            if (placeToTP != null)
            {
                creatureToMove.transform.GetComponent<CreatureMover>().navMeshAgent.enabled = false;
                Vector3 moveDir = placeToTP.position - creatureToMove.transform.position;
                creatureToMove.transform.parent.transform.position += moveDir;

                creatureToMove.transform.GetComponent<CreatureMover>().tailIKScript.RebootTargets();
                creatureToMove.GetComponent<CreatureManager>().currentState = CreatureState.none;
                creatureToMove.GetComponent<CreatureManager>().currentSuspicion = 0;

                creatureToMove.transform.GetComponent<CreatureMover>().navMeshAgent.enabled = true;
            }

            if (turnsTheCreatureAggressive)
            {
                AudioManager.Instance.PlaySoundContinuous(0, 1, 1);
                creatureToMove.GetComponent<CreatureManager>().TurnAggressive();
            }

            if (changeWaypointTillChase)
            {
                creatureToMove.GetComponent<CreatureWaypoints>().numberOfWaypointBeforeGoNear = newWaypointTillChase;
            }

            if (changeWalkSpeed)
            {
                creatureToMove.GetComponent<CreatureMover>().walkSpeed = newWalkSpeed;
            }

            if (isFinalPoursuite)
            {
                StartCoroutine(AmbianceManager.Instance.StartEndPoursuite());
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
