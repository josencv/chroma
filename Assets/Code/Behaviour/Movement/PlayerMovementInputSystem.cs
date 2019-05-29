using Chroma.Game.Containers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Chroma.Behaviour.Movement
{
    public class PlayerMovementInputSystem : JobComponentSystem
    {
        private EntityQuery Camera;

        protected override void OnCreate()
        {
            Camera = GetEntityQuery(ComponentType.ReadOnly<Rotation>(), ComponentType.ReadOnly<LocalToWorld>(), ComponentType.ReadOnly<CameraContainer>());
        }

        [BurstCompile]
        private struct PlayerMovementInputJob : IJobForEach<MovementComponent, CharacterContainer>
        {
            public float HorizontalInput;
            public float VerticalInput;

            [DeallocateOnJobCompletion]
            public NativeArray<LocalToWorld> CameraLocalToWorldArray;

            public void Execute(ref MovementComponent movementComponent, [ReadOnly] ref CharacterContainer characterContainer)
            {
                // Converting input vector to camera space, ignoring camera vertical inclination:
                // Build the "camera space parallel to the floor" rotation matrix with the camera LocalToWorld component
                float3 movementVector = new float3(HorizontalInput, 0, VerticalInput);
                float3 forwardParallel = CameraLocalToWorldArray[0].Forward;
                forwardParallel = math.normalize(new float3(forwardParallel.x, 0, forwardParallel.z));  // Make camera forward parallel to the floor by zeroing out Y value and normalizing
                float3 cameraRight = CameraLocalToWorldArray[0].Right;
                float3 worldUp = new float3(0, 1, 0);

                float3x3 rotationMatrix = new float3x3(cameraRight, worldUp, forwardParallel);

                quaternion cameraRotationParallelToFloor = new quaternion(rotationMatrix);
                movementVector = math.mul(cameraRotationParallelToFloor, movementVector);
                movementComponent.MovementVector = movementVector;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new PlayerMovementInputJob()
            {
                CameraLocalToWorldArray = Camera.ToComponentDataArray<LocalToWorld>(Allocator.TempJob),
                HorizontalInput = Input.GetAxis("Horizontal"),
                VerticalInput = Input.GetAxis("Vertical"),
            };

            return job.Schedule(this, inputDeps);
        }
    }
}
