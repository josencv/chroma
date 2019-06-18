using System.Collections.Generic;
using XboxCtrlrInput;

namespace Chroma.Infrastructure.Input
{
    public class XboxGameInput : GameInput
    {
        private Dictionary<XboxButton, GameInputButton> buttonMapper;   // Xbox button to GameInputButton mapper dictionary
        private Dictionary<XboxDPad, GameInputButton> DPadMapper;       // Xbox DPad to GameInputButton mapper dictionary
        private Dictionary<XboxAxis, GameInputAxis> axisMapper;         // Xbox axis to GameInputAxis plus axis value mapper dictionary
        private Dictionary<XboxAxis, GameInputButton> specialMapper;    // Special mapper used to change behaviour of some inputs, like the axis triggers
        private Dictionary<XboxAxis, float> previousTriggerState;       // Used to generate trigger down or pressed states
        private XboxController xboxControllerInputNumber;

        public XboxGameInput(PlayerInputNumber playerInputNumber, XboxController xboxControllerInputNumber)
            : base(InputType.Xbox, playerInputNumber)
        {
            this.xboxControllerInputNumber = xboxControllerInputNumber;
            buttonMapper = new Dictionary<XboxButton, GameInputButton>();
            DPadMapper = new Dictionary<XboxDPad, GameInputButton>();
            axisMapper = new Dictionary<XboxAxis, GameInputAxis>();
            specialMapper = new Dictionary<XboxAxis, GameInputButton>();
            previousTriggerState = new Dictionary<XboxAxis, float>();
            previousTriggerState.Add(XboxAxis.LeftTrigger, 0);
            previousTriggerState.Add(XboxAxis.RightTrigger, 0);
            RegisterInputs();
        }

        /// <summary>
        /// Maps all the game inputs to a keyboard key
        /// </summary>
        private void RegisterInputs()
        {
            buttonMapper.Add(XboxButton.A, GameInputButton.A);
            buttonMapper.Add(XboxButton.B, GameInputButton.B);
            buttonMapper.Add(XboxButton.Y, GameInputButton.Y);
            buttonMapper.Add(XboxButton.X, GameInputButton.X);
            buttonMapper.Add(XboxButton.LeftBumper, GameInputButton.L1);
            buttonMapper.Add(XboxButton.RightBumper, GameInputButton.R1);
            buttonMapper.Add(XboxButton.LeftStick, GameInputButton.LeftStick);
            buttonMapper.Add(XboxButton.RightStick, GameInputButton.RightStick);
            buttonMapper.Add(XboxButton.Start, GameInputButton.Start);
            buttonMapper.Add(XboxButton.Back, GameInputButton.Back);

            DPadMapper.Add(XboxDPad.Up, GameInputButton.DPadUp);
            DPadMapper.Add(XboxDPad.Left, GameInputButton.DPadLeft);
            DPadMapper.Add(XboxDPad.Down, GameInputButton.DPadDown);
            DPadMapper.Add(XboxDPad.Right, GameInputButton.DPadRight);

            axisMapper.Add(XboxAxis.LeftStickX, GameInputAxis.LeftStickX);
            axisMapper.Add(XboxAxis.LeftStickY, GameInputAxis.LeftStickY);
            axisMapper.Add(XboxAxis.RightStickX, GameInputAxis.RightStickX);
            axisMapper.Add(XboxAxis.RightStickY, GameInputAxis.RightStickY);
            axisMapper.Add(XboxAxis.LeftTrigger, GameInputAxis.RightStickX);
            axisMapper.Add(XboxAxis.RightTrigger, GameInputAxis.RightStickY);

            specialMapper.Add(XboxAxis.LeftTrigger, GameInputButton.L2);
            specialMapper.Add(XboxAxis.RightTrigger, GameInputButton.R2);
        }

        public override void UpdateState()
        {
            // Triggers button down game events
            foreach (KeyValuePair<XboxButton, GameInputButton> entry in buttonMapper)
            {
                if (XCI.GetButtonDown(entry.Key, xboxControllerInputNumber))
                {
                    currentButtonState[entry.Value] = GameInputButtonState.Down;
                }
                else if(XCI.GetButtonUp(entry.Key, xboxControllerInputNumber))
                {
                    currentButtonState[entry.Value] = GameInputButtonState.Up;
                }
                else if (XCI.GetButton(entry.Key, xboxControllerInputNumber))
                {
                    currentButtonState[entry.Value] = GameInputButtonState.Pressed;
                }
                else
                {
                    currentButtonState[entry.Value] = GameInputButtonState.Released;
                }
            }

            // Special mapping for special mapper
            foreach (KeyValuePair<XboxAxis, GameInputButton> entry in specialMapper)
            {
                float triggerValue = XCI.GetAxis(entry.Key, xboxControllerInputNumber);

                if (triggerValue > 0 && previousTriggerState[entry.Key] == 0)
                {
                    currentButtonState[entry.Value] = GameInputButtonState.Down;
                }
                else if (XCI.GetAxis(entry.Key, xboxControllerInputNumber) > 0)
                {
                    currentButtonState[entry.Value] = GameInputButtonState.Pressed;
                }
                else if(XCI.GetAxis(entry.Key, xboxControllerInputNumber) == 0)
                {
                    currentButtonState[entry.Value] = GameInputButtonState.Up;
                }

                previousTriggerState[entry.Key] = triggerValue;
            }

            float[] leftStickValues = new float[2];
            float[] rightStickValues = new float[2];

            // Triggers axis actions
            foreach (KeyValuePair<XboxAxis, GameInputAxis> entry in axisMapper)
            {
                switch (entry.Key)
                {
                    case XboxAxis.LeftStickX:
                        leftStickValues[0] = XCI.GetAxis(XboxAxis.LeftStickX, xboxControllerInputNumber);
                        break;
                    case XboxAxis.LeftStickY:
                        leftStickValues[1] = XCI.GetAxis(XboxAxis.LeftStickY, xboxControllerInputNumber);
                        break;
                    case XboxAxis.RightStickX:
                        rightStickValues[0] = XCI.GetAxis(XboxAxis.RightStickX, xboxControllerInputNumber);
                        break;
                    case XboxAxis.RightStickY:
                        rightStickValues[1] = XCI.GetAxis(XboxAxis.RightStickY, xboxControllerInputNumber);
                        break;
                }
            }

            // Updates the GameInput stick (axis) states
            currentStickState[GameInputStick.Left] = leftStickValues;
            currentStickState[GameInputStick.Right] = rightStickValues;
        }
    }
}
