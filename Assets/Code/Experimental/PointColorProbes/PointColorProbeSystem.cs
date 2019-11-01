using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Chroma.Experimental.PointColorProbes
{
    public class PointColorProbeSystem : JobComponentSystem
    {
        [BurstCompile]
        struct RotationSpeedRotation : IJobForEach<PointColorProbeComponent>
        {
            public float dt;

            public void Execute(ref PointColorProbeComponent probe)
            {
                rotation.value = math.mul(math.normalize(rotation.value), quaternion.axisAngle(math.up(), speed.speed * dt));
            }
        }

        // Any previously scheduled jobs reading/writing from Rotation or writing to RotationSpeed 
        // will automatically be included in the inputDeps dependency.
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new RotationSpeedRotation() { dt = Time.deltaTime };
            return job.Schedule(this, inputDeps);
        }
    }
}
