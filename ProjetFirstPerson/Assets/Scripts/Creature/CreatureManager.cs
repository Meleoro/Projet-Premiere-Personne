using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Creature
{
    public class CreatureManager : MonoBehaviour
    {
        [Header("AI Parameters")] 
        [SerializeField] private float earRadius;
        [SerializeField] private float visionRange;
        [SerializeField] private float visionRadius;

        [Header("References")] 
        [SerializeField] private Transform mainRotationJoint;
        private List<ICreatureComponent> creatureComponents = new List<ICreatureComponent>();

        
        private void Start()
        {
            creatureComponents = GetComponents<ICreatureComponent>().ToList();
        }


        private void Update()
        {
            for (int i = 0; i < creatureComponents.Count; i++)
            {
                creatureComponents[i].ComponentUpdate();
            }
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(mainRotationJoint.position, earRadius);
            
            Gizmos.matrix = Matrix4x4.TRS(mainRotationJoint.position, mainRotationJoint.rotation * Quaternion.Euler(0, -90, 0), Vector3.one);
            Gizmos.DrawFrustum(Vector3.zero, visionRadius, visionRange, 0, 1);
        }
    }
}
