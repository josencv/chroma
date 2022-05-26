using UnityEngine;
using Zenject;

namespace Chroma.Behaviour.Movement
{
    [RequireComponent(typeof(CharacterController), typeof(Movable))]
    public class CharacterControllerAnimation : MonoBehaviour
    {
        private Animator animator;

        [Inject]
        private void Inject(Animator animator)
        {
            this.animator = animator;
        }

        public void Animate(string parameter, float amount)
        {
            animator.SetFloat(parameter, amount);
        }
    }
}
