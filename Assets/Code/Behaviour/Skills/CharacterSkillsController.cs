using Chroma.Behaviour.Skills.Push;
using Chroma.ColorSystem;
using Chroma.Infrastructure.Input;
using UnityEngine;
using Zenject;
using Color = Chroma.ColorSystem.Color;

namespace Chroma.Behaviour.Skills
{
    [RequireComponent(typeof(Absorber))]
    public class CharacterSkillsController : MonoBehaviour
    {
        private Absorber absorber;
        private ColorSelector colorSelector;
        private InputManager inputManager;

        private PushSkill pushSkill;
        private bool absorbing;
        private AbsorptionType selectedAbsorptionType;

        [Inject]
        private void Inject(Absorber absorber, ColorSelector colorSelector, InputManager inputManager)
        {
            this.absorber = absorber;
            this.colorSelector = colorSelector;
            this.inputManager = inputManager;
        }

        private void Awake()
        {
            pushSkill = new PushSkill(transform);
            selectedAbsorptionType = AbsorptionType.Skill;
        }

        private void Update()
        {
            // TODO: reorganize input management
            GameInput input = inputManager.GetGameInput();
            
            if(absorbing)
            {
                if(input.GetButtonState(GameInputButton.Y) == GameInputButtonState.Released)
                {
                    float absorbedAmount = absorber.ExecuteAbsobption(colorSelector.SelectedColor);
                    if(selectedAbsorptionType == AbsorptionType.Skill)
                    {
                        UseSkill(colorSelector.SelectedColor, absorbedAmount);
                    }
                    
                    absorbing = false;
                }
            }
            else if(input.GetButtonState(GameInputButton.Y) == GameInputButtonState.Pressed)
            {
                absorber.StartAbsorption();
                absorbing = true;
            }
        }

        private void UseSkill(Color color, float amount)
        {
            if(color == Color.Red)
            {
                pushSkill.Push(amount);
            }
        }
    }
}
