using Chroma.Infrastructure.Input;
using UnityEngine;
using Zenject;

namespace Chroma.Game
{
    public class GameLoop : MonoBehaviour
    {
        private InputManager inputManager;

        [Inject]
        private void Inject(InputManager inputManager)
        {
            this.inputManager = inputManager;
        }

        private void Update()
        {
            inputManager.DetectGameInputType();
            inputManager.ProcessGameInputs();
        }
    }
}
