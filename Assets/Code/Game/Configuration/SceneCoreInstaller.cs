using System;
using Chroma.Behaviour.AI.Components;
using Chroma.Behaviour.Attack;
using Chroma.ColorSystem;
using Chroma.Components.Movement;
using Chroma.Game.Containers;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Chroma.Game.Configuration
{
    /// <summary>
    /// This is the 'pre-scene' DI installer of the game, meant to be used in a zenject scene context
    /// </summary>
    public class SceneCoreInstaller : MonoInstaller
    {
        [SerializeField]
        private Settings settings;

        public override void InstallBindings()
        {
            // Note: bindings can be grouped and moved to different installers to encourage re-usability
            Container.Bind<Camera>().FromComponentInHierarchy().AsSingle();
            Container.Bind<Character>().FromComponentInHierarchy().AsSingle();

            Container.Bind<CharacterControllerMovement>().FromComponentSibling();
            Container.Bind<CharacterController>().FromComponentSibling();
            Container.Bind<Animator>().FromComponentSibling();
            Container.Bind<Rigidbody>().FromComponentSibling();
            Container.Bind<CharacterControllerAnimation>().FromComponentSibling();
            Container.Bind<SimpleAttack>().FromComponentSibling();
            Container.Bind<Collider>().FromComponentSibling();
            Container.Bind<Absorber>().FromComponentSibling();

            // AI related
            Container.Bind<NavMeshAgent>().FromComponentSibling();
            Container.Bind<Eyes>().FromComponentInChildren();

            // Color system
            Container.Bind<ColorProbe>().FromComponentsInHierarchy().AsSingle();
            Container.Bind<ColorProbeQuadrantSystem>().AsSingle().NonLazy();
            Container.Bind<ColorProbeRecoverySystem>().FromComponentInNewPrefab(settings.ColorProbeRecoverySystemPrefab).AsSingle();
            Container.Bind<AbsorptionRenderSystem>().FromComponentInNewPrefab(settings.AbsorptionRenderSystemPrefab).AsSingle();
            Container.Bind<ColorUnlockSystem>().FromComponentInNewPrefab(settings.ColorUnlockSystemPrefab).AsSingle().NonLazy();
        }

        [Serializable]
        public class Settings
        {
            public AbsorptionRenderSystem AbsorptionRenderSystemPrefab;
            public ColorProbeRecoverySystem ColorProbeRecoverySystemPrefab;
            public ColorUnlockSystem ColorUnlockSystemPrefab;
        }
    }
}
