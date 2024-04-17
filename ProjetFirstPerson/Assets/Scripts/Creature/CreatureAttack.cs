using System.Collections;
using UnityEngine;

namespace Creature
{
    public class CreatureAttack : MonoBehaviour, ICreatureComponent
    {
        [Header("Parameters")] 
        [SerializeField] private float attackRange;
        [SerializeField] private float attackDuration;
        [SerializeField] private float attackCooldown;
        
        [Header("Private Infos")] 
        private bool attacked;

        [Header("References")] 
        [SerializeField] private Collider attackCollider;
        private CreatureMover moveScript;
        
        
        void Start()
        {
            moveScript = GetComponent<CreatureMover>();
            attackCollider.enabled = false;
        }

        
        public void ComponentUpdate()
        {
            if (VerifyCanAttack())
                StartCoroutine(DoAttack());
        }
        

        private bool VerifyCanAttack()
        {
            if (attacked)
                return false;

            return Vector3.Distance(transform.position, CharacterManager.Instance.transform.position) < attackRange;
        } 

        private IEnumerator DoAttack()
        {
            attacked = true;
            attackCollider.enabled = true;
            moveScript.StopMoving();
            
            yield return new WaitForSeconds(attackDuration);
            
            attackCollider.enabled = false;
            
            yield return new WaitForSeconds(attackCooldown);
            
            moveScript.RestartMoving();
            attacked = false;
        }
    }
}
