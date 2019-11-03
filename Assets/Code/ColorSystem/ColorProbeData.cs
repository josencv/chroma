namespace Chroma.ColorSystem
{
    public struct ColorProbeData
    {
        private Color color;
        private float amount;

        public ColorProbeData(Color color)
        {
            this.color = color;
            amount = 1;
        }

        public Color Color { get { return color; } }
        public float Amount { get { return amount; } }
    }
}
