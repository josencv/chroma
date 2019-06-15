namespace Chroma.Infrastructure.Input
{
    /// <summary>
    /// Struct to save a pair of GameInputAxis and a value to do the correct mapping
    /// from keyboard key to controller axis with value (positive means up or right, 
    /// negative left or down, depending on the axis)
    /// </summary>
    public struct GameInputAxisValuePair
    {
        public GameInputAxis GameInputAxis { get; set; }
        public float Value { get; set; }

        public GameInputAxisValuePair(GameInputAxis axis, float value)
        {
            GameInputAxis = axis;
            Value = value;
        }
    }
}
