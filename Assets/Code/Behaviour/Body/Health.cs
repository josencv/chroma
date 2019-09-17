using System;
using UnityEngine;

namespace Chroma.Behaviour.Body
{

    public class Health : MonoBehaviour
    {
        public event Action Died;

        [SerializeField]
        private float maxHealth = 100;
        private float currentHealth;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void ReceiveDamage(float damage)
        {
            currentHealth = Mathf.Max(currentHealth - damage, 0);
            if(currentHealth == 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Debug.Log("Entity has died");
            Died?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
