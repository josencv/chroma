namespace Chroma.ColorSystem
{
    public class ColorInfo
    {
        public Color Color { get; private set; }
        public bool Unlocked { get; private set; }

        public ColorInfo(Color color, bool unlocked)
        {
            Color = color;
            Unlocked = unlocked;
        }
    }
}
