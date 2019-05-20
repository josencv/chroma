using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Chroma.Behaviour.WatchTarget
{
    public class WatchTargetSystem : JobComponentSystem
    {
        private struct WatchTargetJob : IJobForEach<WatchTargetComponent>
        {
            public float DeltaTime;

            public void Execute(ref WatchTargetComponent watchTargetComponent)
            {
                // COTO: For now the movement system will update the transform directly. We have yet to decide which physics system to use
                //var moveVector = new float3(movementComponent.HorizontalValue, 0, movementComponent.VerticalValue);
                //translation.Value += moveVector * movementComponent.Speed * DeltaTime;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new WatchTargetJob()
            {
                DeltaTime = Time.deltaTime
            };

            return job.Schedule(this, inputDeps);
        }
    }
}
