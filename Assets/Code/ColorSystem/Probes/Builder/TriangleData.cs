using UnityEngine;

namespace Chroma.ColorSystem.Probes.Builder
{
    public struct TriangleData
    {
        public Color AverageColor { get; private set; }
        public Vector3 Center { get; private set; }
        public float Area { get; private set; }

        public TriangleData(Color averageColor, Vector3 center, float area)
        {
            AverageColor = averageColor;
            Center = center;
            Area = area;
        }
    }
}
