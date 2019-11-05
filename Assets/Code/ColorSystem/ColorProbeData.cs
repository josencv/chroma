using UnityEngine;

namespace Chroma.ColorSystem
{
    public struct ColorProbeData
    {
        public Vector3 Position;
        public Color Color;
        public float Amount;

        public ColorProbeData(Vector3 position, Color color)
        {
            Position = position;
            Color = color;
            Amount = 1;
        }
    }
}
