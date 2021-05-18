namespace Chroma.Infrastructure.FSM
{
    enum StateMachineFieldType { Float, Int, Bool, Trigger }

    /// <summary>
    /// A field of a state machine. State machines changes between states by changing these fields.
    /// Can be a float, int, bool or trigger field
    /// Important note: field value is always a float, regardless the field type, as any of those types
    /// can be represented with a float value (e.g. boolean value 'true' is 1.0f)
    /// </summary>
    class StateMachineField
    {
        public StateMachineFieldType Type { get; set; } // The field type
        public float Value { get; set; }                // Current value of the field

        /// <summary>
        /// Initializes an instance of the StateMachineField class
        /// </summary>
        /// <param name="type">The type of the field</param>
        /// <param name="value">The starting value of the field</param>
        public StateMachineField(StateMachineFieldType type, float value)
        {
            Type = type;
            Value = value;
        }
    }
}
