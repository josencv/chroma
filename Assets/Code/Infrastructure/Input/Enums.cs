namespace Chroma.Infrastructure.Input
{
    /// <summary>
    /// Enumerates the possible numbers of the input of a player
    /// </summary>
    public enum PlayerInputNumber { Player1 = 0, Player2 = 1, Player3 = 2, Player4 = 3 }

    /// <summary>
    /// Enumerates the posible game input types, like an Xbox controller or the keyboard
    /// </summary>
    public enum InputType { Xbox = 0, Keyboard = 1, Joystick = 2 }

    /// <summary>
    /// Enumerates all the game buttons available. Any input source should be mapped to these game buttons
    /// (like an xbox controller or a keyboard)
    /// </summary>
    public enum GameInputButton { A, B, X, Y, Start, Back, L1, L2, R1, R2, DPadUp, DPadLeft, DPadDown, DPadRight, LeftStick, RightStick, None }

    /// <summary>
    /// Enumerates all the game axis available. Any input source should be mapped to these game axis
    /// (like an xbox controller or a keyboard)
    /// </summary>
    public enum GameInputAxis { LeftStickX, LeftStickY, RightStickX, RightStickY, None }

    /// <summary>
    /// Used to differentiate the left from the right stick from the controller
    /// </summary>
    public enum GameInputStick { Left, Right }

    /// <summary>
    /// Represents the maximum and minimum posible values for an input axis. It is used to map the keyboard buttons
    /// to an axis with a value.
    /// </summary>
    public enum GameInputAxisValue { MaxValue = 1, MinValue = -1 }

    /// <summary>
    /// Posible states of a button
    /// </summary>
    public enum GameInputButtonState { Pressed, Down, Up, Released }
}
