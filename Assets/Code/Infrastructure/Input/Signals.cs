namespace Chroma.Infrastructure.Input
{
    /// <summary>
    /// Represents a game signal that can be triggered by a GameInput button
    /// </summary>
    public delegate void ButtonSignal();

    /// <summary>
    /// Represents a game signal that can be triggered by a stick (axis)
    /// </summary>
    public delegate void StickSignal(float valueX, float valueY);
}
