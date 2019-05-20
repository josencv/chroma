//using System.Collections;
//using System.Collections.Generic;
//using Unity.Burst;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Jobs;
//using Unity.Mathematics;
//using Unity.Transforms;
//using UnityEngine;

//namespace Chroma.Behaviour.CameraFollow
//{
//    [UpdateAfter(typeof(Movement.MovementSystem))]
//    public class CameraFollowSystem : JobComponentSystem
//    {
//        private EntityQuery m_Player;

//        protected override void OnCreateManager()
//        {
//            // Cached access to a set of ComponentData based on a specific query 
//            m_Player = GetEntityQuery(ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<Player>());
//        }

//        [BurstCompile]
//        public struct CameraFollowJob : IJobForEach<Translation, Rotation, CameraComponent>
//        {
//            [DeallocateOnJobCompletion]
//            public NativeArray<Translation> target;


//            public float deltaTime;
//            public void Execute(ref Translation camPosition, ref Rotation camRotation, [ReadOnly]  ref CameraComponent c2)
//            {
//                // check if player exist
//                if(target.Length == 0)
//                {
//                    return;
//                }

//                // Follow the Player
//                float3 desiredPosition = target[0].Value; // + GameBootstrap.instance.offset;
//                //float3 smoothedPosition = math.lerp(camPosition.Value, desiredPosition, GameBootstrap.instance.CameraSmoothSpeed * deltaTime);
//                //camPosition.Value = smoothedPosition;
//                camPosition.Value = desiredPosition;

//                // Rotate Camera to the Player
//                float3 lookVector = target[0].Value - camPosition.Value;
//                Quaternion rotation = Quaternion.LookRotation(lookVector);
//                camRotation.Value = rotation;
//            }
//        }

//        protected override JobHandle OnUpdate(JobHandle inputDeps)
//        {
//            var job = new CameraFollowJob
//            {
//                target = m_Player.ToComponentDataArray<Translation>(Allocator.TempJob),
//                deltaTime = Time.deltaTime
//            };
//            return job.Schedule(this, inputDeps);

//        }
//    }
//}
