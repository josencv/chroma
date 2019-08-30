using Chroma.Game.Configuration;
using UnityEngine;

namespace Chroma.Behaviour.Skills.Push
{
    public class PushSkill
    {
        private const GameLayers pushablesLayerMask = GameLayers.Objects;
        private const float timeToForceCoefficient = 500;   // seconds to... eh... whatever unity uses as force unit? Newtons?
        private const float pushDistance = 2.0f;
        private const float pushFov = 30;       // The "field of vision" of the emission, in euler degrees. TODO: use it
        private const float raycastHeightOffset = 0.4f;     // How much the raycast start point should be displaced up

        private Transform transform;

        private float chargeThreshold = 1;  // In seconds
        private float chargeMaxCap = 5;     // In seconds
        private float chargedAmount;        // In seconds

        public PushSkill(Transform transform)
        {
            this.transform = transform;
            chargedAmount = 0;
        }

        public void Charge(float deltaTime)
        {
            chargedAmount += deltaTime;
        }

        public void Release()
        {
            if(chargedAmount < chargeThreshold)
            {
                chargedAmount = 0;
                return;
            }

            Vector3 rayStartingPoint = transform.position + Vector3.up * raycastHeightOffset;
            Ray ray = new Ray(rayStartingPoint, transform.forward);

            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, pushDistance, (int)pushablesLayerMask))
            {
                Pushable pushable = hit.collider.GetComponent<Pushable>();
                Push(pushable);
            }

            chargedAmount = 0;
        }

        private void Push(Pushable pushable)
        {
            Vector3 direction = transform.forward;
            Vector3 force = direction * Mathf.Min(chargedAmount, chargeMaxCap) * timeToForceCoefficient;
            pushable.GetPushed(force);
        }
    }
}
