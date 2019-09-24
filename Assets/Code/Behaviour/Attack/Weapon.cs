using UnityEngine;
using Zenject;

namespace Chroma.Behaviour.Attack
{
    [RequireComponent(typeof(Collider))]
    public class Weapon : MonoBehaviour
    {
        private new Collider collider;

        [SerializeField]
        private float damage = 10f;
        [SerializeField]
        private int attackRate = 1000; // In milliseconds
        [SerializeField]
        private float range = 1.4f;
        [SerializeField]
        private float attackDuration = 100;  // In milliseconds

        [Inject]
        private void Inject(Collider collider)
        {
            this.collider = collider;
        }

        private void Awake()
        {
            collider.enabled = false;
            collider.isTrigger = true;
        }

        public void Enable()
        {
            collider.enabled = true;
        }

        public void Disable()
        {
            collider.enabled = false;
        }

        public float Damage { get { return damage; } }
        public float Range { get { return range; } }
        public int AttackRate { get { return attackRate; } }
        public float AttackDuration { get { return attackDuration; } }
    }
}
