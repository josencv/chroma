using UnityEngine;
using Zenject;

namespace Chroma.Behaviour.Skills.Push
{
    [RequireComponent(typeof(Rigidbody))]
    public class Pushable : MonoBehaviour
    {
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
    }
}
