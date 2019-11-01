using Unity.Entities;
using Unity.Mathematics;

namespace Chroma.Experimental.PointColorProbes
{
    public struct PointColorProbeComponent : IComponentData
    {
        public float3 Location;
        public float Color;
    }
}
