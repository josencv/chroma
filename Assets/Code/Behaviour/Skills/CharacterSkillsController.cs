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
        private InputManager inputManager;
        private Absorber absorber;

        private PushSkill pushSkill;
        private bool absorbing;
        private Color selectedColor;
        private AbsorptionType selectedAbsorptionType;

        [Inject]
        private void Inject(Absorber absorber, InputManager inputManager)
        {
            this.absorber = absorber;
            this.inputManager = inputManager;
        }

        private void Awake()
        {
            pushSkill = new PushSkill(transform);
            selectedColor = Color.Blue;
            selectedAbsorptionType = AbsorptionType.Skill;
        }

        private void Update()
        {
            GameInput input = inputManager.GetGameInput();
            
            if(absorbing)
            {
                if(input.GetButtonState(GameInputButton.X) == GameInputButtonState.Released)
                {
                    float absorbedAmount = absorber.ExecuteAbsobption(selectedColor);
                    if(selectedAbsorptionType == AbsorptionType.Skill)
                    {
                        UseSkill(selectedColor, absorbedAmount);
                    }
                    
                    absorbing = false;
                }
            }
            else if(input.GetButtonState(GameInputButton.X) == GameInputButtonState.Pressed)
            {
                absorber.StartAbsorption();
                absorbing = true;
            }
        }

        private void UseSkill(Color color, float amount)
        {
            if(color == Color.Blue)
            {
                pushSkill.Push(amount);
            }
        }
    }
}
