using Chroma.Infrastructure.Input;
using Chroma.Utility;
using UnityEngine;
using Zenject;

namespace Chroma.Components.Movement
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterControllerMovement : MonoBehaviour
    {
        private const float minMovementInputThreshold = 0.15f;
        private const float walkSpeed = 0.5f;
        private const float walkInputThreshold = 0.8f;
        private const float runSpeed = 1.7f;

        private CharacterController controller;
        private CharacterControllerAnimation animator;
        private new UnityEngine.Camera camera;
        private Vector3 gravity;

        [SerializeField] private float speedMultiplier = 3;

        [Inject]
        private void Inject(
            UnityEngine.Camera camera,
            CharacterController controller,
            [InjectOptional] CharacterControllerAnimation animator)
        {
            this.camera = camera;
            this.controller = controller;
            this.animator = animator;
        }

        private void Awake()
        {
            gravity = Physics.gravity;
        }

        private void Update()
        {
            float[] leftStickValues = InputManager.CurrentGameInput.GetStickState(GameInputStick.Left);
            Move(leftStickValues[0], leftStickValues[1]);
        }

        public void Move(float x, float z)
        {
            Vector3 intendedMovement = new Vector3(x, 0, z);
            if(intendedMovement.magnitude < minMovementInputThreshold)
            {
                intendedMovement = Vector3.zero;
            }

            intendedMovement = Vector3.ClampMagnitude(intendedMovement, 1);
            intendedMovement = VectorMath.TransformToSpacePlane(camera.transform, intendedMovement);    // No gravity
            float speed = intendedMovement.magnitude > walkInputThreshold ? runSpeed : walkSpeed;
            Vector3 movement = Time.deltaTime * (intendedMovement.normalized * speed * speedMultiplier + gravity); // Gravity and speed added

            controller.Move(movement);
            Animate(intendedMovement);

            if(intendedMovement != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(intendedMovement);
            }
        }

        private void Animate(Vector3 movement)
        {
            float value = movement.magnitude;

            // JIN: I'm adding a null check for now. We can later decide if the animator should be a required component
            if(animator != null)
            {
                animator.Animate("speed", value);
            }
        }

        public void Stop()
        {
            controller.Move(Vector3.zero);
        }
    }
}
