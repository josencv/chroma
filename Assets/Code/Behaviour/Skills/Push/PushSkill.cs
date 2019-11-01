using Chroma.Game.Configuration;
using UnityEngine;

namespace Chroma.Behaviour.Skills.Push
{
    public class PushSkill
    {
        private const GameLayers pushablesLayerMask = GameLayers.Objects;
        private const float amountToForceCoefficient = 50;   // seconds to... eh... whatever unity uses as force unit? Newtons?
        private const float pushDistance = 2.0f;
        private const float pushFov = 30;       // The "field of vision" of the emission, in euler degrees. TODO: use it
        private const float raycastHeightOffset = 0.4f;     // How much the raycast start point should be displaced up

        private Transform transform;

        private float minAmountThreshold = 5;  // In seconds
        private float maxAmountCap = 50;     // In seconds

        public PushSkill(Transform transform)
        {
            this.transform = transform;
        }

        public void Push(float amount)
        {
            Debug.Log("Trying to push object with an amount of " + amount);
            if(amount < minAmountThreshold)
            {
                return;
            }

            Vector3 rayStartingPoint = transform.position + Vector3.up * raycastHeightOffset;
            Ray ray = new Ray(rayStartingPoint, transform.forward);

            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, pushDistance, (int)pushablesLayerMask))
            {
                Pushable pushable = hit.collider.GetComponent<Pushable>();
                PushObject(pushable, amount);
            }
        }

        private void PushObject(Pushable pushable, float amount)
        {
            Vector3 direction = transform.forward;
            Vector3 force = direction * Mathf.Min(amount, maxAmountCap) * amountToForceCoefficient;
            pushable.GetPushed(force);
        }
    }
}
