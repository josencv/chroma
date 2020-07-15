using System;
using UnityEngine;

namespace Chroma.ColorSystem.Probes
{
    [Serializable]
    public struct ColorProbe
    {
        [SerializeField]
        private Vector3 position;
        [SerializeField]
        private Color color;
        [NonSerialized]
        private float amount;

        public Vector3 Position { get { return position; } }
        public Color Color { get { return color; } }
        public float Amount { get { return amount; } }

        public ColorProbe(Vector3 position, Color color)
        {
            this.position = position;
            this.color = color;
            amount = 1;
        }

        public float GetAbsorbed()
        {
            float aux = Amount;
            amount = 0;

            return aux;
        }

        public float Recover(float recoveryAmount)
        {
            amount = Mathf.Min(amount + recoveryAmount, 1);

            return amount;
        }
    }
}
