using UnityEngine;

namespace Chroma.ColorSystem.Effects
{
    public class AbsorptionPointRenderData
    {
        public Vector3 Position { get; set; }
        public float Radius { get; set; }
        public float Saturation { get; set; }

        public AbsorptionPointRenderData(Vector3 position, float radius)
        {
            Position = position;
            Radius = radius;
            Saturation = 0;
        }
    }
}
