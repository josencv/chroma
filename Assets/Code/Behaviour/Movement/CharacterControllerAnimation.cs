using UnityEngine;

namespace Chroma.Components.Movement
{
    [RequireComponent(typeof(CharacterController), typeof(CharacterControllerMovement), typeof(Animator))]
    public class CharacterControllerAnimation : MonoBehaviour
    {
        private Animator anim;

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        public void Animate(string parameter, float amount)
        {
            anim.SetFloat(parameter, amount);
        }
    }
}
