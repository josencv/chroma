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
        private GameInput gameInput;
        private InputType currentInputType;

        public InputManager()
        {
            gameInput = new XboxGameInput(PlayerInputNumber.Player2, XboxController.First);
            currentInputType = InputType.Xbox;
        }

        public void ProcessGameInputs() 
        {
            gameInput.UpdateState();
        }

        public void DetectGameInputType()
        {
            if (currentInputType == InputType.Xbox && UnityEngine.Input.GetKeyDown(KeyCode.A))
            {
                Debug.Log("Changing to Keyboard Controller");
                gameInput.CleanState();
                gameInput = new KeyboardGameInput(PlayerInputNumber.Player1);
                currentInputType = InputType.Keyboard;
                InputChange?.Invoke(gameInput);
            }

            // Using XboxCtrlrInput to detect XBox controller type defeats one of the purposes of having an abstraction of the game input, but I think is unavoidable
            else if (currentInputType == InputType.Keyboard && XCI.GetButtonDown(XboxButton.A))
            {
                Debug.Log("Changing to XBox Controller");
                gameInput.CleanState();
                gameInput = new XboxGameInput(PlayerInputNumber.Player2, XboxController.First);  // TODO: resolve with DI
                currentInputType = InputType.Xbox;
                InputChange?.Invoke(gameInput);
            }
        }

        public GameInput GetGameInput()
        {
            return gameInput;
        }
    }

}
