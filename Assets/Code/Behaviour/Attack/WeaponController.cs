﻿using Chroma.Behaviour.StateMachine;
using UnityEngine;
using Zenject;

namespace Chroma.Behaviour.Attack
{
    public enum WeaponState
    {
        Sheathed,
        Drawing,
        Sheathing,
        Drawn,
    }

    public enum AnimationLayer
    {
        Base,
        Battle,
        Weapon,
    }

    public class WeaponController : MonoBehaviour
    {
        private const string DrawWeaponAnimatorParamName = "drawWeapon";
        private const string SheatheWeaponAnimatorParamName = "sheatheWeapon";

        private EntityStateMachine entityStateMachine;
        private Animator animator;
        private Weapon weapon;

        [SerializeField]
        private Transform handAnchor;
        [SerializeField]
        private Transform sheathAnchor;
        [SerializeField]
        private Vector3 entityMeshScale = Vector3.one;

        private Vector3 inverseEntityMeshScale;
        private Vector3 inverseWeaponObjectScale;
        private WeaponState currentState;

        [Inject]
        private void Inject(Animator animator, Weapon weapon, [InjectOptional] EntityStateMachine entityStateMachine)
        {
            this.animator = animator;
            this.weapon = weapon;
            this.entityStateMachine = entityStateMachine;
        }

        private void Awake()
        {
            currentState = WeaponState.Sheathed;
        }

        private void Start()
        {
            inverseEntityMeshScale = new Vector3(1 / entityMeshScale.x, 1 / entityMeshScale.y, 1 / entityMeshScale.z);
            inverseWeaponObjectScale = new Vector3(1 / weapon.transform.localScale.x, 1 / weapon.transform.localScale.y, 1 / weapon.transform.localScale.z);
            if (weapon != null)
            {
                MatchEntityScale();
                AttachWeaponToSheath();
            }
        }

        public void DrawWeapon()
        {
            currentState = WeaponState.Drawing;
            if(weapon == null || animator == null)
            {
                Debug.LogError("Cannot draw weapon because no weapon or animator is present");
                return;
            }

            animator.SetTrigger(DrawWeaponAnimatorParamName);
        }

        public void SheatheWeapon()
        {
            currentState = WeaponState.Sheathing;
            if(weapon == null || animator == null)
            {
                Debug.LogError("Cannot draw weapon because no weapon or animator is present");
                return;
            }

            animator.SetTrigger(SheatheWeaponAnimatorParamName);
        }

        // This method is meant to be called by an animation event
        public void AttachWeaponToHand()
        {
            weapon.transform.SetParent(handAnchor.transform);
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
            currentState = WeaponState.Drawn;
            animator.SetLayerWeight((int)AnimationLayer.Battle, 1);
        }

        // This method is meant to be called by an animation event
        public void AttachWeaponToSheath()
        {
            weapon.transform.SetParent(sheathAnchor.transform, false);
            weapon.transform.localPosition = weapon.SheathTranslation;
            weapon.transform.localRotation = weapon.SheathRotation;
            currentState = WeaponState.Sheathed;
            animator.SetLayerWeight((int)AnimationLayer.Battle, 0);
        }

        /// <summary>
        /// Enables the weapon collider. Meant to be called by an animation event
        /// </summary>
        public void EnableWeaponHitbox()
        {
            if(weapon == null)
            {
                Debug.LogError("Cannot enable weapon hitbox: no weapon present");
                return;
            }

            weapon.EnableCollider();
        }

        public void OnDrawSheatheAnimationEnd()
        {
            entityStateMachine?.StateMachine.SetTrigger(EntityStateMachine.ActionEndedFieldName);
        }

        private void MatchEntityScale()
        {
            weapon.transform.localScale = Vector3.Scale(weapon.transform.localScale, inverseEntityMeshScale);
        }
    }
}
