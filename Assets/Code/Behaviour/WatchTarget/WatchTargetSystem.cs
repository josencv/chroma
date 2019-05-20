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
        private struct WatchTargetJob : IJobForEach<WatchTargetComponent, Rotation>
        {
            public float DeltaTime;

            public void Execute(ref WatchTargetComponent watchTargetComponent, ref Rotation rotation)
            {
                // TODOING: importing THAT into THIS for THIS to work
                //float3 lookVector = target[0].Value - camPosition.Value;
                //Quaternion rotation = Quaternion.LookRotation(lookVector);
                //camRotation.Value = rotation;
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
