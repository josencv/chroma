using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Chroma.ColorSystem.ECS
{
    public class ColorProbeQuadrantSystem : JobComponentSystem
    {
        public const float QuadrandSize = 50;
        NativeMultiHashMap<int, ProbeData> quadrantsHashMap;

        protected override void OnCreate()
        {
            base.OnCreate();
            EntityManager.CreateArchetype(typeof(QuadrantSystemData));
            quadrantsHashMap = new NativeMultiHashMap<int, ProbeData>(1000, Allocator.Persistent);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            quadrantsHashMap.Dispose();
        }

        [BurstCompile]
        struct ColorProbeQuadrantJob : IJobForEachWithEntity<Translation, PointColorProbeComponent>
        {
            public NativeMultiHashMap<int, ProbeData>.ParallelWriter QuadrantsWriter;
            public EntityCommandBuffer commandBuffer;

            public void Execute(Entity entity, int index, [ReadOnly] ref Translation translation, [ReadOnly] ref PointColorProbeComponent probe)
            {
                int quadrantHash = GetQuadrantHash(translation.Value);
                QuadrantsWriter.Add(quadrantHash, new ProbeData { Probe = probe, Position = translation.Value });

                //commandBuffer.DestroyEntity(entity);
                // TODO: delete entities
                // TODO: store the NativeMultiHashMap as a new entity with a unique archetype
            }
        }

        private static int GetQuadrantHash(float3 position)
        {
            return (int)(math.floor(position.x / QuadrandSize) +
                         math.floor(position.y / QuadrandSize) * 1000 +
                         math.floor(position.z / QuadrandSize) * 1000000);
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new ColorProbeQuadrantJob() {
                QuadrantsWriter = quadrantsHashMap.AsParallelWriter(),
            };

            return job.Schedule(this, inputDeps);
        }
    }
}
