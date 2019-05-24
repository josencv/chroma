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
        private EntityQuery m_Player;

        protected override void OnCreateManager()
        {
            // Cached access to a set of ComponentData based on a specific query 
            m_Player = GetEntityQuery(ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<Chroma.Game.Containers.CharacterContainer>());
        }

        [BurstCompile]
        private struct WatchTargetJob : IJobForEach<WatchTargetComponent, Rotation, Translation>
        {
            [ReadOnly]
            public float DeltaTime;

            [DeallocateOnJobCompletion]
            public NativeArray<Translation> Target;

            public void Execute([ReadOnly] ref WatchTargetComponent watchTargetComponent, ref Rotation rotation, ref Translation translation)
            {
                // check if player exists
                if(Target.Length == 0)
                {
                    return;
                }

                // TODOING: importing THAT into THIS for THIS to work
                // Rotate Camera to the Player
                float3 lookVector = Target[0].Value - translation.Value;
                Quaternion newRotation = Quaternion.LookRotation(lookVector);
                rotation.Value = newRotation;
            }
        }
          
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new WatchTargetJob()
            {
                Target = m_Player.ToComponentDataArray<Translation>(Allocator.TempJob),
                DeltaTime = Time.deltaTime
            };

            return job.Schedule(this, inputDeps);
        }
    }
}
