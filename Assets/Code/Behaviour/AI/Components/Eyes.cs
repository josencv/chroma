﻿using Chroma.Game.Configuration;
using Chroma.Game.Containers;
using UnityEngine;
using Zenject;

namespace Chroma.Behaviour.AI.Components
{
    public class Eyes : MonoBehaviour
    {
        private Character character;

        [SerializeField]
        private float sightDistance = 10.0f;

        private float sqrSightDistance;
        private float fovAngle = 45.0f;  // In degrees

        [Inject]
        private void Inject(Character character)
        {
            this.character = character;
        }

        private void Start()
        {
            sqrSightDistance = sightDistance * sightDistance;
        }

        private void OnDrawGizmos()
        {
            Debug.DrawRay(transform.position, transform.forward * sightDistance, Color.red);
        }

        public bool IsTargetOnSight()
        {
            // We move the character position up a little to avoid raycasting to the feet of the player.
            // A game object could be used to simplify the calculation.
            Vector3 vectorInBetween = (character.transform.position + Vector3.up * 0.5f) - transform.position;
            if(IsTargetInFieldOfVision(vectorInBetween))
            {
                // Check if there are any obstacles between this object and the target
                RaycastHit hit;
                Ray ray = new Ray(transform.position, vectorInBetween);

                // TODO: add a layer mask to check only the relevant layers
                GameLayers layerMask = GameLayers.Characters;
                if(Physics.Raycast(ray, out hit, sightDistance, (int)layerMask) && hit.collider.tag == GameTag.Player)
                {
                    return true;
                }
            }


            return false;
        }

        private bool IsTargetInFieldOfVision(Vector3 vectorInBetween)
        {
            return vectorInBetween.sqrMagnitude <= sqrSightDistance &&
                Vector3.Angle(transform.forward, vectorInBetween) <= fovAngle;
        }
    }
}
