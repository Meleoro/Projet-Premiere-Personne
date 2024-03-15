using UnityEngine;
using UnityEngine.AI;

namespace Creature
{
    public class CreatureMover : MonoBehaviour, ICreatureComponent
    {
        [Header("Parameters")]
        
        [Header("Debug Parameters")] 
        [SerializeField] private bool doDebugMovement;
        [SerializeField] private Transform debugWantedPos;

        [Header("Private Infos")] 
        private float timerMovement;
        
        [Header("References")] 
        [SerializeField] private NavMeshAgent navMeshAgent;
    
        
        public void ComponentUpdate()
        {
            SetNextPos();
        }


        private void SetNextPos()
        {
            if (doDebugMovement)
            {
                navMeshAgent.SetDestination(debugWantedPos.position);
            }
        }
    }
}
