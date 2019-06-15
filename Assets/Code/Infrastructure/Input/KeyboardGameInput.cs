using UnityEngine;
using System.Collections.Generic;

namespace Chroma.Infrastructure.Input
{
    /// <summary>
    /// This class is the keyboard specialization of the GameInput class.
    /// Allows a player to play the game with the keyboard.
    /// </summary>
    public class KeyboardGameInput : GameInput
    {
        /// <summary>
        /// Keyboard key to GameInputButton mapper dictionary
        /// </summary>
        private Dictionary<KeyCode, GameInputButton> buttonMapper;
        /// <summary>
        /// Keyboard key to GameInputAxis plus axis value mapper dictionary
        /// </summary>
        private Dictionary<KeyCode, GameInputAxisValuePair> axisMapper;

        public KeyboardGameInput(PlayerInputNumber playerInputNumber)
            : base(InputType.Keyboard, playerInputNumber)
        {
            buttonMapper = new Dictionary<KeyCode, GameInputButton>();
            axisMapper = new Dictionary<KeyCode, GameInputAxisValuePair>();
            RegisterInputs();
        }

        /// <summary>
        /// Maps all the game inputs to a keyboard key
        /// </summary>
        private void RegisterInputs()
        {
            buttonMapper.Add(KeyCode.A, GameInputButton.A);
            buttonMapper.Add(KeyCode.S, GameInputButton.B);
            buttonMapper.Add(KeyCode.D, GameInputButton.Y);
            buttonMapper.Add(KeyCode.W, GameInputButton.X);
            buttonMapper.Add(KeyCode.Q, GameInputButton.L1);
            buttonMapper.Add(KeyCode.E, GameInputButton.R1);
            buttonMapper.Add(KeyCode.Alpha1, GameInputButton.L2);
            buttonMapper.Add(KeyCode.Alpha4, GameInputButton.R2);
            buttonMapper.Add(KeyCode.Alpha2, GameInputButton.LeftStick);
            buttonMapper.Add(KeyCode.Alpha3, GameInputButton.RightStick);
            buttonMapper.Add(KeyCode.Return, GameInputButton.Start);
            buttonMapper.Add(KeyCode.Space, GameInputButton.Back);
            buttonMapper.Add(KeyCode.I, GameInputButton.DPadUp);
            buttonMapper.Add(KeyCode.J, GameInputButton.DPadLeft);
            buttonMapper.Add(KeyCode.K, GameInputButton.DPadDown);
            buttonMapper.Add(KeyCode.L, GameInputButton.DPadRight);

            axisMapper.Add(KeyCode.UpArrow, new GameInputAxisValuePair(GameInputAxis.LeftStickY, (float)GameInputAxisValue.MaxValue));
            axisMapper.Add(KeyCode.DownArrow, new GameInputAxisValuePair(GameInputAxis.LeftStickY, (float)GameInputAxisValue.MinValue));
            axisMapper.Add(KeyCode.RightArrow, new GameInputAxisValuePair(GameInputAxis.LeftStickX, (float)GameInputAxisValue.MaxValue));
            axisMapper.Add(KeyCode.LeftArrow, new GameInputAxisValuePair(GameInputAxis.LeftStickX, (float)GameInputAxisValue.MinValue));
            axisMapper.Add(KeyCode.Keypad8, new GameInputAxisValuePair(GameInputAxis.RightStickY, (float)GameInputAxisValue.MaxValue));
            axisMapper.Add(KeyCode.Keypad5, new GameInputAxisValuePair(GameInputAxis.RightStickY, (float)GameInputAxisValue.MinValue));
            axisMapper.Add(KeyCode.Keypad6, new GameInputAxisValuePair(GameInputAxis.RightStickX, (float)GameInputAxisValue.MaxValue));
            axisMapper.Add(KeyCode.Keypad4, new GameInputAxisValuePair(GameInputAxis.RightStickX, (float)GameInputAxisValue.MinValue));
        }

        public override void UpdateState()
        {
            // Triggers button down game events
            foreach (KeyValuePair<KeyCode, GameInputButton> entry in buttonMapper)
            {
                if (UnityEngine.Input.GetKeyDown(entry.Key))
                {
                    currentButtonState[entry.Value] = GameInputButtonState.Down;
                }
                else if (UnityEngine.Input.GetKeyUp(entry.Key))
                {
                    currentButtonState[entry.Value] = GameInputButtonState.Up;
                }
                else if (UnityEngine.Input.GetKey(entry.Key))
                {
                    currentButtonState[entry.Value] = GameInputButtonState.Pressed;
                }
                else
                {
                    currentButtonState[entry.Value] = GameInputButtonState.Released;
                }
            }

            float[] leftStickValues = new float[2];
            float[] rightStickValues = new float[2];

            // Triggers axis actions
            foreach (KeyValuePair<KeyCode, GameInputAxisValuePair> entry in axisMapper)
            {
                if (UnityEngine.Input.GetKey(entry.Key))
                {
                    switch (entry.Value.GameInputAxis)
                    {
                        case GameInputAxis.LeftStickX:
                            leftStickValues[0] = entry.Value.Value;
                            break;
                        case GameInputAxis.LeftStickY:
                            leftStickValues[1] = entry.Value.Value;
                            break;
                        case GameInputAxis.RightStickX:
                            rightStickValues[0] = entry.Value.Value;
                            break;
                        case GameInputAxis.RightStickY:
                            rightStickValues[1] = entry.Value.Value;
                            break;
                    }
                }
            }

            // Updates the GameInput stick (axis) states
            currentStickState[GameInputStick.Left] = leftStickValues;
            currentStickState[GameInputStick.Right] = rightStickValues;
        }

    }

}
