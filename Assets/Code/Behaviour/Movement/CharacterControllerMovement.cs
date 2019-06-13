using Chroma.Infrastructure.Input;
using Chroma.Infrastructure.Unity;
using Chroma.Utility;
using UnityEngine;

namespace Chroma.Components.Movement
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterControllerMovement : MonoBehaviourExtension
    {
        private const float minMovementInputThreshold = 0.15f;

        private CharacterController controller;
        private new UnityEngine.Camera camera;
        private Vector3 gravity;

        [SerializeField] private float speed = 3;

        private void Awake()
        {
            gravity = Physics.gravity;
            camera = UnityEngine.Camera.main;
            controller = GetComponent<CharacterController>();
        }

        public void Move(float x, float z)
        {
            Vector3 intendedMovement = new Vector3(x, 0, z);
            if(intendedMovement.magnitude < minMovementInputThreshold)
            {
                intendedMovement = Vector3.zero;
            }

            intendedMovement = VectorMath.TransformToSpacePlane(camera.transform, intendedMovement);    // No gravity
            Vector3 movement = Time.deltaTime * (Vector3.ClampMagnitude(intendedMovement, 1) * speed + gravity); // Gravity added

            controller.Move(movement);

            if(intendedMovement != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(intendedMovement);
            }
        }

        private void Update()
        {
            float[] leftStickValues = InputManager.CurrentGameInput.GetStickState(GameInputStick.Left);
            Move(leftStickValues[0], leftStickValues[1]);
        }

        public void Stop()
        {
            controller.Move(Vector3.zero);
        }
    }
}
