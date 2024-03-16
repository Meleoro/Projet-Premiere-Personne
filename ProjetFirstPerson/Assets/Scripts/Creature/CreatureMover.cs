using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Creature
{
    public class CreatureMover : MonoBehaviour, ICreatureComponent
    {
        [Header("Parameters")] 
        [SerializeField] private float rotateSpeed;
        
        [Header("Debug Parameters")] 
        [SerializeField] private bool doDebugMovement;
        [SerializeField] private Transform debugWantedPos;

        [Header("Private Infos")] 
        private float timerMovement;

        [Header("References")] 
        [SerializeField] private Transform targetIKBody;
        private NavMeshAgent navMeshAgent;
        private Rigidbody rb;


        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            rb = GetComponent<Rigidbody>();
        }


        public void ComponentUpdate()
        {
            SetNextPos();
            
            ManageRotation();
        }


        private void SetNextPos()
        {
            if (doDebugMovement)
            {
                navMeshAgent.SetDestination(debugWantedPos.position);
            }
        }

        private void ManageRotation()
        {
            Vector3 dirToRotateTo = navMeshAgent.velocity;
            Vector3 currentDir = targetIKBody.position - transform.position;
            currentDir = currentDir.normalized * 4;

            currentDir = Vector3.RotateTowards(currentDir, dirToRotateTo, Time.deltaTime * rotateSpeed, Time.deltaTime * rotateSpeed);
            
            targetIKBody.position = transform.position + currentDir;
        }
    }
}
