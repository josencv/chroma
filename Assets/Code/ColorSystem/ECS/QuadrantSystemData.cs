using Unity.Collections;

namespace Chroma.ColorSystem.ECS
{
    public struct QuadrantSystemData
    {
        public NativeMultiHashMap<int, ProbeData> Value;
    }
}
