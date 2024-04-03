using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;


namespace Creature
{
    public class CreatureManager : MonoBehaviour
    {
        [Header("AI Parameters")] 
        [SerializeField] private float earLoudRadius;
        [SerializeField] private float earNormalRadius;
        [SerializeField] private float visionRange;
        [SerializeField] private float visionRadius;

        [Header("Public Infos")] 
        public bool heardSomething;
        [HideInInspector] public Vector3 heardLocation;

        [Header("References")] 
        [SerializeField] private Transform mainRotationJoint;
        private List<ICreatureComponent> creatureComponents = new List<ICreatureComponent>();

        
        private void Start()
        {
            creatureComponents = GetComponents<ICreatureComponent>().ToList();
        }


        private void Update()
        {
            DoEarAI();
            
            for (int i = 0; i < creatureComponents.Count; i++)
            {
                creatureComponents[i].ComponentUpdate();
            }
        }


        private void DoEarAI()
        {
            if (heardSomething)
                return;
            
            
            if (Vector3.Distance(mainRotationJoint.position, CharacterManager.Instance.transform.position) < earLoudRadius)
            {
                Debug.Log(12);
                
                if (CharacterManager.Instance.currentNoiseType == NoiseType.Loud)
                {
                    heardSomething = true;
                    heardLocation = CharacterManager.Instance.transform.position;
                }
                
                else if (Vector3.Distance(mainRotationJoint.position, CharacterManager.Instance.transform.position) < earNormalRadius)
                {
                    if (CharacterManager.Instance.currentNoiseType == NoiseType.Normal)
                    {
                        heardSomething = true;
                        heardLocation = CharacterManager.Instance.transform.position;
                    }
                }
            }
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(mainRotationJoint.position, earLoudRadius);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(mainRotationJoint.position, earNormalRadius);
            
            Gizmos.color = Color.white;
            Gizmos.matrix = Matrix4x4.TRS(mainRotationJoint.position, mainRotationJoint.rotation * Quaternion.Euler(0, -90, 0), Vector3.one);
            Gizmos.DrawFrustum(Vector3.zero, visionRadius, visionRange, 0, 1);
        }
    }
}
