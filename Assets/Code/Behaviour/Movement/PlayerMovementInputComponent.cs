using Chroma.Game.Containers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Chroma.Behaviour.Movement
{
    public class PlayerMovementInputSystem : JobComponentSystem
    {
        [BurstCompile]
        private struct PlayerMovementInputJob : IJobForEach<MovementComponent, CharacterContainer>
        {
            public float HorizontalInput;
            public float VerticalInput;

            public void Execute(ref MovementComponent movementComponent, [ReadOnly] ref CharacterContainer characterContainer)
            {
                // COTO: we could always change which input system we use later. For now I'm using Unity's built-in system.
                movementComponent.HorizontalValue = HorizontalInput;
                movementComponent.VerticalValue = VerticalInput;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new PlayerMovementInputJob()
            {
                HorizontalInput = Input.GetAxis("Horizontal"),
                VerticalInput = Input.GetAxis("Vertical"),
            };

            return job.Schedule(this, inputDeps);
        }
    }
}
