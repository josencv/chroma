using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chroma.Components.Movement
{
    [RequireComponent(typeof(CharacterController), typeof(CharacterControllerMovement))]
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
