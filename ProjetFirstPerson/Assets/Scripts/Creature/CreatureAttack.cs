using System.Collections;
using UnityEngine;

namespace Creature
{
    public class CreatureAttack : MonoBehaviour, ICreatureComponent
    {
        [Header("Parameters")] 
        [SerializeField] private float attackRange;
        [SerializeField] private float attackStartUp;
        [SerializeField] private float attackSpeed;
        [SerializeField] private float attackDuration;
        [SerializeField] private float attackCooldown;
        
        [Header("Private Infos")] 
        private bool attacked;

        [Header("References")] 
        [SerializeField] private Collider attackCollider;
        private CreatureMover moveScript;
        private CreatureManager mainScript;
        private CreatureWaypoints waypointsScript;
        
        
        void Start()
        {
            moveScript = GetComponent<CreatureMover>();
            waypointsScript = GetComponent<CreatureWaypoints>();
            mainScript = GetComponent<CreatureManager>();
            attackCollider.enabled = false;
        }

        
        public void ComponentUpdate()
        {
            if (mainScript.currentState != CreatureState.aggressive) return;
            
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

            moveScript.StopMoving();
            Vector3 saveTr = transform.position;
            Vector3 charSaveTr = CharacterManager.Instance.transform.position;
            
            yield return new WaitForSeconds(attackStartUp);

            attackCollider.enabled = true;

            moveScript.RestartMoving();
            waypointsScript.DoAttack(saveTr, charSaveTr);
            moveScript.StartAttackSpeed(attackSpeed);
            
            yield return new WaitForSeconds(attackDuration);

            moveScript.StopMoving();
            attackCollider.enabled = false;
            
            yield return new WaitForSeconds(attackCooldown);

            waypointsScript.isAttacking = false;

            moveScript.StartAggressiveSpeed();
            moveScript.RestartMoving();
            attacked = false;
        }
    }
}
