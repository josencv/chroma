using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

using static Unity.Mathematics.math;
using static Unity.Mathematics.quaternion;


namespace Chroma.Behaviour.Movement
{
    public class RotationSystem : JobComponentSystem
    {
        [BurstCompile]
        private struct LookAtJob : IJobForEach<MovementComponent, Rotation>
        {
            public void Execute([ReadOnly] ref MovementComponent movementComponent, ref Rotation rotation)
            {
                if(length(movementComponent.MovementVector) > 0)
                {
                    rotation.Value = LookRotation(movementComponent.MovementVector, new float3(0, 1, 0));
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new LookAtJob();

            return job.Schedule(this, inputDeps);
        }
    }
}
