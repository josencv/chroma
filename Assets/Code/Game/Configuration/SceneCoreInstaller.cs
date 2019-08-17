using Chroma.Components.Movement;
using Chroma.Game.Containers;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Chroma.Game.Configuration
{
    /// <summary>
    /// This is the 'global' DI installer of the game, meant to be used in a zenject project context
    /// </summary>
    public class SceneCoreInstaller : MonoInstaller
    {
        [SerializeField]
        private Settings settings;

        public override void InstallBindings()
        {
            // Note: bindings can be grouped and moved to different installers to encourage re-usability
            Container.Bind<Camera>().FromComponentInHierarchy().AsSingle();
            Container.Bind<CharacterContainer>().FromComponentInHierarchy().AsSingle();

            Container.Bind<CharacterControllerMovement>().FromComponentSibling();
            Container.Bind<CharacterController>().FromComponentSibling();
            Container.Bind<Animator>().FromComponentSibling();
            Container.Bind<CharacterControllerAnimation>().FromComponentSibling();
            Container.Bind<NavMeshAgent>().FromComponentSibling();
        }

        public class Settings
        {
        }
    }
}
