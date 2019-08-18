using Chroma.Game.Configuration;
using Chroma.Game.Containers;
using UnityEngine;
using Zenject;

namespace Chroma.Behaviour.AI.Components
{
    public class Eyes : MonoBehaviour
    {
        private CharacterContainer character;

        [SerializeField]
        private float sightDistance = 10.0f;

        private float sqrSightDistance;
        private float fovAngle = 45.0f;  // In degrees

        [Inject]
        private void Inject(CharacterContainer character)
        {
            this.character = character;
        }

        private void Start()
        {
            sqrSightDistance = sightDistance * sightDistance;
        }


#if UNITY_EDITOR
        private void Update()
        {
            Debug.DrawRay(transform.position, transform.forward * sightDistance, Color.red);
        }
#endif

        public bool IsTargetOnSight()
        {
            // We move the character position up a little to avoid raycasting to the feet of the player.
            // A game object could be used to simplify the calculation.
            Vector3 vectorInBetween = (character.transform.position + Vector3.up * 0.5f) - transform.position;
            if(IsTargetInFieldOfVision(vectorInBetween))
            {
                RaycastHit hit;
                Ray ray = new Ray(transform.position, vectorInBetween);

                // TODO: add a layer mask to check only the relevant layers
                if(Physics.Raycast(ray, out hit, sightDistance) && hit.collider.tag == Tags.Player)
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
