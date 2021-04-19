using System;
using System.Collections;
using Chroma.Behaviour.Body;
using Chroma.Game.Configuration;
using UnityEngine;

namespace Chroma.Behaviour.Attack
{
    public class SimpleAttack : MonoBehaviour
    {
        [SerializeField]
        private Weapon weapon;
        private bool isAttacking;
        private DateTime lastAttackTime;
        
        /// <summary>
        /// The recommended distance between the attacker and the target to execute the attack. Used mostly in AI.
        /// Note: consider moving this logic out of this component
        /// </summary>
        public float AttackApproachDistance { get { return weapon.Range * 0.8f; } }

        /// <summary>
        /// The attack rate in milliseconds
        /// </summary>
        public int AttackRate { get { return weapon.AttackRate; } }

        private void Awake()
        {
            if(weapon == null)
            {
                throw new ApplicationException("a weapon was not provided");
            }

            lastAttackTime = DateTime.MinValue;
            isAttacking = false;
        }

        public void StartAttack()
        {
            Debug.Log("CanAttack() is " + CanAttack());
            if(!CanAttack())
            {
                return;
            }

            isAttacking = true;
            weapon.EnableCollider();
            StartCoroutine(AttackAnimation());
        }

        private bool CanAttack()
        {
            TimeSpan elapsedTime = DateTime.Now - lastAttackTime;
            return !isAttacking && elapsedTime.TotalMilliseconds >= weapon.AttackRate;
        }

        private void FinishAttack()
        {
            weapon.DisableCollider();
            isAttacking = false;
            lastAttackTime = DateTime.Now;
        }

        // Simulates and attack animation: waits for the attack duration and then finishes the attack
        private IEnumerator AttackAnimation()
        {
            yield return new WaitForSeconds(weapon.AttackDuration / 1000);
            FinishAttack();
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("OnTriggerEnter SimpleAttack");
            if(IsHostile(other))
            {
                Hit(other);
            }
        }

        private bool IsHostile(Component component)
        {
            return (component.transform.root.tag == GameTag.Player && tag == GameTag.Enemy) ||
                   (component.transform.root.tag == GameTag.Enemy && tag == GameTag.Player);
        }

        private void Hit(Collider other)
        {
            Health health = other.GetComponent<Health>();
            if(health)
            {
                Debug.Log(health.gameObject.tag + " received " + weapon.Damage + " damage from " + gameObject.tag + " simple attack");
                health.ReceiveDamage(weapon.Damage);
            }
        }

        private void OnDrawGizmos()
        {
            Debug.DrawRay(transform.position + Vector3.up * 1f, transform.forward * weapon.Range, Color.cyan);
        }
    }
}
