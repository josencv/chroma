using Unity.Mathematics;

namespace Chroma.Experimental.PointColorProbes
{
    public struct PointColorProbe
    {
        public float3 location;
        public int color;

        public PointColorProbe(float3 location, int color)
        {
            this.location = location;
            this.color = color;
        }
    }
}
