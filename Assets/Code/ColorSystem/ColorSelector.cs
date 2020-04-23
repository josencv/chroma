using System;
using Chroma.ColorSystem.UI;
using Chroma.Infrastructure.Input;
using UnityEngine;
using Zenject;

namespace Chroma.ColorSystem
{
    public class ColorSelector : MonoBehaviour
    {
        public event Action<Color> ColorChanged;

        private InputManager inputManager;
        private SelectedColorUIController selectedColorUIController;
        private GameInput gameInput;
        private int selectedColorIndex;
        private Color[] orderedColors =
        {
            Color.Red,
            Color.Yellow,
            Color.Green,
            Color.Cyan,
            Color.Blue,
            Color.Magenta,
        };

        public Color SelectedColor { get { return orderedColors[selectedColorIndex]; } }

        [Inject]
        private void Inject(InputManager inputManager, SelectedColorUIController selectedColorUIController)
        {
            this.inputManager = inputManager;
            this.selectedColorUIController = selectedColorUIController;
        }

        private void Awake()
        {
            selectedColorIndex = 0;
        }

        private void Start()
        {
            selectedColorUIController.ChangeSelectedColor(orderedColors[selectedColorIndex]);
        }

        private void Update()
        {
            // TODO: reorganize input management
            gameInput = inputManager.GetGameInput();
            if(gameInput.GetButtonState(GameInputButton.L1) == GameInputButtonState.Down)
            {
                SelectPreviousColor();
            }
            else if(gameInput.GetButtonState(GameInputButton.R1) == GameInputButtonState.Down)
            {
                SelectNextColor();
            }
        }

        private void SelectNextColor()
        {
            selectedColorIndex = (selectedColorIndex + 1) % orderedColors.Length;
            ColorChanged?.Invoke(orderedColors[selectedColorIndex]);
            StartChangeColorAnimation();
        }

        private void SelectPreviousColor()
        {
            selectedColorIndex--;
            if(selectedColorIndex < 0)
            {
                selectedColorIndex = orderedColors.Length - 1;
            }
            ColorChanged?.Invoke(orderedColors[selectedColorIndex]);
            StartChangeColorAnimation();
        }

        private void StartChangeColorAnimation()
        {
            selectedColorUIController.ChangeSelectedColor(orderedColors[selectedColorIndex]);
        }
    }
}
