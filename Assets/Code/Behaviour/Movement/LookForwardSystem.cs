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
    public class LookForwardSystem : JobComponentSystem
    {
        [BurstCompile]
        private struct LookForwardJob : IJobForEach<MovementComponent, Rotation>
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
            var job = new LookForwardJob();

            return job.Schedule(this, inputDeps);
        }
    }
}
