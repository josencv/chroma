using Chroma.Behaviour.Body;
using Chroma.Game.Configuration;
using UnityEngine;
using Zenject;

namespace Chroma.Behaviour.Skills.Push
{
    [RequireComponent(typeof(Rigidbody))]
    public class Pushable : MonoBehaviour
    {
        private const float baseDamage = 5;

        /// <summary>
        /// The minimum momentum needed for the pushable object to do damage
        /// </summary>
        private const float momentumThreshold = 3.0f;

        private new Rigidbody rigidbody;
        private bool shouldBePushed;
        private Vector3 force;

        [Inject]
        private void Inject(Rigidbody rigidbody)
        {
            this.rigidbody = rigidbody;
        }

        private void Awake()
        {
            shouldBePushed = false;
        }

        public void GetPushed(Vector3 force)
        {
            shouldBePushed = true;
            this.force = force;
        }

        private void FixedUpdate()
        {
            if(shouldBePushed)
            {
                rigidbody.AddForce(force);
                shouldBePushed = false;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.tag == GameTag.Enemy)
            {
                Health health = collision.gameObject.GetComponent<Health>();
                if(health)
                {
                    float momentum = rigidbody.mass * rigidbody.velocity.magnitude;

                    if(momentum > momentumThreshold)
                    {
                        float damage = baseDamage * momentum;
                        Debug.Log(collision.gameObject.tag + " received " + damage + " damage from pushable object");
                        health?.ReceiveDamage(damage);
                    }
                    else
                    {
                        Debug.Log("Momentum of " + momentum + " was less than the minimum threshold, so no damage was done to " + collision.gameObject.tag);
                    }
                }
            }
        }
    }
}
