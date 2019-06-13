using System.Collections.Generic;

namespace Chroma.Infrastructure.Input
{
    /// <summary>
    /// This class represents the player game input. It is an abstraction from the actual hardware input used.
    /// </summary>
    public abstract class GameInput
    {
        protected InputType type;
        protected PlayerInputNumber playerInputNumber;                                  // Player input number to differenciate from other player inputs
        protected Dictionary<GameInputButton, GameInputButtonState> currentButtonState; // Saves the current state of the GameInput buttons
        protected Dictionary<GameInputStick, float[]> currentStickState;                // Stores the state of the GameInput sticks (composed axis)

        /// <summary>
        /// GameInput constructor
        /// </summary>
        /// <param name="type">Type of the hardaware input source</param>
        /// <param name="playerInputNumber">The number of the player to be assigned to the input</param>
        public GameInput(InputType type, PlayerInputNumber playerInputNumber)
        {
            this.type = type;
            this.playerInputNumber = playerInputNumber;
            currentButtonState = new Dictionary<GameInputButton, GameInputButtonState>();
            currentStickState = new Dictionary<GameInputStick, float[]>();

            InitializeControllerState();
        }

        /// <summary>
        /// Initializes the state for first time use
        /// </summary>
        private void InitializeControllerState()
        {
            currentButtonState.Add(GameInputButton.A, GameInputButtonState.Released);
            currentButtonState.Add(GameInputButton.B, GameInputButtonState.Released);
            currentButtonState.Add(GameInputButton.Y, GameInputButtonState.Released);
            currentButtonState.Add(GameInputButton.X, GameInputButtonState.Released);
            currentButtonState.Add(GameInputButton.L1, GameInputButtonState.Released);
            currentButtonState.Add(GameInputButton.L2, GameInputButtonState.Released);
            currentButtonState.Add(GameInputButton.R1, GameInputButtonState.Released);
            currentButtonState.Add(GameInputButton.R2, GameInputButtonState.Released);
            currentButtonState.Add(GameInputButton.Start, GameInputButtonState.Released);
            currentButtonState.Add(GameInputButton.Back, GameInputButtonState.Released);
            currentButtonState.Add(GameInputButton.LeftStick, GameInputButtonState.Released);
            currentButtonState.Add(GameInputButton.RightStick, GameInputButtonState.Released);
            currentButtonState.Add(GameInputButton.DPadUp, GameInputButtonState.Released);
            currentButtonState.Add(GameInputButton.DPadLeft, GameInputButtonState.Released);
            currentButtonState.Add(GameInputButton.DPadDown, GameInputButtonState.Released);
            currentButtonState.Add(GameInputButton.DPadRight, GameInputButtonState.Released);
            currentStickState.Add(GameInputStick.Left, new float[2]);
            currentStickState.Add(GameInputStick.Right, new float[2]);
        }

        /// <summary>
        /// Updates the GameInput button and axis states. To be implemented by each child class.
        /// </summary>
        public abstract void UpdateState();

        public void CleanState()
        {
            currentButtonState[GameInputButton.A] = GameInputButtonState.Released;
            currentButtonState[GameInputButton.B] = GameInputButtonState.Released;
            currentButtonState[GameInputButton.Y] = GameInputButtonState.Released;
            currentButtonState[GameInputButton.X] = GameInputButtonState.Released;
            currentButtonState[GameInputButton.L1] = GameInputButtonState.Released;
            currentButtonState[GameInputButton.L2] = GameInputButtonState.Released;
            currentButtonState[GameInputButton.R1] = GameInputButtonState.Released;
            currentButtonState[GameInputButton.R2] = GameInputButtonState.Released;
            currentButtonState[GameInputButton.Start] = GameInputButtonState.Released;
            currentButtonState[GameInputButton.Back] = GameInputButtonState.Released;
            currentButtonState[GameInputButton.LeftStick] = GameInputButtonState.Released;
            currentButtonState[GameInputButton.RightStick] = GameInputButtonState.Released;
            currentButtonState[GameInputButton.DPadUp] = GameInputButtonState.Released;
            currentButtonState[GameInputButton.DPadLeft] = GameInputButtonState.Released;
            currentButtonState[GameInputButton.DPadDown] = GameInputButtonState.Released;
            currentButtonState[GameInputButton.DPadRight] = GameInputButtonState.Released;
            currentStickState[GameInputStick.Left] = new float[2];
            currentStickState[GameInputStick.Right] = new float[2];
        }

        /// <summary>
        /// Gets the state of the button passed. Posible states are defined in the
        /// GameInputButton enum.
        /// </summary>
        /// <param name="button">GameInput button type to be checked</param>
        /// <returns></returns>
        public GameInputButtonState GetButtonState(GameInputButton button)
        {
            return currentButtonState[button];
        }

        /// <summary>
        /// Gets the axis values of the stick passed.
        /// </summary>
        /// <param name="stick">GameInput stick to be checked</param>
        /// <returns></returns>
        public float[] GetStickState(GameInputStick stick)
        {
            return currentStickState[stick];
        }

        #region Private methods

        /// <summary>
        /// Triggers a button signal event
        /// </summary>
        /// <param name="signal"></param>
        private void TriggerEvent(ButtonSignal signal)
        {
            if (signal != null)
            {
                signal.Invoke();
            }
        }

        private void TriggerEvent(StickSignal signal, float x, float y)
        {
            if (signal != null)
            {
                signal.Invoke(x, y);
            }
        }

        #endregion

        public PlayerInputNumber PlayerInputNumber { get { return playerInputNumber; } }
    }
}
