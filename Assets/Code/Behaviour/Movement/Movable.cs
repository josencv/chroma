using Chroma.Behaviour.StateMachine;
using Chroma.Utility;
using UnityEngine;
using Zenject;

namespace Chroma.Behaviour.Movement
{
    [RequireComponent(typeof(CharacterController))]
    public class Movable : MonoBehaviour
    {
        private const float minMovementInputThreshold = 0.15f;
        private const float walkSpeed = 0.5f;
        private const float walkInputThreshold = 0.8f;
        private const float runSpeed = 1.7f;

        private EntityStateMachine entityStateMachine;
        private CharacterController controller;
        private CharacterControllerAnimation animator;
        private new UnityEngine.Camera camera;
        private Vector2 nextMovement = Vector2.zero;
        private Vector3 gravity;

        [SerializeField] private float speedMultiplier = 3;

        [Inject]
        private void Inject(
            UnityEngine.Camera camera,
            CharacterController controller,
            [InjectOptional] EntityStateMachine entityStateMachine,
            [InjectOptional] CharacterControllerAnimation animator)
        {
            this.camera = camera;
            this.controller = controller;
            this.entityStateMachine = entityStateMachine;
            this.animator = animator;
        }

        private void Awake()
        {
            gravity = Physics.gravity;
        }

        private void Update()
        {
            MoveTick();
        }

        public void Move(Vector2 movement)
        {
            nextMovement = movement;
        }

        private void MoveTick()
        {
            Vector3 intendedMovement = new Vector3(nextMovement.x, 0, nextMovement.y);
            if(intendedMovement.magnitude < minMovementInputThreshold)
            {
                intendedMovement = Vector3.zero;
            }

            intendedMovement = Vector3.ClampMagnitude(intendedMovement, 1);
            intendedMovement = VectorMath.TransformToSpacePlane(camera.transform, intendedMovement);    // No gravity
            float speed = intendedMovement.magnitude > walkInputThreshold ? runSpeed : walkSpeed;
            Vector3 realMovement = Time.deltaTime * (intendedMovement.normalized * speed * speedMultiplier + gravity); // Gravity and speed added

            controller.Move(realMovement);
            Animate(intendedMovement);
            UpdateStateMachineBlackboard(intendedMovement);

            if(intendedMovement != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(intendedMovement);
            }
        }

        private void UpdateStateMachineBlackboard(Vector3 movement)
        {
            float value = movement.magnitude;
            // TODO: de-hardcode string
            entityStateMachine?.StateMachine.SetFloatField("movementSpeed", value);
        }

        private void Animate(Vector3 movement)
        {
            float value = movement.magnitude;

            // JIN: I'm adding a null check for now. We can later decide if the animator should be a required component
            animator?.Animate("speed", value);
        }

        public void Stop()
        {
            controller.Move(Vector3.zero);
        }
    }
}
