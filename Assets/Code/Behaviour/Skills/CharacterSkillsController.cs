using Chroma.Behaviour.Skills.Push;
using Chroma.Infrastructure.Input;
using UnityEngine;
using Zenject;

namespace Chroma.Behaviour.Skills
{
    public class CharacterSkillsController : MonoBehaviour
    {
        private InputManager inputManager;

        private PushSkill pushSkill;

        [Inject]
        private void Inject(InputManager inputManager)
        {
            this.inputManager = inputManager;
        }

        private void Awake()
        {
            pushSkill = new PushSkill(transform);
        }

        private void Update()
        {
            GameInput input = inputManager.GetGameInput();
            if(input.GetButtonState(GameInputButton.A) == GameInputButtonState.Pressed)
            {
                pushSkill.Charge(Time.deltaTime);
            }
            else if (input.GetButtonState(GameInputButton.A) == GameInputButtonState.Up)
            {
                pushSkill.Release();
            }
        }
    }
}
