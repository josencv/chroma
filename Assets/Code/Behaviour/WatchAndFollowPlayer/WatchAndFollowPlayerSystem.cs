using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Chroma.Behaviour.WatchAndFollowPlayer
{
    public class WatchAndFollowPlayerSystem : JobComponentSystem
    {
        private EntityQuery m_Player;

        protected override void OnCreateManager()
        {
            // Cached access to a set of ComponentData based on a specific query 
            m_Player = GetEntityQuery(ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<Chroma.Game.Containers.CharacterContainer>());
        }

        [BurstCompile]
        private struct WatchTargetJob : IJobForEach<WatchAndFollowPlayerComponent, Rotation, Translation>
        {
            [ReadOnly]
            public float DeltaTime;

            [DeallocateOnJobCompletion]
            public NativeArray<Translation> Target;

            public void Execute([ReadOnly] ref WatchAndFollowPlayerComponent component, ref Rotation rotation, ref Translation translation)
            {
                // check if player exists
                if(Target.Length == 0)
                {
                    return;
                }

                float3 playerPosition = Target[0].Value;

                // Follow the Player
                float3 desiredPosition = playerPosition + component.FollowOffset;
                float3 smoothedPosition = math.lerp(translation.Value, desiredPosition, component.FollowSpeed * DeltaTime);
                translation.Value = smoothedPosition;

                // Rotate Camera to the Player
                float3 lookVector = playerPosition - translation.Value;
                Quaternion desiredRotation = Quaternion.LookRotation(lookVector);
                rotation.Value = Quaternion.Lerp(rotation.Value, desiredRotation, component.RotationSpeed * DeltaTime);
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
