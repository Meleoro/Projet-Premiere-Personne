using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Creature
{
    public class CreatureManager : MonoBehaviour
    {
        [Header("References")] 
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
    }
}
