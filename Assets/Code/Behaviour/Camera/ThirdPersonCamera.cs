using System;
using Chroma.Game.Commands;
using Chroma.Game.Containers;
using Chroma.Infrastructure.Input;
using UnityEngine;
using Zenject;

namespace Chroma.Components.Camera
{
    /// <summary>
    /// Meant to be attached to the pivot of the camera arm.
    /// </summary>
    public class ThirdPersonCamera : MonoBehaviour
    {
        private const float minHorizontalTurnSpeed = 50;
        private const float maxHorizontalTurnSpeed = 150;
        private const float minVerticalTurnSpeed = 30;
        private const float maxVerticalTurnSpeed = 80;
        private const float maxVerticalRotation = 88;
        private const float minVerticalRotation = -20;
        private const float minInputThreshold = 0.15f;

        private Character characterContainer;
        private Transform pivot;
        private Transform cameraTransform;
        private float verticalPositionOffset;
        // Note: later on an additional "controller" layer may be needed
        private InputCommandIssuer commandIssuer;

        private float nextVerticalMovement = 0;
        private float nextHorizontalMovement = 0;

        [SerializeField] private Transform target;  // The target followed by the camera
        [SerializeField] private bool invertYAxis = true;   // Note: the Y axis in this context is the screen vertical axis
        [SerializeField] private bool invertXAxis = false;
        [SerializeField] private bool horizontalRotationEnabled = true;
        [SerializeField] private bool verticalRotationEnabled = false;
        [SerializeField] private float minDistanceToTarget = 5.0f;
        [SerializeField] private float maxVerticalPositionOffset = 0.7f;

        [SerializeField]
        [Range(minVerticalRotation, maxVerticalRotation)]
        private float verticalRotation = minVerticalRotation;

        [SerializeField]
        [Range(0.5f, 5f)]
        private float distanceToTargetFactor = 1.8f;

        [SerializeField]
        [Range(minHorizontalTurnSpeed, maxHorizontalTurnSpeed)]
        private float horizontalTurnSpeed = 100;

        [SerializeField]
        [Range(minVerticalTurnSpeed, maxVerticalTurnSpeed)]
        private float verticalTurnSpeed = 50;

        [Inject]
        private void Inject(UnityEngine.Camera camera, Character characterContainer, InputCommandIssuer commandIssuer)
        {
            cameraTransform = camera.transform;
            this.characterContainer = characterContainer;
            this.commandIssuer = commandIssuer;
        }

        private void Awake()
        {
            commandIssuer.CommandIssued += ProcessCommand;
            pivot = transform;

            if(target == null)
            {
                target = characterContainer.GetComponent<Transform>();
            }
        }

        private void Start()
        {
            ApplyRotation();
        }

        private void LateUpdate()
        {
            FollowTarget();
            ControlCamera();
        }

        private void FollowTarget()
        {
            pivot.position = new Vector3(target.position.x, target.position.y + verticalPositionOffset, target.position.z);
        }

        private void ProcessCommand(Command command, CommandArgs args)
        {
            if(command == Command.MoveCamera)
            {
                nextHorizontalMovement = args.Vector2Value.x;
                nextVerticalMovement = args.Vector2Value.y;
            }
        }

        private void ControlCamera()
        {
            RotateHorizontally(nextHorizontalMovement);
            RotateVertically(nextVerticalMovement);
        }

        private void RotateHorizontally(float amount)
        {
            if(Math.Abs(amount) < minInputThreshold || !horizontalRotationEnabled)
            {
                return;
            }

            float invert = invertXAxis ? 1 : -1;
            pivot.transform.RotateAround(target.transform.position, Vector3.up, Time.deltaTime * amount * horizontalTurnSpeed * invert);
        }

        private void RotateVertically(float amount)
        {
            if(Math.Abs(amount) < minInputThreshold || !verticalRotationEnabled)
            {
                return;
            }

            float invert = invertYAxis ? 1 : -1;
            verticalRotation += Time.deltaTime * amount * verticalTurnSpeed * invert;

            ApplyRotation();
        }

        private void ApplyRotation()
        {
            CorrectVerticalRotation();
            AdjustDistanceToTarget();
            AdjustVerticalPositionOffset();
        }

        private void CorrectVerticalRotation()
        {
            verticalRotation = Mathf.Clamp(verticalRotation, minVerticalRotation, maxVerticalRotation);

            // JIN: I don't know exactly how Unity's internal euler degrees works, but starting from 90, It looks like unity starts decreasing this value
            // Example: when the editor value is 80, the internal value is 80, but when the editor value is 100, the internal value is ALSO 80 (180 - verticalRotation)
            // So, after 90 degrees, behaviour becomes erratic. Maybe a way to avoid this is using radians instead of euler degrees,
            // but as long as we keep rotation under 90, everything should be fine.
            pivot.eulerAngles = new Vector3(verticalRotation, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }

        private void AdjustDistanceToTarget()
        {
            float distance = minDistanceToTarget + (verticalRotation) / 10.0f * distanceToTargetFactor;
            cameraTransform.transform.localPosition = new Vector3(cameraTransform.transform.localPosition.x, cameraTransform.transform.localPosition.y, -distance);
        }

        private void AdjustVerticalPositionOffset()
        {
            verticalPositionOffset = CalculateVerticalPositionOffset();
            pivot.position = new Vector3(pivot.position.x, target.position.y + verticalPositionOffset, pivot.position.z);
        }

        private float CalculateVerticalPositionOffset()
        {
            return maxVerticalPositionOffset * (1 - (verticalRotation - 90) / maxVerticalRotation);
        }

        public float VerticalRotation
        {
            get
            {
                return verticalRotation;
            }
        }
    }
}
