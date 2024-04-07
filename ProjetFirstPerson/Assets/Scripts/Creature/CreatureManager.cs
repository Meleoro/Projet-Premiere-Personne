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
        [SerializeField] [Range(0, 60)] private int visionRadiusX;
        [SerializeField] [Range(0, 60)] private int visionRadiusY;

        [Header("Public Infos")] 
        public bool heardSomething;
        [HideInInspector] public Vector3 heardLocation;
        public bool seenSomething;
        [HideInInspector] public Vector3 seenLocation;

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
            DoViewAI();
            
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

        private void DoViewAI()
        {
            bool playerInView = false;
            Vector3 currentDir = -mainRotationJoint.right;
            currentDir = Quaternion.Euler(-visionRadiusX * 0.5f, -visionRadiusY * 0.5f, 0) * currentDir;
            
            for (int x = 0; x < visionRadiusX; x+=4)
            {
                for (int y = 0; y < visionRadiusX; y+=4)
                {
                    //Debug.DrawLine(mainRotationJoint.position, mainRotationJoint.position + currentDir * visionRange, Color.cyan, 0.1f);

                    if (Physics.Raycast(mainRotationJoint.position, currentDir, out RaycastHit hit, visionRange,
                            LayerManager.Instance.playerGroundLayer))
                    {
                        if (hit.collider.CompareTag("Player"))
                        {
                            seenSomething = true;
                            seenLocation = hit.collider.transform.position;

                            return;
                        }
                    }
                    
                    currentDir = Quaternion.Euler(0, 4, 0) * currentDir;
                }
                
                currentDir = Quaternion.Euler(4, -visionRadiusY, 0) * currentDir;
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
            Gizmos.DrawFrustum(Vector3.zero, visionRadiusX, visionRange, 0, (float)-visionRadiusX / visionRadiusY);
        }
    }
    
    
    
}
