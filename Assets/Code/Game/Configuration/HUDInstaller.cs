using System;
using Chroma.Behaviour.AI.Components;
using Chroma.Behaviour.Attack;
using Chroma.Behaviour.Skills;
using Chroma.ColorSystem;
using Chroma.ColorSystem.Effects;
using Chroma.ColorSystem.UI;
using Chroma.Components.Movement;
using Chroma.Game.Containers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Chroma.Game.Configuration
{
    /// <summary>
    /// This is the 'pre-scene' DI installer of the game, meant to be used in a zenject scene context
    /// </summary>
    public class HUDInstaller : MonoInstaller
    {
        [SerializeField]
        private Settings settings;

        public override void InstallBindings()
        {
            Container.Bind<SelectedColorUIController>().FromComponentInHierarchy().AsSingle().NonLazy();
            Container.Bind<SelectedColorBackgroundUI>().FromComponentInChildren();
            Container.Bind<Image>().FromComponentSibling();
        }

        [Serializable]
        public class Settings
        {
        }
    }
}
