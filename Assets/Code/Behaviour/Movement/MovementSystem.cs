using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;

namespace Chroma.Behaviour.Movement
{
    public class MovementSystem : JobComponentSystem
    {
        [BurstCompile]
        private struct MovementSpeedJob : IJobForEach<MovementComponent, PhysicsVelocity>
        {
            public void Execute([ReadOnly] ref MovementComponent movementComponent, ref PhysicsVelocity velocity)
            {
                velocity.Linear = movementComponent.MovementVector * movementComponent.Speed;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new MovementSpeedJob();

            return job.Schedule(this, inputDeps);
        }
    }
}
