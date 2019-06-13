using Chroma.Infrastructure.Input;
using Chroma.Infrastructure.Unity;

namespace Chroma.Assets.Code.Game
{
    public class GameLoop : MonoBehaviourExtension
    {
        private InputManager inputManager;

        private void Awake()
        {
            inputManager = new InputManager();
        }

        private void Update()
        {
            inputManager.DetectGameInputType();
            inputManager.ProcessGameInputs();
        }
    }
}
