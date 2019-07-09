using UnityEngine;
using XboxCtrlrInput;

namespace Chroma.Infrastructure.Input {

    public delegate void InputChangeEventHandler(GameInput input);

    /// <summary>
    /// Input manager service that manages all input received and maps
    /// to the correct action, depending on the game context or state.
    /// </summary>
    public class InputManager {
        public event InputChangeEventHandler InputChange;

        private InputType currentInputType;

        // TODO: change access mechanism to DI or something else
        public static GameInput CurrentGameInput { get; private set; }

        public InputManager()
        {
            CurrentGameInput = new XboxGameInput(PlayerInputNumber.Player1, XboxController.First);
            currentInputType = InputType.Xbox;
        }

        public void ProcessGameInputs() 
        {
            CurrentGameInput.UpdateState();
        }

        public void DetectGameInputType()
        {
            if (currentInputType == InputType.Xbox && UnityEngine.Input.GetKeyDown(KeyCode.A))
            {
                Debug.Log("Changing to Keyboard Controller");
                CurrentGameInput.CleanState();
                CurrentGameInput = new KeyboardGameInput(PlayerInputNumber.Player1);
                currentInputType = InputType.Keyboard;
                InputChange?.Invoke(CurrentGameInput);
            }

            // Using XboxCtrlrInput to detect XBox controller type defeats one of the purposes of having an abstraction of the game input, but I think is unavoidable
            else if (currentInputType == InputType.Keyboard && XCI.GetButtonDown(XboxButton.A))
            {
                Debug.Log("Changing to XBox Controller");
                CurrentGameInput.CleanState();
                CurrentGameInput = new XboxGameInput(PlayerInputNumber.Player2, XboxController.First);
                currentInputType = InputType.Xbox;
                InputChange?.Invoke(CurrentGameInput);
            }
        }

        public GameInput GetGameInput()
        {
            return CurrentGameInput;
        }
    }
}
