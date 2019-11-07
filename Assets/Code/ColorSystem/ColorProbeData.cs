using UnityEngine;

namespace Chroma.ColorSystem
{
    public struct ColorProbeData
    {
        public Vector3 Position { get; }
        public Color Color { get; }
        public float Amount { get; private set; }

        public ColorProbeData(Vector3 position, Color color)
        {
            Position = position;
            Color = color;
            Amount = 1;
        }

        public float GetAbsorbed()
        {
            float aux = Amount;
            Amount = 0;

            return aux;
        }

        public float Recover(float amount)
        {
            Amount = Mathf.Min(Amount + amount, 1);

            return Amount;
        }
    }
}
