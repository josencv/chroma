using UnityEngine;
using Zenject;

namespace Chroma.Behaviour.Attack
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField]
        private new Collider collider;
        [SerializeField]
        private float damage = 10f;
        [SerializeField]
        private int attackRate = 1000; // In milliseconds
        [SerializeField]
        private float range = 1.4f;
        [SerializeField]
        private float attackDuration = 100;  // In milliseconds
        [SerializeField]
        private Quaternion sheathRotation = Quaternion.identity;  // In degrees
        [SerializeField]
        private Vector3 sheathTranslation = Vector3.zero;  // In milliseconds

        private void Awake()
        {
            collider.enabled = false;
            collider.isTrigger = true;
        }

        public void EnableCollider()
        {
            collider.enabled = true;
        }

        public void DisableCollider()
        {
            collider.enabled = false;
        }

        public float Damage { get { return damage; } }
        public float Range { get { return range; } }
        public int AttackRate { get { return attackRate; } }
        public float AttackDuration { get { return attackDuration; } }
        public Quaternion SheathRotation { get { return sheathRotation; } }
        public Vector3 SheathTranslation { get { return sheathTranslation; } }
    }
}
